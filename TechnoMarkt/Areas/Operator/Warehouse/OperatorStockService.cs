using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;

namespace TechnoMarkt.Areas.Operator.Warehouse
{
    public class OperatorStockService : IOperatorStockService
    {
        private readonly IStockReadService _stockReadService;

        public OperatorStockService(IStockReadService stockReadService)
        {
            _stockReadService = stockReadService;
        }

        public Task<List<StockItemsTableRow>> GetStockItemsAsync(int storeId, StockFilter? filter) =>
            _stockReadService.GetStockItemsAsync(storeId, filter);
    }
}
