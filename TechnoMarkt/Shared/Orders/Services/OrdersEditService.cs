using TechnoMarkt.Data;
using TechnoMarkt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Orders.Services
{
    public class OrdersEditService : IOrdersEditService
    {
        private readonly AppDbContext _context;

        public OrdersEditService(AppDbContext context) => _context = context;

        public async Task<(bool Succeeded, string NotificationText)> AdvanceOrderStatusAsync(int orderId, int operatorId)
        {
            Order? order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return (false, $"Замовлення #{orderId} не знайдено");

            OrderStatus? nextStatus = order.Status switch
            {
                OrderStatus.New => OrderStatus.Processing,
                OrderStatus.Processing => OrderStatus.Ready,
                _ => null
            };

            if (nextStatus == null)
                return (false, $"Замовлення #{orderId} не може бути просунуто далі зі статусу '{order.Status}'");

            order.Status = nextStatus.Value;
            order.OperatorId = operatorId;

            await _context.SaveChangesAsync();
            return (true, $"Статус замовлення #{orderId} змінено на '{nextStatus}'");
        }

        public async Task<(bool Succeeded, string NotificationText)> CompleteOrderAsync(int orderId, int operatorId, PayMethod method, int? paymentMethodId = null)
        {
            Order? order = await _context.Orders
                .Include(order => order.OrderLines)
                    .ThenInclude(orderLine => orderLine.Item)
                        .ThenInclude(item => item.Warehouses)
                .Include(order => order.Payments)
                .FirstOrDefaultAsync(order => order.OrderId == orderId);

            if (order == null)
                return (false, $"Замовлення #{orderId} не знайдено");

            if (order.Status != OrderStatus.Ready)
                return (false, "Оплата можлива лише для замовлень зі статусом 'Готове до видачі'");

            if (order.Payments.Any(payment => payment.Status == PaymentStatus.Completed))
                return (false, $"Замовлення #{orderId} вже оплачено");

            if (paymentMethodId.HasValue)
            {
                bool methodBelongsToClient = await _context.PaymentMethods
                    .AnyAsync(paymentMethod => paymentMethod.PaymentMethodId == paymentMethodId.Value && paymentMethod.ClientId == order.ClientId);

                if (!methodBelongsToClient)
                    return (false, "Спосіб оплати не належить цьому клієнту");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            Payment? pendingPayment = order.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Pending);

            if (pendingPayment != null)
            {
                pendingPayment.Status = PaymentStatus.Completed;
                pendingPayment.Method = method;
                pendingPayment.Date = DateTime.Now;
                pendingPayment.Amount = order.OrderTotal;
            }
            else
            {
                _context.Payments.Add(new Payment()
                {
                    OrderId = orderId,
                    Amount = order.OrderTotal,
                    Method = method,
                    Status = PaymentStatus.Completed,
                    Date = DateTime.Now
                });
            }

            var itemIds = order.OrderLines.Select(line => line.ItemId).ToList();
            var warehouses = await _context.Warehouses
                .Where(warehouse => warehouse.StoreId == order.StoreId && itemIds.Contains(warehouse.ItemId))
                .ToDictionaryAsync(warehouse => warehouse.ItemId);

            foreach (OrderLine line in order.OrderLines)
            {
                if (!warehouses.TryGetValue(line.ItemId, out var warehouse))
                {
                    await transaction.RollbackAsync();
                    return (false, $"Товар '{line.Item.Name}' не знайдено на складі магазину");
                }

                if (warehouse.Quantity < line.Quantity)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Недостатня кількість товару '{line.Item.Name}' на складі");
                }

                warehouse.Quantity -= line.Quantity;
            }

            order.Status = OrderStatus.Completed;
            order.OperatorId = operatorId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Замовлення #{orderId} оплачено та завершено");
        }

        public async Task<(bool Succeeded, string NotificationText)> ReturnOrderAsync(int orderId, int operatorId)
        {
            Order? order = await _context.Orders
                .Include(order => order.OrderLines)
                    .ThenInclude(orderLine => orderLine.Item)
                        .ThenInclude(item => item.Warehouses)
                .Include(order => order.Payments)
                .FirstOrDefaultAsync(order => order.OrderId == orderId);

            if (order == null)
                return (false, $"Замовлення #{orderId} не знайдено");

            if (order.Status != OrderStatus.Completed)
                return (false, "Повернення можливе лише для завершених замовлень");

            Payment? completedPayment = order.Payments
                .FirstOrDefault(payment => payment.Status == PaymentStatus.Completed);

            if (completedPayment == null)
                return (false, "Оплату для цього замовлення не знайдено");

            if ((DateTime.Now - completedPayment.Date).TotalDays > 14)
                return (false, "Термін повернення (14 днів) вичерпано");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            completedPayment.Status = PaymentStatus.Refunded;
            completedPayment.Date = DateTime.Now;

            var itemIds = order.OrderLines.Select(line => line.ItemId).ToList();
            var warehouses = await _context.Warehouses
                .Where(warehouse => warehouse.StoreId == order.StoreId && itemIds.Contains(warehouse.ItemId))
                .ToDictionaryAsync(warehouse => warehouse.ItemId);

            foreach (OrderLine line in order.OrderLines)
            {
                if (warehouses.TryGetValue(line.ItemId, out var warehouse))
                    warehouse.Quantity += line.Quantity;
            }

            order.Status = OrderStatus.Returned;
            order.OperatorId = operatorId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, $"Повернення замовлення #{orderId} оформлено");
        }

        public async Task<(bool Succeeded, string NotificationText)> CancelOrderAsync(int orderId)
        {
            Order? order = await _context.Orders
                .Include(order => order.Payments)
                .FirstOrDefaultAsync(order => order.OrderId == orderId);

            if (order == null)
                return (false, $"Замовлення #{orderId} не знайдено");

            if (order.Status == OrderStatus.Completed
             || order.Status == OrderStatus.Returned
             || order.Status == OrderStatus.Cancelled)
                return (false, $"Замовлення #{orderId} не може бути скасовано зі статусу '{order.Status}'");

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return (true, $"Замовлення #{orderId} скасовано");
        }
    }
}