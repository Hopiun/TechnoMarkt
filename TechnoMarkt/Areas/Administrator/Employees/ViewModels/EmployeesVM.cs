using TechnoMarkt.Areas.Administrator.Employees.ViewModels.Forms;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Administrator.Employees.ViewModels
{
    public class EmployeesViewModel
    {
        public EmployeesKpiDto Kpi { get; set; } = null!;
        public Dictionary<EmployeeRole, decimal> AverageSalaryByRole { get; set; } = [];
        public List<OperatorOrdersDto> OperatorOrderStats { get; set; } = [];
        public StoreFormVM? StoreEditForm { get; set; }
        public EmployeeFilter? EmployeeFilter { get; set; }
        public List<EmployeeRowDto> Employees { get; set; } = [];
        public EmployeeFormVM? EmployeeForm { get; set; }
    }

    public record EmployeesKpiDto((int, int) OperatorsPresence, (int, int) ManagersPresence, decimal AverageSalary);
    public record OperatorOrdersDto(int EmployeeId, string FullName, int OrdersCount, decimal Sum);

    public class EmployeeRowDto()
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmployeeRole Role { get; set; }
        public decimal? Salary { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}





