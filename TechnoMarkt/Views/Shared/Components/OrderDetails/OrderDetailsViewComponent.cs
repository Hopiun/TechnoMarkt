using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Views.Shared.Components.OrderDetails
{
    public class OrderDetailsViewComponent : ViewComponent
    {
        private AppDbContext _context;

        public OrderDetailsViewComponent(AppDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync(int orderId, string role = "")
        {
            Order? order = await _context.Orders
                .Include(order => order.Payments)
                .Include(order => order.Client)
                    .ThenInclude(client => client.PaymentMethods)
                .FirstOrDefaultAsync(order => order.OrderId == orderId);

            if (order == null)
                return View(new OrderDetailsViewModel { Role = role });

            var completedPayment = order.Payments
                .FirstOrDefault(payment => payment.Status == PaymentStatus.Completed);

            bool canBeReturned = order.Status == OrderStatus.Completed
                && completedPayment != null
                && (DateTime.Now - completedPayment.Date).TotalDays <= 14;

            return View(new OrderDetailsViewModel()
            {
                OrderId = orderId,
                OrderStatus = order.Status,
                OrderTotal = order.OrderTotal,
                IsPaid = completedPayment != null,
                CanBeReturned = canBeReturned,
                Role = role,
                Rows = await GetOrderLinesAsync(orderId),


                ClientPaymentMethods = role == "Operator" ? order.Client.PaymentMethods
                        .Select(pm => new PaymentMethodRow()
                        {
                            PaymentMethodId = pm.PaymentMethodId,
                            BankName = pm.BankName,
                            CardType = pm.CardType,
                            MaskedCardNumber = $"**** **** **** {pm.CardNumber[^4..]}"
                        })
                        .ToList() : []
            });
        }

        private async Task<List<OrderLineRow>> GetOrderLinesAsync(int orderId) =>
            await _context.OrderLines
                .Where(orderLine => orderLine.OrderId == orderId)
                .Select(orderLine => new OrderLineRow
                {
                    ItemId = orderLine.ItemId,
                    ImageUrl = orderLine.Item.ImageUrl,
                    ItemName = orderLine.Item.Name,
                    Category = orderLine.Item.Category.Name,
                    Quantity = orderLine.Quantity,
                    PriceAtMoment = orderLine.PriceAtMoment,
                    DiscountPercent = orderLine.Item.Discounts
                        .Where(d => d.DateFrom <= DateOnly.FromDateTime(orderLine.Order.OrderDate)
                                 && d.DateTo >= DateOnly.FromDateTime(orderLine.Order.OrderDate))
                        .Select(d => (int?)d.Percent)
                        .FirstOrDefault()
                })
                .ToListAsync();
    }
}


