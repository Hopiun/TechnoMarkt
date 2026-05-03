using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Shared.Stock.ViewModels.Kpi;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;

namespace TechnoMarkt.Areas.Manager.Warehouse
{
    public class ManagerStockService : IManagerStockService
    {
        private readonly IStockReadService _stockReadService;
        private readonly IStockStatsService _stockStatsService;
        private readonly IStockEditService _stockEditService;

        public ManagerStockService(IStockReadService stockReadService, IStockStatsService stockStatsService, IStockEditService stockEditService)
        {
            _stockReadService = stockReadService;
            _stockStatsService = stockStatsService;
            _stockEditService = stockEditService;
        }

        public Task<List<StockItemsTableRow>> GetStockItemsAsync(int storeId, StockFilter? filter) =>
            _stockReadService.GetStockItemsAsync(storeId, filter);

        public Task<WarehouseKpiDto> GetKpiAsync(int storeId, WarehouseKpiFilter filter) =>
            _stockStatsService.GetKpiAsync(storeId, filter);

        public Task<(bool Succeeded, string? NotificationText)> RestockItemAsync(int warehouseId, int storeId, int addedQuantity) =>
            _stockEditService.RestockItemAsync(warehouseId, storeId, addedQuantity);

        public Task<(bool Succeeded, string? NotificationText)> AdjustInventoryAsync(int warehouseId, int storeId, int exactQuantity) =>
            _stockEditService.AdjustInventoryAsync(warehouseId, storeId, exactQuantity);
    }
}
