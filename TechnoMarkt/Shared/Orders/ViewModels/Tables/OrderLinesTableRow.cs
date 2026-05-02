using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Orders.ViewModels.Tables
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal OrderTotal { get; set; }
        public bool IsPaid { get; set; }
        public bool CanBeReturned { get; set; }
        public string Role { get; set; } = string.Empty;
        public List<OrderLineRow> Rows { get; set; } = [];
        public List<PaymentMethodRow> ClientPaymentMethods { get; set; } = [];
    }

    public class OrderLineRow
    {
        public int ItemId { get; set; }
        public string? ImageUrl { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; }
        public int? DiscountPercent { get; set; }

        public decimal LineTotal => (1 - (DiscountPercent ?? 0) / 100m) * PriceAtMoment * Quantity;
    }

    public class PaymentMethodRow
    {
        public int PaymentMethodId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
    }
}



