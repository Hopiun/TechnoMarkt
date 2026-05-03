using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Utilities;

namespace TechnoMarkt.Extensions.Query
{
    public static class DiscountQueryExtensions
    {
        public static Task<Discount?> WithCategoryAsync(this IQueryable<Discount> query, int categoryId)
            => query.FirstOrDefaultAsync(discount => discount.CategoryId == categoryId);

        public static IQueryable<Discount> InPeriod(this IQueryable<Discount> query, DateRange period)
            => query.Where(discount => discount.DateFrom <= DateOnly.FromDateTime(period.From)
                                       && discount.DateTo >= DateOnly.FromDateTime(period.To));
    }
}