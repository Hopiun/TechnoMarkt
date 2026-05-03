using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    public interface IManagerItemsService : IItemsReadService<ManagerItemsFilter>
    {
        public Task<List<ItemRowDto>> GetTopItemsAsync(int storeId, MonthYearFilter filter);
        public Task<ItemFormVM?> GetItemFormAsync(int? itemId);
        public Task<(bool Succeeded, string NotificationMessage)> AddItemAsync(ItemFormVM newItem, int storeId);
        public Task<(bool Succeeded, string NotificationMessage)> UpdateItemAsync(ItemFormVM updatedItem);
        public Task<(bool Succeeded, string NotificationMessage)> DeleteItemAsync(int itemId);
    }
}





