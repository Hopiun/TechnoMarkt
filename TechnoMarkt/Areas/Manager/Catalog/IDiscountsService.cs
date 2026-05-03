using TechnoMarkt.Areas.Manager.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    public interface IDiscountsService
    {
        public Task<DiscountFormVM?> GetDiscountFormAsync(int? itemId, int? categoryId);
        public Task<(bool Succeeded, string NotificationMessage)> AssignDiscountAsync(DiscountFormVM newDiscount);
        public Task<(bool Succeeded, string NotificationMessage)> UpdateDiscountAsync(DiscountFormVM updatedDiscount);
        public Task<(bool Succeeded, string NotificationMessage)> RevokeDiscountAsync(int? itemId, int? categoryId);
    }
}



