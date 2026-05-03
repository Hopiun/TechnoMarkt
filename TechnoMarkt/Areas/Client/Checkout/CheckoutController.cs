using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnoMarkt.Shared.Cart.ViewModels;
using System.Security.Claims;

namespace TechnoMarkt.Areas.Client.Checkout
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class CheckoutController : Controller
    {
        private readonly IClientCheckoutService _checkoutService;

        public CheckoutController(IClientCheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var checkoutData = await _checkoutService.GetCheckoutCartAsync();
            if (!checkoutData.HasItems)
            {
                TempData["Warning"] = "Корзина пуста";
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutVM();
            ViewData["Cart"] = checkoutData.Cart;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(CheckoutVM form)
        {
            var checkoutData = await _checkoutService.GetCheckoutCartAsync();
            var cart = checkoutData.Cart;

            if (!checkoutData.HasItems)
            {
                TempData["Error"] = "Корзина пуста";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Cart"] = cart;
                return View("Index", form);
            }

            var result = await _checkoutService.ProcessOrderAsync(User.FindFirstValue(ClaimTypes.NameIdentifier), form);
            if (result.Succeeded)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Index", "Orders");
            }

            if (result.RedirectToCart)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index", "Cart");
            }

            TempData["Error"] = result.Message;
            ViewData["Cart"] = result.Cart ?? cart;
            return View("Index", form);
        }
    }
}




