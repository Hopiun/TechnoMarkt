using TechnoMarkt.Shared.Cart.ViewModels;

namespace TechnoMarkt.Controllers
{
    public interface ICartFacadeService
    {
        int GetTotalItemsCount();
        Task<CartVM> GetCartAsync();
        Task<(bool Succeeded, string? Error)> AddAsync(AddToCartVM form, string userName);
        Task UpdateAsync(int itemId, int quantity, string userName);
        Task RemoveAsync(int itemId, string userName);
        Task<int> ClearAsync(string userName);
    }
}
