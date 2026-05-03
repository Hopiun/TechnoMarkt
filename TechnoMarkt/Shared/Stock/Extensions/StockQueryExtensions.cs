using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Models;

namespace TechnoMarkt.Shared.Stock.Extensions
{
    public static class StockQueryExtensions
    {
        public static IQueryable<Warehouse> ByStore(this IQueryable<Warehouse> query, int storeId) =>
            query.Where(warehouse => warehouse.StoreId == storeId);

        public static IQueryable<Warehouse> LowStock(this IQueryable<Warehouse> query, int criticalThreshold = 5) =>
            query.Where(warehouse => warehouse.Quantity <= criticalThreshold);

        public static IQueryable<Warehouse> OutOfStock(this IQueryable<Warehouse> query) =>
            query.Where(warehouse => warehouse.Quantity == 0);

        public static IQueryable<Warehouse> InStock(this IQueryable<Warehouse> query) =>
            query.Where(warehouse => warehouse.Quantity > 0);
    }
}

