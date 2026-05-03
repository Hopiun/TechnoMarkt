using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Kpi;

namespace TechnoMarkt.Areas.Administrator.Orders.ViewModels
{
    public class AdminOrdersViewModel : OrdersKpiVM
    {
        public MonthYearFilter TopOrderChartFilter { get; set; } = new();
        public List<OrdersTableRow> TopOrders { get; set; } = [];
        public Dictionary<OrderStatus, int> OrdersCountByStatus { get; set; } = [];
    }
}



