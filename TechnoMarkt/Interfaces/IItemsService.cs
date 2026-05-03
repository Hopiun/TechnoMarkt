using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Interfaces
{
    public interface IItemsReadService<TFilter>
    {
        public Task<(List<ItemCardVM> Items, int TotalCount)> GetItemsAsync(int? categoryId, int storeId, TFilter? filter);
        public Task<ItemVM> GetItemDetailsAsync(int itemId, int storeId);
    }
}

