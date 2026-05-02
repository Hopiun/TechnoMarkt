using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Orders
{
    public interface IAdministratorOrderService : IOrdersReadService<OrdersFilter>, IOrdersStatsService
    {
    }
}
