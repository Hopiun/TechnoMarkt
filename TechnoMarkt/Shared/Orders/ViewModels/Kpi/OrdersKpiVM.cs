using TechnoMarkt.Areas.Administrator.Employees.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Shared.Orders.ViewModels.Kpi
{
    public class OrdersKpiVM
    {
        public OrdersKpiDto Kpi { get; set; } = null!;
        public OrdersTableVM OrdersTable { get; set; } = new();
    }

    public record OrdersKpiDto(int TodayActiveCount, decimal TodayPaidTotal, decimal TodayPaidCashTotal, decimal TodayPaidCardTotal);
}





