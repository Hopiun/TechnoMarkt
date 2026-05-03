using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Shared.Common.ViewModels.Forms;

namespace TechnoMarkt.Areas.Manager.Suppliers.ViewModels
{
    public class SupplierFormVM : FormVM
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Введіть назву постачальника")]
        [StringLength(150, ErrorMessage = "Максимум 150 символів")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Максимум 255 символів")]
        public string? Contact { get; set; }

        [Required(ErrorMessage = "Оберіть країну")]
        public int? CountryId { get; set; }
        public SelectList? Countries { get; set; }

        [Range(0, 5, ErrorMessage = "Рейтинг має бути від 0 до 5")]
        public decimal? Rating { get; set; }
    }
}



