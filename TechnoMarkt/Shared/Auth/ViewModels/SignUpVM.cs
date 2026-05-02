using TechnoMarkt.Models;
using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Shared.Auth.ViewModels
{
    public class SignUpVM
    {
        [Required(ErrorMessage = "Ѕудь ласка, вед≥ть ≥м'€")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ѕудь ласка, вед≥ть пр≥звище")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ѕудь ласка, вед≥ть email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ѕудь ласка, вед≥ть пароль")]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "ѕарол≥ не сп≥впадають")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}



