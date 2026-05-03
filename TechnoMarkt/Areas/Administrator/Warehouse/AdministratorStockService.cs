using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Shared.Stock.ViewModels.Kpi;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Warehouse
{
    public class AdministratorStockService : IAdministratorStockService
    {
        private readonly IStockReadService _stockReadService;
        private readonly IStockStatsService _stockStatsService;

        public AdministratorStockService(IStockReadService stockReadService, IStockStatsService stockStatsService)
        {
            _stockReadService = stockReadService;
            _stockStatsService = stockStatsService;
        }

        public Task<List<StockItemsTableRow>> GetStockItemsAsync(int storeId, StockFilter? filter) =>
            _stockReadService.GetStockItemsAsync(storeId, filter);

        public Task<WarehouseKpiDto> GetKpiAsync(int storeId, WarehouseKpiFilter filter) =>
            _stockStatsService.GetKpiAsync(storeId, filter);
    }
}
