using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Shared.Common.ViewModels.Forms;
using TechnoMarkt.Areas.Manager.Attributes;

namespace TechnoMarkt.Areas.Manager.Catalog.ViewModels
{
    public class ItemFormVM : FormVM
    {
        public int? Id { get; set; }

        [Display(Name = "Назва товару")]
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(200, ErrorMessage = "Максимум 200 символів")]
        public string Name { get; set; } = null!;

        [Display(Name = "Категорія")]
        [Required(ErrorMessage = "Оберіть категорію")]
        public int? CategoryId { get; set; }
        public SelectList? Categories { get; set; }

        [Display(Name = "Бренд")]
        [Required(ErrorMessage = "Оберіть бренд")]
        public int? BrandId { get; set; }
        public SelectList? Brands { get; set; }

        [Display(Name = "Ціна (?)")]
        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, 999999.99, ErrorMessage = "Ціна має бути більше 0")]
        public decimal Price { get; set; }

        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Display(Name = "Вага (кг)")]
        [Range(0.01, 9999.99, ErrorMessage = "Вага має бути більше 0")]
        public double? Weight { get; set; }

        [Display(Name = "Габарити")]
        [StringLength(50, ErrorMessage = "Максимум 50 символів")]
        public string? Dimensions { get; set; }

        [Display(Name = "Постачальник")]
        [Required(ErrorMessage = "Оберіть постачальника")]
        public int? SupplierId { get; set; }
        public SelectList? Suppliers { get; set; }

        [Display(Name = "Фото товару")]
        [AllowedExtensions(new[] { ".jpg", ".jpeg", ".png", ".webp" })]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Максимальний розмір файлу — 5MB")]
        public IFormFile? Image { get; set; }
        public string? CurrentImageUrl { get; set; }
    }
}



