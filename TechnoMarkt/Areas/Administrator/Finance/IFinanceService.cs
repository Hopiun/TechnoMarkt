using TechnoMarkt.Areas.Administrator.Finance.ViewModels;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Finance
{
    public interface IFinanceService
    {
        public Task<FinanceKpiDto> GetKpiAsync(int storeId, DateRangeFilter filter);
        public Task<Dictionary<PayMethod, int>> GetTransactionsByMethodAsync(int storeId, DateRangeFilter filter);
        public Task<Dictionary<DateOnly, decimal>> GetDailySalesAsync(int storeId, DateOnly weekStart);
        public Task<List<QuarterlyRevenueDto>> GetQuarterlyRevenueAsync(int storeId, int year);

        Task<List<TransactionsTableRow>> GetTransactionsAsync(int storeId, TransactionsFilter filter);
    }
}





