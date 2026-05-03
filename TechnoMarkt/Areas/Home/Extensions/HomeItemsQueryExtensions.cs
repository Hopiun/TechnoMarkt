using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Home.Extensions
{
    public static class HomeItemsQueryExtensions
    {
        public static IQueryable<Item> ByBrands(this IQueryable<Item> query, List<int>? brandIds)
            => brandIds != null && brandIds.Any() ? query.Where(item => brandIds.Contains(item.BrandId)) : query;

        public static IQueryable<Item> ByMinPrice(this IQueryable<Item> query, decimal? minPrice)
            => minPrice.HasValue ? query.Where(item => item.Price >= minPrice.Value) : query;

        public static IQueryable<Item> ByMaxPrice(this IQueryable<Item> query, decimal? maxPrice)
            => maxPrice.HasValue ? query.Where(item => item.Price <= maxPrice.Value) : query;

        public static IQueryable<Item> InStock(this IQueryable<Item> query, int storeId)
            => query.Where(item => item.Warehouses.Any(w => w.StoreId == storeId && w.Quantity > 0));

        public static IQueryable<Item> WithActiveDiscounts(this IQueryable<Item> query)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            return query.Where(item => item.Discounts.Any(d => d.ItemId == item.ItemId && d.DateFrom <= today && d.DateTo >= today)
                                    || item.Discounts.Any(d => d.CategoryId == item.CategoryId && d.DateFrom <= today && d.DateTo >= today));
        }
    }
}

