using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Dashboard.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardKpiDto Kpi { get; set; } = null!;
        public Dictionary<string, decimal> SalesByCategory { get; set; } = [];
        public List<TransactionsTableRow> RecentTransactions { get; set; } = [];
        public Dictionary<DateOnly, decimal> DailySales { get; set; } = [];
    }

    public record DashboardKpiDto(decimal MonthlyRevenue, decimal AverageOrder, int CompletedOrdersCount, int CriticalStockCount);
}






