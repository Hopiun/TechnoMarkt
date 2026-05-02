using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Manager.Extensions
{
    public static class ManagerItemQueryExtensions
    {
        public static IQueryable<Item> BySupplier(this IQueryable<Item> query, int? supplierId)
            => supplierId.HasValue ? query.Where(item => item.SupplierId == supplierId) : query;
    }
}