using TechnoMarkt.Models;
using System.ComponentModel.DataAnnotations;

namespace TechnoMarkt.Shared.Cart.ViewModels
{
    public class AddToCartVM
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        [Range(1, 999, ErrorMessage = "Кількість повинна бути від 1 до 999")]
        public int Quantity { get; set; } = 1;
    }
}



