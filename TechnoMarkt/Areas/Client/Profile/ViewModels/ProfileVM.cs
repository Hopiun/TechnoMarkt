using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Areas.Client.Profile.ViewModels
{
    public class ClientProfileViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal WalletBalance { get; set; }
    }

    public class EditProfileFormVM
    {
        [Required(ErrorMessage = "Поле Ім'я є обов'язковим")]
        [StringLength(100, ErrorMessage = "Максимальна довжина 100 символів")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле Прізвище є обов'язковим")]
        [StringLength(100, ErrorMessage = "Максимальна довжина 100 символів")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;
    }

    public class ChangeEmailFormVM
    {
        [Required(ErrorMessage = "Поле Новий Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Невірний формат Email")]
        [Display(Name = "Новий Email")]
        public string NewEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле Поточний пароль є обов'язковим")]
        [Display(Name = "Поточний пароль")]
        public string CurrentPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordFormVM
    {
        [Required(ErrorMessage = "Поле Поточний пароль є обов'язковим")]
        [Display(Name = "Поточний пароль")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле Новий пароль є обов'язковим")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
        [Display(Name = "Новий пароль")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження паролю")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}


