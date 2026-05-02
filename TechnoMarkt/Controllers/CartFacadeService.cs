using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Cart.ViewModels;
using TechnoMarkt.Services;

namespace TechnoMarkt.Controllers
{
    public class CartFacadeService : ICartFacadeService
    {
        private readonly CartService _cartService;
        private readonly AppDbContext _context;
        private readonly IEventLogsService _eventLogsService;

        public CartFacadeService(CartService cartService, AppDbContext context, IEventLogsService eventLogsService)
        {
            _cartService = cartService;
            _context = context;
            _eventLogsService = eventLogsService;
        }

        public int GetTotalItemsCount() => _cartService.GetTotalItemsCount();
        public Task<CartVM> GetCartAsync() => _cartService.GetCartAsync();

        public async Task<(bool Succeeded, string? Error)> AddAsync(AddToCartVM form, string userName)
        {
            var item = await _context.Items.FindAsync(form.ItemId);
            var itemName = item?.Name ?? $"товар #{form.ItemId}";
            await _cartService.AddToCartAsync(form.ItemId, form.Quantity);
            await _eventLogsService.LogAsync("Create", "Cart", form.ItemId, $"{userName} додав до кошика '{itemName}', {form.Quantity} шт.");
            return (true, null);
        }

        public async Task UpdateAsync(int itemId, int quantity, string userName)
        {
            var item = await _context.Items.FindAsync(itemId);
            var itemName = item?.Name ?? $"товар #{itemId}";
            await _cartService.UpdateQuantityAsync(itemId, quantity);
            await _eventLogsService.LogAsync("Update", "Cart", itemId, $"{userName} змінив кількість '{itemName}' у кошику на {quantity} шт.");
        }

        public async Task RemoveAsync(int itemId, string userName)
        {
            var item = await _context.Items.FindAsync(itemId);
            var itemName = item?.Name ?? $"товар #{itemId}";
            await _cartService.RemoveFromCartAsync(itemId);
            await _eventLogsService.LogAsync("Delete", "Cart", itemId, $"{userName} видалив '{itemName}' з кошика");
        }

        public async Task<int> ClearAsync(string userName)
        {
            var cart = await _cartService.GetCartAsync();
            var itemCount = cart.TotalItems;
            await _cartService.ClearCartAsync();
            await _eventLogsService.LogAsync("Delete", "Cart", null, $"{userName} очистив кошик (видалено {itemCount} товарів)");
            return itemCount;
        }
    }
}
