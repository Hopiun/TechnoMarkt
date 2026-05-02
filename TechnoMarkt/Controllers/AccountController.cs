using TechnoMarkt.Shared.Auth.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TechnoMarkt.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                if (User.IsInRole("Administrator")) return RedirectToAction("Index", "Dashboard", new { area = "Administrator" });
                if (User.IsInRole("Manager")) return RedirectToAction("Index", "Catalog", new { area = "Manager" });
                if (User.IsInRole("Operator")) return RedirectToAction("Index", "Orders", new { area = "Operator" });
                if (User.IsInRole("Client")) return RedirectToAction("Index", "Orders", new { area = "Client" });

                return RedirectToLocal(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM form, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(form);
            var result = await _accountService.LoginAsync(form);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Помилка входу");
                return View(form);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return result.Role switch
            {
                "Administrator" => RedirectToAction("Index", "Dashboard", new { area = "Administrator" }),
                "Manager" => RedirectToAction("Index", "Catalog", new { area = "Manager" }),
                "Operator" => RedirectToAction("Index", "Orders", new { area = "Operator" }),
                "Client" => RedirectToAction("Index", "Orders", new { area = "Client" }),
                _ => RedirectToLocal(returnUrl)
            };
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                if (User.IsInRole("Administrator")) return RedirectToAction("Index", "Dashboard", new { area = "Administrator" });
                if (User.IsInRole("Manager")) return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
                if (User.IsInRole("Operator")) return RedirectToAction("Index", "Orders", new { area = "Operator" });
                if (User.IsInRole("Client")) return RedirectToAction("Index", "Orders", new { area = "Client" });

                return RedirectToAction("Login");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new SignUpVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpVM form, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(form);
            var result = await _accountService.SignUpAsync(form);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Error ?? "Помилка реєстрації");
                return View(form);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Orders", new { area = "Client" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync(User);
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }

        private IActionResult RedirectToLocal(string? returnUrl = null) => Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home", new { area = "Home" });
    }
}

