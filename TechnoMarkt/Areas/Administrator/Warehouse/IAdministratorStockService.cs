using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Administrator.Warehouse
{
    public interface IAdministratorStockService : IStockReadService, IStockStatsService
    {
    }
}
