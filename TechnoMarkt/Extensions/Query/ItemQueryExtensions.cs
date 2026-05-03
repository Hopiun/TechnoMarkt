using TechnoMarkt.Models;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Extensions.Query
{
    public static class ItemQueryExtensions
    {
        public static IQueryable<Item> ByCategory(this IQueryable<Item> query, int? categoryId)
            => categoryId.HasValue
                ? query.Where(item => item.CategoryId == categoryId.Value || item.Category.ParentCategoryId == categoryId.Value)
                : query;

        public static IQueryable<Item> SearchByName(this IQueryable<Item> query, string? name)
            => !string.IsNullOrWhiteSpace(name) ? query.Where(item => item.Name.ToLower().Contains(name.ToLower()))
                : query;

        public static IQueryable<Item> InPriceRange(this IQueryable<Item> query, (decimal?, decimal?) priceRange)
        {
            var (min, max) = priceRange;

            bool bothHaveValues = min.HasValue && max.HasValue;

            if (!bothHaveValues)
                return query;

            if (bothHaveValues && min.Value > max.Value)
                (min, max) = (max, min);

            if (min.HasValue && min.Value >= 0)
                query = query.Where(item => item.Price >= min.Value);

            if (max.HasValue && max.Value >= 0)
                query = query.Where(item => item.Price <= max.Value);

            return query;
        }

        public static IQueryable<Item> SortByAscending(this IQueryable<Item> query, ItemsSortBy? sortBy)
        {
            if (!sortBy.HasValue) return query;

            return sortBy switch
            {
                ItemsSortBy.Name => query.OrderBy(item => item.Name),
                ItemsSortBy.Price => query.OrderBy(item => item.Price),
                _ => query.OrderBy(item => item.ItemId)
            };
        }

        public static IQueryable<Item> SortByDescending(this IQueryable<Item> query, ItemsSortBy? sortBy)
        {
            if (!sortBy.HasValue) return query;

            return sortBy switch
            {
                ItemsSortBy.Name => query.OrderByDescending(item => item.Name),
                ItemsSortBy.Price => query.OrderByDescending(item => item.Price),
                _ => query.OrderBy(item => item.ItemId)
            };
        }
    }
}
