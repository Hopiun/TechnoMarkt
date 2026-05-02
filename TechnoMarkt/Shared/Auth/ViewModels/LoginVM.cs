using TechnoMarkt.Models;
using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Shared.Auth.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Ѕудь ласка, вед≥ть email")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ѕудь ласка, вед≥ть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}



