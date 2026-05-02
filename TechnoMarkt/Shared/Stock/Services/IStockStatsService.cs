using TechnoMarkt.Shared.Stock.ViewModels.Kpi;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Stock.Services
{
    public interface IStockStatsService
    {
        Task<WarehouseKpiDto> GetKpiAsync(int storeId, WarehouseKpiFilter filter);
    }
}
