using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Cart.ViewModels;
using TechnoMarkt.Services;
using ClientModel = TechnoMarkt.Models.Client;

namespace TechnoMarkt.Areas.Client.Checkout
{
    public class ClientCheckoutService : IClientCheckoutService
    {
        private readonly CartService _cartService;
        private readonly AppDbContext _context;
        private readonly IEventLogsService _eventLogsService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientCheckoutService(
            CartService cartService,
            AppDbContext context,
            IEventLogsService eventLogsService,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _context = context;
            _eventLogsService = eventLogsService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool HasItems, CartVM Cart)> GetCheckoutCartAsync()
        {
            CartVM cart = await _cartService.GetCartAsync();
            return (cart.Items.Any(), cart);
        }

        public async Task<CheckoutProcessResult> ProcessOrderAsync(string? userId, CheckoutVM form)
        {
            CartVM cart = await _cartService.GetCartAsync();
            if (!cart.Items.Any())
                return new CheckoutProcessResult(false, true, "Корзина пуста", null, null);

            ClientModel? client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId.ToString() == userId);
            if (client == null)
                return new CheckoutProcessResult(false, true, "Помилка: клієнт не знайдено", null, null);

            int storeId = 1;
            string? storeIdCookie = _httpContextAccessor.HttpContext?.Request.Cookies["StoreId"];
            if (!string.IsNullOrWhiteSpace(storeIdCookie) && int.TryParse(storeIdCookie, out int parsedStoreId))
                storeId = parsedStoreId;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Store? store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == storeId);
                if (store == null)
                    return new CheckoutProcessResult(false, true, "Помилка: магазини не знайдено", null, null);

                Order order = new()
                {
                    ClientId = client.ClientId,
                    StoreId = store.StoreId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.New,
                    OrderTotal = cart.TotalAmount
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var cartItem in cart.Items)
                {
                    _context.OrderLines.Add(new OrderLine
                    {
                        OrderId = order.OrderId,
                        ItemId = cartItem.ItemId,
                        Quantity = cartItem.Quantity,
                        PriceAtMoment = cartItem.DiscountedPrice
                    });
                }

                Payment payment = new()
                {
                    OrderId = order.OrderId,
                    Amount = cart.TotalAmount,
                    Date = DateTime.Now,
                    Method = form.PaymentMethod,
                    Status = PaymentStatus.Pending
                };
                _context.Payments.Add(payment);

                if (form.PaymentMethod == PayMethod.Online)
                {
                    if (client.WalletBalance < cart.TotalAmount)
                    {
                        await transaction.RollbackAsync();
                        return new CheckoutProcessResult(false, false, "Недостатньо коштів на бонусному рахунку", cart, null);
                    }

                    client.WalletBalance -= cart.TotalAmount;
                    var itemIds = cart.Items.Select(i => i.ItemId).ToList();
                    var warehouses = await _context.Warehouses
                        .Where(w => w.StoreId == order.StoreId && itemIds.Contains(w.ItemId))
                        .ToDictionaryAsync(w => w.ItemId);

                    foreach (var cartLine in cart.Items)
                    {
                        if (!warehouses.TryGetValue(cartLine.ItemId, out var warehouse))
                        {
                            await transaction.RollbackAsync();
                            return new CheckoutProcessResult(false, false, $"Товар '{cartLine.Name}' не знайдено на складі магазину", cart, null);
                        }

                        if (warehouse.Quantity < cartLine.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return new CheckoutProcessResult(false, false, $"Недостатня кількість товару '{cartLine.Name}' на складі", cart, null);
                        }

                        warehouse.Quantity -= cartLine.Quantity;
                        _context.Entry(warehouse).Property(w => w.Quantity).IsModified = true;
                        _context.Entry(warehouse).State = EntityState.Modified;
                    }

                    payment.Status = PaymentStatus.Completed;
                    payment.Date = DateTime.Now;
                    order.Status = OrderStatus.Completed;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                await _cartService.ClearCartAsync();

                string paymentMethodDesc = form.PaymentMethod == PayMethod.Online ? "онлайн (бонусний рахунок)" : "у відділенні";
                await _eventLogsService.LogAsync("Create", "Order", order.OrderId, $"Оформлено замовлення #{order.OrderId} на суму {cart.TotalAmount:N0} ({paymentMethodDesc})");

                return new CheckoutProcessResult(true, false, $"Замовлення #{order.OrderId} успішно оформлено!", null, order.OrderId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new CheckoutProcessResult(false, false, $"Помилка при оформленні замовлення: {ex.Message}", cart, null);
            }
        }
    }
}
