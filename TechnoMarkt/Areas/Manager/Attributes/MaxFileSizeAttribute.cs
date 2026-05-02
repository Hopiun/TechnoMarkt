using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Areas.Manager.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxBytes;

        public MaxFileSizeAttribute(int maxBytes) => _maxBytes = maxBytes;

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file)
                return ValidationResult.Success;

            if (file.Length > _maxBytes)
                return new ValidationResult(ErrorMessage ?? $"Максимальний розмір — {_maxBytes / 1024 / 1024}MB");

            return ValidationResult.Success;
        }
    }
}