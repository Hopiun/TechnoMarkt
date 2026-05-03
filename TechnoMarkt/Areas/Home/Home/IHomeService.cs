using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Home.Home
{
    public interface IHomeService
    {
        public Task<List<CategoryCard>> GetMainCategoriesAsync();
        public Task<List<ItemCardVM>> GetTopItemsAsync();
    }
}



