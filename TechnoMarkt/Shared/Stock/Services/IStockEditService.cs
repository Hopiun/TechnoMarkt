using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Stock.Services
{
    public interface IStockEditService
    {
        Task<(bool Succeeded, string? NotificationText)> RestockItemAsync(int warehouseId, int storeId, int addedQuantity);
        Task<(bool Succeeded, string? NotificationText)> AdjustInventoryAsync(int warehouseId, int storeId, int exactQuantity);
    }
}
