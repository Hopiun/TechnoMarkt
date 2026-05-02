using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Models;

namespace TechnoMarkt.Extensions.Query
{
    public static class CategoryQueryExtensions
    {
        public static IQueryable<Category> ParentCategories(this IQueryable<Category> query)
            => query.Where(category => category.ParentCategoryId == null);
    }
}