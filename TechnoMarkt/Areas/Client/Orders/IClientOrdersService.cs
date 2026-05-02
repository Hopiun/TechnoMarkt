using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Areas.Client.Orders.ViewModels;

namespace TechnoMarkt.Areas.Client.Orders
{
    public interface IClientOrdersService : IOrdersReadService<ClientOrdersFilter>
    {
        Task<(bool Succeeded, string NotificationText)> CancelOrderAsync(int orderId, int clientId);
    }
}
