using TechnoMarkt.Areas.Administrator.Employees.ViewModels.Forms;
using TechnoMarkt.Areas.Administrator.Employees.ViewModels;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Administrator.Employees
{
    public interface IEmployeeService
    {
        public Task<EmployeesKpiDto> GetKpiAsync(int storeId);
        public Task<Dictionary<EmployeeRole, decimal>> GetAverageSalaryByRoleAsync(int storeId);
        public Task<List<OperatorOrdersDto>> GetOperatorOrderStatsAsync(int storeId);
        public Task<List<EmployeeRowDto>> GetEmployeesAsync(int storeId, EmployeeFilter? filter);
        public Task<EmployeeFormVM?> GetEmployeeFormAsync(int employeeId);
        public Task<StoreFormVM?> GetStoreFormAsync(int storeId);
        public Task<(bool Succeeded, string NotificationText)> AddEmployeeAsync(int storeId, EmployeeFormVM newEmployee);
        public Task<(bool Succeeded, string NotificationText)> UpdateEmployeeAsync(int employeeId, EmployeeFormVM updatedEmployee);
        public Task<(bool Succeeded, string NotificationText)> UpdateStoreAsync(int storeId, StoreFormVM updatedStore);
        public Task<(bool Succeeded, string NotificationText)> DeactivateEmployeeAsync(int employeeId, int currentUserId);
        public Task<(bool Succeeded, string NotificationText)> ReactivateEmployeeAsync(int employeeId);

    }
}





