using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Interfaces
{
    public interface ICategoriesReadService
    {
        public Task<List<CategoryItem>> GetCategoriesAsync();
        public Task<SelectedCategoryItem> GetCategoryAsync(int? categoryId);
    }
}
