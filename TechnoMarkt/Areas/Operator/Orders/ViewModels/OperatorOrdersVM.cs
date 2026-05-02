using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Kpi;

namespace TechnoMarkt.Areas.Operator.Orders.ViewModels
{
    public class OperatorOrdersViewModel : OrdersKpiVM
    {
        public Dictionary<OrderStatus, int> OrderCountByStatus { get; set; } = [];
    }
}



