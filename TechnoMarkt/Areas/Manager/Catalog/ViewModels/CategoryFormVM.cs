using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Shared.Common.ViewModels.Forms;

namespace TechnoMarkt.Areas.Manager.Catalog.ViewModels
{
    public class CategoryFormVM : FormVM
    {
        public int? Id { get; set; }

        [Display(Name = "Назва категорії")]
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, ErrorMessage = "Максимум 100 символів")]
        public string Name { get; set; } = null!;

        [Display(Name = "Батьківська категорія")]
        public int? ParentCategoryId { get; set; }
        public SelectList? ParentCategories { get; set; }
    }
}



