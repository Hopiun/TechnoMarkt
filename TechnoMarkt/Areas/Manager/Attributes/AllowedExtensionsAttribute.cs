using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Areas.Manager.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is not IFormFile file)
                return ValidationResult.Success;

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!_extensions.Contains(extension))
                return new ValidationResult($"Дозволені формати: {string.Join(", ", _extensions)}");

            return ValidationResult.Success;
        }
    }
}