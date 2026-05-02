using TechnoMarkt.Models;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Orders.Services
{
    public interface IOrdersEditService
    {
        Task<(bool Succeeded, string NotificationText)> AdvanceOrderStatusAsync(int orderId, int operatorId);
        Task<(bool Succeeded, string NotificationText)> CompleteOrderAsync(int orderId, int operatorId, PayMethod method, int? paymentMethodId = null);
        Task<(bool Succeeded, string NotificationText)> ReturnOrderAsync(int orderId, int operatorId);
        Task<(bool Succeeded, string NotificationText)> CancelOrderAsync(int orderId);
    }
}
