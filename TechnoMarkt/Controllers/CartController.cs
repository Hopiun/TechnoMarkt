using Microsoft.AspNetCore.Mvc;
using TechnoMarkt.Shared.Cart.ViewModels;

namespace TechnoMarkt.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartFacadeService _cartService;

        public CartController(ICartFacadeService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult GetCount()
        {
            var count = _cartService.GetTotalItemsCount();
            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartAsync();
            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> ModalPartial()
        {
            var cart = await _cartService.GetCartAsync();
            return PartialView("~/Views/Shared/Partials/Cart/_CartModalPartial.cshtml", cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartVM form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage));
                    return BadRequest(new { error = $"Помилка валідації: {errors}" });
                }

                var user = User.Identity?.Name ?? "Гість";
                var result = await _cartService.AddAsync(form, user);
                if (!result.Succeeded)
                    return BadRequest(new { error = result.Error ?? "Помилка додавання" });

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var count = _cartService.GetTotalItemsCount();
                    return Json(new { success = true, count });
                }

                TempData["Success"] = "Товар додано до корзини";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CART ADD ERROR]: {ex.Message}\n{ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[INNER EXCEPTION]: {ex.InnerException.Message}");
                }
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return BadRequest(new { error = $"Помилка: {ex.Message} - {ex.InnerException?.Message}" });
                }
                
                TempData["Error"] = "Виникла помилка: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(int itemId, int quantity)
        {
            var user = User.Identity?.Name ?? "Гість";
            await _cartService.UpdateAsync(itemId, quantity, user);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cart = await _cartService.GetCartAsync();
                return PartialView("~/Views/Shared/Components/Cart/Default.cshtml", cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int itemId)
        {
            var user = User.Identity?.Name ?? "Гість";
            await _cartService.RemoveAsync(itemId, user);

            TempData["Success"] = "Товар видалено з корзини";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cart = await _cartService.GetCartAsync();
                return PartialView("~/Views/Shared/Components/Cart/Default.cshtml", cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var user = User.Identity?.Name ?? "Гість";
            await _cartService.ClearAsync(user);

            TempData["Success"] = "Корзину очищено";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cart = await _cartService.GetCartAsync();
                return PartialView("~/Views/Shared/Components/Cart/Default.cshtml", cart);
            }

            return RedirectToAction("Index");
        }
    }
}


