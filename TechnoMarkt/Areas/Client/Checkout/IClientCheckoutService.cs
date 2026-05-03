using TechnoMarkt.Shared.Cart.ViewModels;

namespace TechnoMarkt.Areas.Client.Checkout
{
    public interface IClientCheckoutService
    {
        Task<(bool HasItems, CartVM Cart)> GetCheckoutCartAsync();
        Task<CheckoutProcessResult> ProcessOrderAsync(string? userId, CheckoutVM form);
    }

    public record CheckoutProcessResult(
        bool Succeeded,
        bool RedirectToCart,
        string Message,
        CartVM? Cart,
        int? OrderId
    );
}
