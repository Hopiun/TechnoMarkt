using Microsoft.AspNetCore.Mvc;
using TechnoMarkt.Shared.Cart.ViewModels;
using TechnoMarkt.Services;

namespace TechnoMarkt.Views.Shared.Components.Cart
{
    public class CartViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cart = await _cartService.GetCartAsync();
            return View(cart);
        }
    }
}

