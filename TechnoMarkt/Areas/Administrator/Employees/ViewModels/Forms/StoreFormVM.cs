using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Shared.Common.ViewModels.Forms;

namespace TechnoMarkt.Areas.Administrator.Employees.ViewModels.Forms
{
    public class StoreFormVM : FormVM
    {
        public int? StoreId { get; set; }

        [Required(ErrorMessage = "Оберіть місто")]
        [Range(1, int.MaxValue, ErrorMessage = "Будь ласка, оберіть місто")]
        public int CityId { get; set; }

        public int? AdminId { get; set; }

        [Required(ErrorMessage = "Введіть адресу магазину")]
        [StringLength(200, ErrorMessage = "Довжина адреси магазину не може бути більшою за 200 символів")]
        public string Address { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Невірний формат телефону")]
        [StringLength(13)]
        public string? Phone { get; set; }

        public SelectList? Cities { get; set; }
        public SelectList? Administrators { get; set; }
    }
}
