using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Manager.Warehouse
{
    public interface IManagerStockService : IStockReadService, IStockStatsService, IStockEditService
    {
    }
}
