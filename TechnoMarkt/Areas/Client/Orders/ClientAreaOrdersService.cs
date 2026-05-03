using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Areas.Client.Orders.ViewModels;

namespace TechnoMarkt.Areas.Client.Orders
{
    public class ClientAreaOrdersService : IClientOrdersService
    {
        private readonly ClientOrdersService _clientOrdersService;

        public ClientAreaOrdersService(ClientOrdersService clientOrdersService)
        {
            _clientOrdersService = clientOrdersService;
        }

        public Task<List<OrdersTableRow>> GetOrdersAsync(int id, ClientOrdersFilter? filter) =>
            _clientOrdersService.GetOrdersAsync(id, filter);

        public Task<(bool Succeeded, string NotificationText)> CancelOrderAsync(int orderId, int clientId) =>
            _clientOrdersService.CancelOrderAsync(orderId, clientId);
    }
}
