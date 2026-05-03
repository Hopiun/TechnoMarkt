using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Utilities;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Shared.Orders.Extensions
{
    public static class OrderQueryExtensions
    {
        public static IQueryable<Order> ByStore(this IQueryable<Order> query, int storeId) =>
            query.Where(order => order.StoreId == storeId);

        public static IQueryable<Order> InPeriod(this IQueryable<Order> query, DateRange range) =>
            query.Where(order => order.OrderDate >= range.From && order.OrderDate <= range.To);

        public static IQueryable<Order> WithStatus(this IQueryable<Order> query, OrderStatus status) =>
            query.Where(order => order.Status == status);

        public static IQueryable<Order> Completed(this IQueryable<Order> query) =>
            query.WithStatus(OrderStatus.Completed);

        public static IQueryable<Order> IncludeOrderLines(this IQueryable<Order> query) =>
            query.Include(order => order.OrderLines);
    }
}

