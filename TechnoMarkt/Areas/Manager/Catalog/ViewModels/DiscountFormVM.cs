using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Shared.Common.ViewModels.Forms;

namespace TechnoMarkt.Areas.Manager.Catalog.ViewModels
{
    public class DiscountFormVM : FormVM
    {
        public int? Id { get; set; }

        public int? ItemId { get; set; }
        public int? CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        [Display(Name = "Знижка (%)")]
        [Required(ErrorMessage = "Вкажіть відсоток знижки")]
        [Range(1, 99, ErrorMessage = "Знижка має бути від 1 до 99%")]
        public decimal Percent { get; set; } = 1;

        [Display(Name = "Дата початку")]
        [Required(ErrorMessage = "Вкажіть дату початку")]
        public DateOnly DateFrom { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Display(Name = "Дата закінчення")]
        [Required(ErrorMessage = "Вкажіть дату закінчення")]
        public DateOnly DateTo { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
    }
}



