using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Cart.ViewModels
{
    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal TotalOriginalAmount => Items.Sum(i => i.OriginalPrice * i.Quantity);
        public decimal TotalAmount => Items.Sum(i => i.LineTotal);
        public decimal TotalDiscount => TotalOriginalAmount - TotalAmount;
    }
}



