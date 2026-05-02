using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Shared.Orders.ViewModels.Kpi;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Orders
{
    public class AdministratorOrderService : IAdministratorOrderService
    {
        private readonly IOrdersReadService<OrdersFilter> _ordersReadService;
        private readonly IOrdersStatsService _ordersStatsService;

        public AdministratorOrderService(IOrdersReadService<OrdersFilter> ordersReadService, IOrdersStatsService ordersStatsService)
        {
            _ordersReadService = ordersReadService;
            _ordersStatsService = ordersStatsService;
        }

        public Task<List<OrdersTableRow>> GetOrdersAsync(int id, OrdersFilter? filter) =>
            _ordersReadService.GetOrdersAsync(id, filter);

        public Task<OrdersKpiDto> GetKpiAsync(int storeId) =>
            _ordersStatsService.GetKpiAsync(storeId);

        public Task<Dictionary<OrderStatus, int>> GetOrdersCountByStatusAsync(int storeId) =>
            _ordersStatsService.GetOrdersCountByStatusAsync(storeId);

        public Task<List<OrdersTableRow>> GetTopOrdersAsync(int storeId, MonthYearFilter filter) =>
            _ordersStatsService.GetTopOrdersAsync(storeId, filter);
    }
}
