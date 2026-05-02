using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Utilities;

namespace TechnoMarkt.Shared.Orders.Extensions
{
    public static class OrderLineQueryExtensions
    {
        public static IQueryable<OrderLine> ByStore(this IQueryable<OrderLine> query, int storeId) =>
            query.Where(orderLine => orderLine.Order.StoreId == storeId);

        public static IQueryable<OrderLine> InPeriod(this IQueryable<OrderLine> query, DateRange range) =>
            query.Where(orderLine => orderLine.Order.OrderDate >= range.From && orderLine.Order.OrderDate <= range.To);

        public static IQueryable<OrderLine> Completed(this IQueryable<OrderLine> query) =>
            query.Where(orderLine => orderLine.Order.Status == OrderStatus.Completed);
    }
}

