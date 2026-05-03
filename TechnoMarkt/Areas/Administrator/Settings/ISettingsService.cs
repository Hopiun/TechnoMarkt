using TechnoMarkt.Areas.Administrator.Settings.ViewModels;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Settings
{
    public interface ISettingsService
    {
        Task<SettingsViewModel> GetSettingsPageAsync();
        Task<EventLogsTableVM> GetEventLogsTableAsync(EventLogsFilter filter, int? pageSize = null, int currentPage = 1);
        Task<(bool Succeeded, string Message)> UpdateSettingsAsync(Dictionary<string, string> settings);
        Task<(bool Succeeded, string Message, byte[]? FileBytes, string? FileName)> CreateBackupAsync();
        Task<(bool Succeeded, string Message)> RestoreBackupAsync(IFormFile backupFile);
    }
}
