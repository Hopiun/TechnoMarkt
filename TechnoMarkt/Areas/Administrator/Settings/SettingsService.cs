using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Areas.Administrator.Settings.ViewModels;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _context;
        private readonly IEventLogsService _eventLogsService;
        private readonly IWebHostEnvironment _env;

        public SettingsService(AppDbContext context, IEventLogsService eventLogsService, IWebHostEnvironment env)
        {
            _context = context;
            _eventLogsService = eventLogsService;
            _env = env;
        }

        public async Task<SettingsViewModel> GetSettingsPageAsync()
        {
            var settings = await _context.AppSettings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return new SettingsViewModel
            {
                AppSettings = settings,
                EventLogsTable = await GetEventLogsTableAsync(new EventLogsFilter(), pageSize: 50)
            };
        }

        public async Task<EventLogsTableVM> GetEventLogsTableAsync(EventLogsFilter filter, int? pageSize = null, int currentPage = 1)
        {
            IQueryable<EventLog> query = _context.EventLogs.OrderByDescending(e => e.CreatedAt);

            if (!string.IsNullOrEmpty(filter.Role))
                query = query.Where(e => e.Role == filter.Role);
            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(e => e.Action == filter.Action);
            if (!string.IsNullOrEmpty(filter.Entity))
                query = query.Where(e => e.Entity == filter.Entity);

            if (!string.IsNullOrEmpty(filter.SearchUser))
            {
                var userIds = await _context.Users
                    .Where(u => u.UserName!.Contains(filter.SearchUser) || u.Email!.Contains(filter.SearchUser))
                    .Select(u => u.Id.ToString())
                    .ToListAsync();

                query = query.Where(e => userIds.Contains(e.UserId!));
            }

            int totalCount = await query.CountAsync();
            var users = await _context.Users.ToDictionaryAsync(u => u.Id.ToString(), u => u.UserName ?? u.Email ?? "Unknown");

            if (pageSize.HasValue)
                query = query.Skip((currentPage - 1) * pageSize.Value).Take(pageSize.Value);

            var logs = await query.ToListAsync();

            return new EventLogsTableVM
            {
                Rows = logs.Select(l => new EventLogsTableRow
                {
                    LogId = l.LogId,
                    UserId = l.UserId,
                    UserFullName = l.UserId != null && users.ContainsKey(l.UserId) ? users[l.UserId] : "—",
                    Role = l.Role ?? "—",
                    Action = l.Action,
                    Entity = l.Entity,
                    EntityId = l.EntityId,
                    Description = l.Description ?? "—",
                    CreatedAt = l.CreatedAt
                }).ToList(),
                Filter = filter,
                CurrentPage = pageSize.HasValue ? currentPage : 1,
                PageSize = pageSize ?? logs.Count,
                TotalCount = totalCount
            };
        }

        public async Task<(bool Succeeded, string Message)> UpdateSettingsAsync(Dictionary<string, string> settings)
        {
            foreach (var kvp in settings)
            {
                var value = kvp.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var existing = await _context.AppSettings.FindAsync(kvp.Key);
                if (existing != null)
                    existing.Value = value;
                else
                    _context.AppSettings.Add(new AppSetting { Key = kvp.Key, Value = value });
            }

            await _context.SaveChangesAsync();
            await _eventLogsService.LogAsync("Update", "AppSettings", "Updated system settings");
            return (true, "Налаштування успішно оновлено");
        }

        public async Task<(bool Succeeded, string Message, byte[]? FileBytes, string? FileName)> CreateBackupAsync()
        {
            var backupPathSetting = await _context.AppSettings.FirstOrDefaultAsync(s => s.Key == "BackupPath");
            string backupDir = (backupPathSetting?.Value ?? Path.Combine(_env.WebRootPath, "backups")).Trim().Trim('"');

            if (!Directory.Exists(backupDir))
                Directory.CreateDirectory(backupDir);

            string fileName = $"TechnoMarkt_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
            string fullPath = Path.Combine(backupDir, fileName);

            string sql = await GenerateBackupSqlAsync();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sql);

            await File.WriteAllBytesAsync(fullPath, bytes);
            await _eventLogsService.LogAsync("Backup", "Database", $"Backup saved to: {fullPath}");

            return (true, "OK", bytes, fileName);
        }

        public async Task<(bool Succeeded, string Message)> RestoreBackupAsync(IFormFile backupFile)
        {
            if (backupFile == null || backupFile.Length == 0)
                return (false, "Оберіть файл для відновлення");

            using var reader = new StreamReader(backupFile.OpenReadStream());
            var sql = await reader.ReadToEndAsync();
            var batches = SplitSqlBatches(sql);

            var connection = _context.Database.GetDbConnection();
            var wasClosed = connection.State == System.Data.ConnectionState.Closed;
            if (wasClosed)
                await connection.OpenAsync();

            try
            {
                using (var disableCmd = connection.CreateCommand())
                {
                    disableCmd.CommandText = "EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';";
                    disableCmd.CommandTimeout = 300;
                    await disableCmd.ExecuteNonQueryAsync();
                }

                var deleteOrder = new[]
                {
                    "Review", "EventLog",
                    "Warehouse", "Payment", "OrderLine", "Order",
                    "Employee", "Client", "Discount", "Item", "Supplier",
                    "Store", "PaymentMethod", "Brand", "Category", "Country", "City"
                };

                foreach (var table in deleteOrder)
                {
                    using var delCmd = connection.CreateCommand();
                    delCmd.CommandText = $"DELETE FROM [{table}];";
                    delCmd.CommandTimeout = 300;
                    await delCmd.ExecuteNonQueryAsync();
                }

                foreach (var batch in batches)
                {
                    var trimmed = batch.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed))
                        continue;

                    if (trimmed.StartsWith("SET IDENTITY_INSERT", StringComparison.OrdinalIgnoreCase)
                        || trimmed.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase))
                    {
                        using var cmd = connection.CreateCommand();
                        cmd.CommandText = trimmed;
                        cmd.CommandTimeout = 300;
                        try
                        {
                            await cmd.ExecuteNonQueryAsync();
                        }
                        catch when (trimmed.StartsWith("SET IDENTITY_INSERT", StringComparison.OrdinalIgnoreCase))
                        {
                        }
                        continue;
                    }

                    var lines = trimmed.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        var sqlLine = line.Trim();
                        if (string.IsNullOrEmpty(sqlLine) || sqlLine.StartsWith("--"))
                            continue;

                        using var cmd = connection.CreateCommand();
                        cmd.CommandText = sqlLine;
                        cmd.CommandTimeout = 300;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                using (var enableCmd = connection.CreateCommand())
                {
                    enableCmd.CommandText = "EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';";
                    enableCmd.CommandTimeout = 300;
                    await enableCmd.ExecuteNonQueryAsync();
                }

                await _eventLogsService.LogAsync("Restore", "Backup", "Restored database from backup");
                return (true, "Базу даних успішно відновлено");
            }
            catch (Exception ex)
            {
                await _eventLogsService.LogAsync("RestoreFailed", "Backup", $"Restore failed: {ex.Message}");
                return (false, $"Відновлення не вдалось. Помилка: {ex.Message}");
            }
            finally
            {
                if (wasClosed)
                    connection.Close();
            }
        }

        private async Task<string> GenerateBackupSqlAsync()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("-- TechnoMarkt Database Backup");
            sb.AppendLine($"-- Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("GO");
            sb.AppendLine("-- Disable FK constraints during restore");
            sb.AppendLine("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';");
            sb.AppendLine("GO");
            sb.AppendLine();

            var tables = new[]
            {
                "City", "Country", "Category", "Brand", "PaymentMethod",
                "Supplier", "Item", "Discount", "Client", "Employee", "Store",
                "Order", "OrderLine", "Payment", "Warehouse", "EventLog",
                "AppSettings", "Review"
            };

            var connection = _context.Database.GetDbConnection();
            var wasClosed = connection.State == System.Data.ConnectionState.Closed;
            if (wasClosed)
                await connection.OpenAsync();

            using var cmd = connection.CreateCommand();

            foreach (var table in tables)
            {
                cmd.CommandText = $"SELECT * FROM [{table}]";
                using var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    reader.Close();
                    continue;
                }

                sb.AppendLine($"-- Data from {table}");
                sb.AppendLine($"SET IDENTITY_INSERT [{table}] ON;");

                var fieldCount = reader.FieldCount;
                var columnNames = new string[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                    columnNames[i] = $"[{reader.GetName(i)}]";

                while (await reader.ReadAsync())
                {
                    var values = new List<string>();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        var value = reader.GetValue(i);
                        if (value == null || value == DBNull.Value)
                            values.Add("NULL");
                        else if (value is DateTime dt)
                            values.Add($"'{dt:yyyy-MM-dd HH:mm:ss}'");
                        else if (value is string s)
                            values.Add($"'{s.Replace("'", "''")}'");
                        else if (value is bool b)
                            values.Add(b ? "1" : "0");
                        else if (value is Guid g)
                            values.Add($"'{g}'");
                        else if (value is decimal or double or float)
                            values.Add(value.ToString()!.Replace(",", "."));
                        else
                            values.Add(value.ToString()!);
                    }
                    sb.AppendLine($"INSERT INTO [{table}] ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", values)});");
                }

                sb.AppendLine($"SET IDENTITY_INSERT [{table}] OFF;");
                sb.AppendLine("GO");
                sb.AppendLine();

                reader.Close();
            }

            if (wasClosed)
                connection.Close();

            sb.AppendLine("-- Re-enable FK constraints");
            sb.AppendLine("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';");
            sb.AppendLine("GO");

            return sb.ToString();
        }

        private static List<string> SplitSqlBatches(string sql)
        {
            var batches = new List<string>();
            var lines = sql.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var currentBatch = new System.Text.StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentBatch.Length > 0)
                    {
                        var batchText = currentBatch.ToString().Trim();
                        if (!string.IsNullOrEmpty(batchText))
                            batches.Add(batchText);
                        currentBatch.Clear();
                    }
                    continue;
                }

                if (trimmedLine.StartsWith("--"))
                    continue;

                currentBatch.AppendLine(line);
            }

            if (currentBatch.Length > 0)
            {
                var batchText = currentBatch.ToString().Trim();
                if (!string.IsNullOrEmpty(batchText))
                    batches.Add(batchText);
            }

            return batches;
        }
    }
}
