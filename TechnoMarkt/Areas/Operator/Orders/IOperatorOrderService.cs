using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;

namespace TechnoMarkt.Areas.Operator.Orders
{
    public interface IOperatorOrderService : IOrdersReadService<OrdersFilter>, IOrdersStatsService, IOrdersEditService
    {
    }
}
