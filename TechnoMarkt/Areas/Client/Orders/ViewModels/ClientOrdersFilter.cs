using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Client.Orders.ViewModels
{
    public class ClientOrdersFilter
    {
        public OrderStatus? Status { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}

