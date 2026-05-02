using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Orders.Services
{
    public class OrdersReadService : IOrdersReadService<OrdersFilter>
    {
        private readonly AppDbContext _context;

        public OrdersReadService(AppDbContext context) => _context = context;

        public async Task<List<OrdersTableRow>> GetOrdersAsync(int storeId, OrdersFilter? filter)
        {
            IQueryable<Order> query = _context.Orders.ByStore(storeId);

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Search))
                    query = query.Where(order => order.Client.FirstName.Contains(filter.Search)
                                          || order.Client.LastName.Contains(filter.Search));

                if (filter.Status.HasValue)
                    query = query.WithStatus(filter.Status.Value);

                if (filter.DateFrom.HasValue)
                    query = query.Where(order => order.OrderDate >= filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue));

                if (filter.DateTo.HasValue)
                    query = query.Where(order => order.OrderDate <= filter.DateTo.Value.ToDateTime(TimeOnly.MaxValue));

                if (filter.PaidOnly)
                    query = query.Where(order => order.Payments.Any(payment => payment.Status == PaymentStatus.Completed));

                if (filter.UnpaidOnly)
                    query = query.Where(order => order.Payments.Any(payment => payment.Status != PaymentStatus.Completed));
            }

            return await query
                .OrderByDescending(order => order.OrderDate)
                .Select(order => new OrdersTableRow()
                {
                    OrderId = order.OrderId,
                    ClientId = order.ClientId,
                    ClientFullName = $"{order.Client.FirstName} {order.Client.LastName}",
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    OrderStatus = order.Status,
                    OrderTotal = order.OrderTotal,
                    PaymentStatus = order.Payments
                        .OrderByDescending(payment => payment.Date)
                        .Select(payment => (PaymentStatus?)payment.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}
