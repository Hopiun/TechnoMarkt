using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    public interface IManagerCategoriesService
    {
        public Task<List<CategorySalesDto>> GetTopCategoriesAsync(int storeId, MonthYearFilter filter);
        public Task<CategoryFormVM?> GetCategoryFormAsync(int? categoryId);
        public Task<(bool Succeeded, string NotificationMessage)> AddCategoryAsync(CategoryFormVM newCategory);
        public Task<(bool Succeeded, string NotificationMessage)> UpdateCategoryAsync(CategoryFormVM updatedCategory);
        public Task<(bool Succeeded, string NotificationMessage)> DeleteCategoryAsync(int categoryId);
    }
}




