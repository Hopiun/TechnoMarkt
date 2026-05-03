using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Cart.ViewModels
{
    public class CartItemVM
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = null!;
        public decimal OriginalPrice { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountedPrice => DiscountPercent > 0 
            ? OriginalPrice * (1 - DiscountPercent / 100m) 
            : OriginalPrice;
        public int Quantity { get; set; }
        public decimal LineTotal => DiscountedPrice * Quantity;
    }
}



