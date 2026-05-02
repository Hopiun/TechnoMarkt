using System.Text.Json;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Cart.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private const string SessionKeyName = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext!;

        private List<CartItemVM> GetCartItems()
        {
            var cartJson = HttpContext.Session.GetString(SessionKeyName);
            return cartJson != null 
                ? JsonSerializer.Deserialize<List<CartItemVM>>(cartJson) ?? new List<CartItemVM>()
                : new List<CartItemVM>();
        }

        private void SaveCart(List<CartItemVM> items)
        {
            var cartJson = JsonSerializer.Serialize(items);
            HttpContext.Session.SetString(SessionKeyName, cartJson);
        }

        public async Task<CartVM> GetCartAsync()
        {
            var cartItems = GetCartItems();
            var viewModel = new CartVM { Items = cartItems };

            var itemIds = cartItems.Select(i => i.ItemId).ToList();
            if (itemIds.Any())
            {
                var itemsFromDb = await _context.Items
                    .Include(i => i.Discounts)
                    .Include(i => i.Category)
                        .ThenInclude(c => c.Discounts)
                    .Where(i => itemIds.Contains(i.ItemId))
                    .ToListAsync();

                var dateNow = DateOnly.FromDateTime(DateTime.Now);

                foreach (var cartItem in viewModel.Items)
                {
                    var dbItem = itemsFromDb.FirstOrDefault(i => i.ItemId == cartItem.ItemId);
                    if (dbItem != null)
                    {
                        cartItem.Name = dbItem.Name;
                        cartItem.OriginalPrice = dbItem.Price;

                        var activeDiscount = dbItem.Discounts
                            .FirstOrDefault(d =>
                                d.ItemId == cartItem.ItemId &&
                                d.DateFrom <= dateNow && d.DateTo >= dateNow);

                        if (activeDiscount == null && dbItem.Category?.Discounts != null)
                        {
                            activeDiscount = dbItem.Category.Discounts
                                .FirstOrDefault(d =>
                                    d.CategoryId == dbItem.CategoryId &&
                                    d.DateFrom <= dateNow && d.DateTo >= dateNow);
                        }

                        if (activeDiscount == null)
                        {
                            activeDiscount = dbItem.Discounts
                                .FirstOrDefault(d =>
                                    d.CategoryId == dbItem.CategoryId &&
                                    d.DateFrom <= dateNow && d.DateTo >= dateNow);
                        }

                        cartItem.DiscountPercent = activeDiscount?.Percent ?? 0;
                    }
                }

                SaveCart(viewModel.Items);
            }

            return viewModel;
        }

        public async Task AddToCartAsync(int itemId, int quantity = 1)
        {
            var cart = await GetCartAsync();
            var existingItem = cart.Items.FirstOrDefault(i => i.ItemId == itemId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var item = await _context.Items
                    .Include(i => i.Discounts)
                    .FirstOrDefaultAsync(i => i.ItemId == itemId);

                if (item != null)
                {
                    var dateNow = DateOnly.FromDateTime(DateTime.Now);
                    var activeDiscount = item.Discounts
                        .FirstOrDefault(d => 
                            ((d.ItemId == itemId || d.CategoryId == item.CategoryId) && 
                             d.DateFrom <= dateNow && d.DateTo >= dateNow));

                    cart.Items.Add(new CartItemVM
                    {
                        ItemId = itemId,
                        Name = item.Name,
                        OriginalPrice = item.Price,
                        DiscountPercent = activeDiscount?.Percent ?? 0,
                        Quantity = quantity
                    });
                }
            }

            SaveCart(cart.Items);
        }

        public async Task UpdateQuantityAsync(int itemId, int quantity)
        {
            if (quantity <= 0)
            {
                await RemoveFromCartAsync(itemId);
                return;
            }

            var cart = await GetCartAsync();
            var item = cart.Items.FirstOrDefault(i => i.ItemId == itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                SaveCart(cart.Items);
            }
        }

        public async Task RemoveFromCartAsync(int itemId)
        {
            var cart = await GetCartAsync();
            cart.Items.RemoveAll(i => i.ItemId == itemId);
            SaveCart(cart.Items);
        }

        public async Task ClearCartAsync()
        {
            SaveCart(new List<CartItemVM>());
        }

        public int GetTotalItemsCount()
        {
            var cart = GetCartItems();
            return cart.Sum(i => i.Quantity);
        }
    }
}

