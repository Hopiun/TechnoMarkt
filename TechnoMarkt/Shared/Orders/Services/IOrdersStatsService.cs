using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Kpi;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Orders.Services
{
    public interface IOrdersStatsService
    {
        Task<OrdersKpiDto> GetKpiAsync(int storeId);
        Task<Dictionary<OrderStatus, int>> GetOrdersCountByStatusAsync(int storeId);
        Task<List<OrdersTableRow>> GetTopOrdersAsync(int storeId, MonthYearFilter filter);
    }
}
