using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Administrator.Finance.ViewModels
{
    public class FinanceViewModel
    {
        public DateRangeFilter DateFilter { get; set; } = new();
        public FinanceKpiDto Kpi { get; set; } = null!;
        public Dictionary<PayMethod, int> TransactionByMethod { get; set; } = [];
        public DateOnly WeekStart { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1));
        public Dictionary<DateOnly, decimal> DailySales { get; set; } = [];
        public int QuarterlyYear { get; set; } = DateTime.Now.Year;
        public List<QuarterlyRevenueDto> QuarterlyRevenue { get; set; } = [];
        public TransactionsTableVM TransactionsTable { get; set; } = new();
    }

    public record FinanceKpiDto(decimal YearlyRevenue, decimal PeriodRevenue, int CompletedOrdersCount, decimal AverageOrder);

    public record QuarterlyRevenueDto(int Quarter, int Year, decimal Revenue);
}




