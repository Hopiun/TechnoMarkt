using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Areas.Client.Orders.ViewModels;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Shared.Orders.Services;

using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Client.Orders
{
    public class ClientOrdersService : IOrdersReadService<ClientOrdersFilter>
    {
        private readonly AppDbContext _context;

        public ClientOrdersService(AppDbContext context) => _context = context;

        public async Task<List<OrdersTableRow>> GetOrdersAsync(int clientId, ClientOrdersFilter? filter)
        {
            IQueryable<Order> query = _context.Orders.Where(o => o.ClientId == clientId);

            if (filter != null)
            {
                if (filter.Status.HasValue)
                    query = query.WithStatus(filter.Status.Value);

                if (filter.DateFrom.HasValue)
                    query = query.Where(o => o.OrderDate >= filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue));

                if (filter.DateTo.HasValue)
                    query = query.Where(o => o.OrderDate <= filter.DateTo.Value.ToDateTime(TimeOnly.MaxValue));
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrdersTableRow()
                {
                    OrderId = o.OrderId,
                    ClientId = o.ClientId,
                    ClientFullName = $"{o.Client.FirstName} {o.Client.LastName}",
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate,
                    OrderStatus = o.Status,
                    OrderTotal = o.OrderTotal,
                    PaymentStatus = o.Payments
                        .OrderByDescending(p => p.Date)
                        .Select(p => (PaymentStatus?)p.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<(bool Succeeded, string NotificationText)> CancelOrderAsync(int orderId, int clientId)
        {
            Order? order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.ClientId == clientId);

            if (order == null)
                return (false, $"Р—Р°РјРѕРІР»РµРЅРЅСЏ #{orderId} РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            if (order.Status == OrderStatus.Completed
             || order.Status == OrderStatus.Returned
             || order.Status == OrderStatus.Cancelled)
                return (false, $"Р—Р°РјРѕРІР»РµРЅРЅСЏ #{orderId} РЅРµ РјРѕР¶Рµ Р±СѓС‚Рё СЃРєР°СЃРѕРІР°РЅРѕ Р·С– СЃС‚Р°С‚СѓСЃСѓ '{order.Status}'");

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return (true, $"Р—Р°РјРѕРІР»РµРЅРЅСЏ #{orderId} СЃРєР°СЃРѕРІР°РЅРѕ");
        }
    }
}







