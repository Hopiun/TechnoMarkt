using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Orders.ViewModels.Tables
{
    public class OrdersTableVM : TableVM<OrdersTableRow, OrdersFilter> { }
    
    public class OrdersTableRow
    {
        public int OrderId { get; set; }
        public int? ClientId { get; set; }
        public string ClientFullName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal OrderTotal { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        public bool IsPaid => PaymentStatus != null && PaymentStatus == Models.PaymentStatus.Completed;
    }

    public class OrdersFilter
    {
        public string? Search { get; set; }
        public OrderStatus? Status { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public bool PaidOnly { get; set; } = false;
        public bool UnpaidOnly { get; set; } = false;

        public bool DateRangeSpecified => DateTo != null;
    }
}






