using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Stock.Services
{
    public interface IStockReadService
    {
        Task<List<StockItemsTableRow>> GetStockItemsAsync(int storeId, StockFilter? filter);
    }
}
