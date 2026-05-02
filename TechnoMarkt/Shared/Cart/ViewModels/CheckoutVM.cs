using TechnoMarkt.Models;
using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Shared.Cart.ViewModels
{
    public class CheckoutVM
    {
        [Required(ErrorMessage = "Оберіть спосіб оплати")]
        [Display(Name = "Спосіб оплати")]
        public PayMethod PaymentMethod { get; set; }

        [Display(Name = "Коментар до замовлення")]
        [StringLength(500)]
        public string? Comment { get; set; }
    }
}



