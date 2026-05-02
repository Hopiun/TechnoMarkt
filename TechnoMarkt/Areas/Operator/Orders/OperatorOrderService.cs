using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Shared.Orders.ViewModels.Kpi;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;

namespace TechnoMarkt.Areas.Operator.Orders
{
    public class OperatorOrderService : IOperatorOrderService
    {
        private readonly IOrdersReadService<OrdersFilter> _ordersReadService;
        private readonly IOrdersStatsService _ordersStatsService;
        private readonly IOrdersEditService _ordersEditService;

        public OperatorOrderService(
            IOrdersReadService<OrdersFilter> ordersReadService,
            IOrdersStatsService ordersStatsService,
            IOrdersEditService ordersEditService)
        {
            _ordersReadService = ordersReadService;
            _ordersStatsService = ordersStatsService;
            _ordersEditService = ordersEditService;
        }

        public Task<List<OrdersTableRow>> GetOrdersAsync(int id, OrdersFilter? filter) =>
            _ordersReadService.GetOrdersAsync(id, filter);

        public Task<OrdersKpiDto> GetKpiAsync(int storeId) =>
            _ordersStatsService.GetKpiAsync(storeId);

        public Task<Dictionary<OrderStatus, int>> GetOrdersCountByStatusAsync(int storeId) =>
            _ordersStatsService.GetOrdersCountByStatusAsync(storeId);

        public Task<List<OrdersTableRow>> GetTopOrdersAsync(int storeId, MonthYearFilter filter) =>
            _ordersStatsService.GetTopOrdersAsync(storeId, filter);

        public Task<(bool Succeeded, string NotificationText)> AdvanceOrderStatusAsync(int orderId, int operatorId) =>
            _ordersEditService.AdvanceOrderStatusAsync(orderId, operatorId);

        public Task<(bool Succeeded, string NotificationText)> CompleteOrderAsync(int orderId, int operatorId, PayMethod method, int? paymentMethodId = null) =>
            _ordersEditService.CompleteOrderAsync(orderId, operatorId, method, paymentMethodId);

        public Task<(bool Succeeded, string NotificationText)> ReturnOrderAsync(int orderId, int operatorId) =>
            _ordersEditService.ReturnOrderAsync(orderId, operatorId);

        public Task<(bool Succeeded, string NotificationText)> CancelOrderAsync(int orderId) =>
            _ordersEditService.CancelOrderAsync(orderId);
    }
}
