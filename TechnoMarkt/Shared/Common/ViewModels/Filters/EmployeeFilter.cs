using TechnoMarkt.Models;

namespace TechnoMarkt.Shared.Common.ViewModels.Filters
{
    public class EmployeeFilter
    {
        public string? Name { get; set; } = null;
        public EmployeeRole? Role { get; set; } = null;
        public EmployeeStatus? Status { get; set; } = null;
    }
}


