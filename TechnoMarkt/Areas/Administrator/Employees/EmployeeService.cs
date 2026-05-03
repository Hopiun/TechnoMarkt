using TechnoMarkt.Areas.Administrator.Employees.Extensions;
using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Areas.Administrator.Employees.ViewModels.Forms;
using TechnoMarkt.Areas.Administrator.Employees.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Identity;

using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Common.ViewModels.Forms;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;


namespace TechnoMarkt.Areas.Administrator.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public EmployeeService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<EmployeesKpiDto> GetKpiAsync(int storeId)
        {
            IQueryable<Employee> activeEmployees = _context.Employees.ByStore(storeId).Active();

            Dictionary<EmployeeRole, (int, int)> presenceByRole = await activeEmployees
                .GroupBy(employee => employee.Role)
                .Select(group => new
                {
                    Role = group.Key,
                    ActiveCount = group.Count(e => e.Status == EmployeeStatus.Present),
                    Overall = group.Count()
                })
                .ToDictionaryAsync(x => x.Role, x => (x.ActiveCount, x.Overall));

            (int, int) operatorsPresence = presenceByRole.GetValueOrDefault(EmployeeRole.Operator, (0, 0));
            (int, int) managersPresence = presenceByRole.GetValueOrDefault(EmployeeRole.Manager, (0, 0));

            decimal averageSalary = await activeEmployees.AverageAsync(employee => employee.Salary) ?? 0m;

            return new EmployeesKpiDto(operatorsPresence, managersPresence, averageSalary);
        }

        public async Task<Dictionary<EmployeeRole, decimal>> GetAverageSalaryByRoleAsync(int storeId)
        {
            return await _context.Employees.ByStore(storeId).Active()
                .GroupBy(employee => employee.Role)
                .Select(group => new
                {
                    Role = group.Key,
                    AverageSalary = group.Average(e => (decimal?)e.Salary) ?? 0m
                })
                .ToDictionaryAsync(x => x.Role, x => x.AverageSalary);
        }

        public async Task<List<OperatorOrdersDto>> GetOperatorOrderStatsAsync(int storeId)
        {
            DateRange month = DateRange.ThisMonth;

            return await _context.Employees.ByStore(storeId).WithRole(EmployeeRole.Operator)
                .Select(employee => new OperatorOrdersDto(employee.EmployeeId, $"{employee.FirstName} {employee.LastName}",

                    employee.Orders.Count(order => order.OrderDate >= month.From && order.OrderDate <= month.To
                        && order.Status == OrderStatus.Completed),
                    employee.Orders.Where(order => order.OrderDate >= month.From && order.OrderDate <= month.To
                        && order.Status == OrderStatus.Completed)
                        .Sum(order => (decimal?)order.OrderTotal) ?? 0))
                .ToListAsync();
        }

        public async Task<List<EmployeeRowDto>> GetEmployeesAsync(int storeId, EmployeeFilter? filter)
        {
            IQueryable<Employee> query = _context.Employees.ByStore(storeId);

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(employee => employee.FirstName.Contains(filter.Name) || employee.LastName.Contains(filter.Name));

                if (filter.Role.HasValue)
                    query = query.WithRole(filter.Role.Value);

                if (filter.Status.HasValue)
                    query = query.WithStatus(filter.Status.Value);
            }

            return await query.Select(employee => new EmployeeRowDto()
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Role = employee.Role,
                Status = employee.Status,
                Salary = employee.Salary
            })
                .ToListAsync();
        }

        public async Task<EmployeeFormVM?> GetEmployeeFormAsync(int employeeId)
        {
            return await _context.Employees
                .Where(employee => employee.EmployeeId == employeeId)
                .Select(employee => new EmployeeFormVM()
                {
                    Action = FormAction.Edit,
                    Id = employee.EmployeeId,
                    UserId = employee.UserId,
                    StoreId = employee.StoreId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Role = employee.Role,
                    Status = employee.Status,
                    Salary = employee.Salary
                })
                .FirstOrDefaultAsync();
        }

        public async Task<StoreFormVM?> GetStoreFormAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);
            if (store == null) return null;

            var cities = await _context.Cities.ToListAsync();
            var admins = await _context.Employees
                .Where(e => e.StoreId == storeId && e.Role == EmployeeRole.Administrator)
                .ToListAsync();

            return new StoreFormVM()
            {
                Action = FormAction.Edit,
                StoreId = store.StoreId,
                CityId = store.CityId,
                AdminId = store.AdminId,
                Address = store.Address,
                Phone = store.Phone,
                Cities = new SelectList(cities, nameof(City.CityId), nameof(City.Name)),
                Administrators = new SelectList(admins, nameof(Employee.EmployeeId), nameof(Employee.FirstName))
            };
        }

        public async Task<(bool Succeeded, string NotificationText)> AddEmployeeAsync(int storeId, EmployeeFormVM newEmployee)
        {
            if (string.IsNullOrWhiteSpace(newEmployee.Email))
                return (false, "Email є обов'язковим");

            if (string.IsNullOrWhiteSpace(newEmployee.Password))
                return (false, "Пароль є обов'язковим");

            AppUser? existingUser = await _userManager.FindByEmailAsync(newEmployee.Email!);

            if (existingUser != null)
                return (false, $"Email {existingUser.Email} вже використовується");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                AppUser user = new AppUser()
                {
                    UserName = newEmployee.Email,
                    Email = newEmployee.Email
                };

                IdentityResult result = await _userManager.CreateAsync(user, newEmployee.Password!);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                Employee employee = new Employee()
                {
                    UserId = user.Id,
                    StoreId = storeId,
                    FirstName = newEmployee.FirstName,
                    LastName = newEmployee.LastName,
                    Role = newEmployee.Role,
                    Status = EmployeeStatus.Present,
                    Salary = newEmployee.Salary
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                await _userManager.AddClaimsAsync(user, new[]
                {
                    new Claim(ClaimTypes.Role, employee.Role.ToString()),
                    new Claim("StoreId", storeId.ToString()),
                    new Claim("EmployeeId", employee.EmployeeId.ToString())
                });

                await transaction.CommitAsync();

                return (true, $"Працівника {newEmployee.FirstName} {newEmployee.LastName} (ID: {employee.EmployeeId}) успішно додано");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMsg = ex.InnerException?.InnerException?.Message
                            ?? ex.InnerException?.Message
                            ?? ex.Message;
                return (false, $"Помилка при створенні працівника: {innerMsg}");
            }
        }

        public async Task<(bool Succeeded, string NotificationText)> UpdateEmployeeAsync(int employeeId, EmployeeFormVM updatedEmployee)
        {
            Employee? employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return (false, $"Співробітника магазину з ID:{employeeId} не знайдено");

            if (updatedEmployee.Role == EmployeeRole.Administrator && employee.Role != EmployeeRole.Administrator)
            {
                bool storeHasAdmin = await _context.Employees
                    .AnyAsync(e => e.StoreId == employee.StoreId && e.Role == EmployeeRole.Administrator);

                if (storeHasAdmin)
                    return (false, "Неможливо змінити роль працівника на 'Адміністратор', бо в магазині вже є адміністратор!");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            employee.FirstName = updatedEmployee.FirstName;
            employee.LastName = updatedEmployee.LastName;
            employee.Role = updatedEmployee.Role;
            employee.Status = updatedEmployee.Status;
            employee.Salary = updatedEmployee.Salary;

            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
            if (user != null)
            {
                var oldClaims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimsAsync(user, oldClaims);
                await _userManager.AddClaimsAsync(user, new[]
                {
                    new Claim(ClaimTypes.Role, employee.Role.ToString()),
                    new Claim("StoreId", employee.StoreId.ToString()),
                    new Claim("EmployeeId", employee.EmployeeId.ToString())
                });
            }

            await transaction.CommitAsync();

            return (true, $"Успішно змінено дані працівника з ID:{employee.EmployeeId}");
        }

        public async Task<(bool Succeeded, string NotificationText)> UpdateStoreAsync(int storeId, StoreFormVM updatedStore)
        {
            Store? store = await _context.Stores.FindAsync(storeId);
            if (store == null) return (false, $"Магазин з ID:{storeId} не знайдено");

            store.CityId = updatedStore.CityId;
            store.Address = updatedStore.Address;
            store.Phone = updatedStore.Phone;
            store.AdminId = updatedStore.AdminId;

            try
            {
                await _context.SaveChangesAsync();
                return (true, $"Успішно оновлено дані магазину (ID:{store.StoreId})");
            }
            catch (Exception ex)
            {
                return (false, $"Помилка оновлення магазину: {ex.Message}");
            }
        }

        public async Task<(bool Succeeded, string NotificationText)> DeactivateEmployeeAsync(int employeeId, int currentUserId)
        {
            Employee? employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return (false, $"Співробітника магазину з ID:{employeeId} не знайдено");

            if (employee.Status == EmployeeStatus.Former)
                return (false, $"{employee.FirstName} {employee.LastName} вже є деактивованим працівником");

            if (employee.UserId == currentUserId)
                return (false, "Не можна деактивувати власний обліковий запис");

            if (employee.Role == EmployeeRole.Administrator)
            {
                int activeAdminCount = await _context.Employees.ByStore(employee.StoreId)
                    .WithRole(EmployeeRole.Administrator).Active()
                    .CountAsync(e => e.EmployeeId != employeeId);

                if (activeAdminCount == 0)
                    return (false, "Не можна деактивувати єдиного адміністратора магазину. Спочатку призначте іншого адміністратора.");
            }

            if (employee.Role == EmployeeRole.Operator || employee.Role == EmployeeRole.Manager)
            {
                int activeCount = await _context.Employees.ByStore(employee.StoreId).WithRole(employee.Role).Active()
                    .CountAsync(e => e.EmployeeId != employeeId);

                if (activeCount == 0)
                    return (false, $"Не можна деактивувати останнього активного " +
                                  $"{(employee.Role == EmployeeRole.Operator ? "касира" : "менеджера")}");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            employee.Status = EmployeeStatus.Former;
            await _context.SaveChangesAsync();

            if (employee.Role == EmployeeRole.Administrator)
            {
                Store? store = await _context.Stores.FindAsync(employee.StoreId);
                if (store != null && store.AdminId == employee.EmployeeId)
                {
                    Employee? nextAdmin = await _context.Employees.ByStore(employee.StoreId)
                        .WithRole(EmployeeRole.Administrator).Active()
                        .FirstOrDefaultAsync();

                    if (nextAdmin != null)
                    {
                        store.AdminId = nextAdmin.EmployeeId;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
            if (user != null)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimsAsync(user, claims);
            }

            await transaction.CommitAsync();

            return (true, $"Запис працівника {employee.FirstName} {employee.LastName} (ID: {employee.EmployeeId}) деактивовано");
        }

        public async Task<(bool Succeeded, string NotificationText)> ReactivateEmployeeAsync(int employeeId)
        {
            Employee? employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return (false, $"Співробітника з ID:{employeeId} не знайдено");

            if (employee.Status != EmployeeStatus.Former)
                return (false, $"{employee.FirstName} {employee.LastName} вже є активним працівником");

            employee.Status = EmployeeStatus.Present;
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
            if (user != null)
            {
                var oldClaims = await _userManager.GetClaimsAsync(user);
                if (oldClaims.Any())
                    await _userManager.RemoveClaimsAsync(user, oldClaims);

                await _userManager.AddClaimsAsync(user, new[]
                {
                    new Claim(ClaimTypes.Role, employee.Role.ToString()),
                    new Claim("StoreId", employee.StoreId.ToString()),
                    new Claim("EmployeeId", employee.EmployeeId.ToString())
                });
            }

            return (true, $"Працівника {employee.FirstName} {employee.LastName} (ID: {employee.EmployeeId}) реактивовано");
        }
    }
}






