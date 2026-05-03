using TechnoMarkt.Models;
using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Shared.Auth.ViewModels
{
    public class SignUpVM
    {
        [Required(ErrorMessage = "Будь ласка, ведіть ім'я")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, ведіть прізвище")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, ведіть email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, ведіть пароль")]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}



