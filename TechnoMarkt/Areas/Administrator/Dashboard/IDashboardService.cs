using TechnoMarkt.Areas.Administrator.Dashboard.ViewModels;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Dashboard
{
    public interface IDashboardService
    {
        public Task<DashboardKpiDto> GetKpiAsync(int storeId);
        public Task<Dictionary<string, decimal>> GetSalesByCategoryAsync(int storeId);
        public Task<List<TransactionsTableRow>> GetRecentTransactionsAsync(int storeId);
        public Task<Dictionary<DateOnly, decimal>> GetDailySalesAsync(int storeId);
    }
}





