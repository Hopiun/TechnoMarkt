using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Forms;
using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Areas.Administrator.Employees.ViewModels.Forms
{
    public class EmployeeFormVM : FormVM
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }

        public int StoreId { get; set; }

        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string? Email { get; set; }

        [MinLength(8, ErrorMessage = "Мінімум 8 символів")]
        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ведіть ім'я працівника")]
        [StringLength(100, ErrorMessage = "Максимум 100 символів")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ведіть прізвище працівника")]
        [StringLength(100, ErrorMessage = "Максимум 100 символів")]
        public string LastName { get; set; } = string.Empty;

        public EmployeeRole Role { get; set; } = EmployeeRole.Operator;

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Present;

        [Range(0, 1_000_000, ErrorMessage = "Зарплата от 0 до 1 000 000")]
        public decimal? Salary { get; set; }
    }
}




