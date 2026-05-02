using TechnoMarkt.Areas.Client.ClientCabinet;
using TechnoMarkt.Areas.Client.Wallet.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnoMarkt.Data;
using TechnoMarkt.Models.Identity;
using System.Security.Claims;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Client.Wallet
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class WalletController : ClientCabinetController
    {
        private readonly IWalletService _walletService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEventLogsService _eventLogsService;

        public WalletController(IWalletService walletService, AppDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IEventLogsService eventLogsService) : base(context)
        {
            _walletService = walletService;
            _signInManager = signInManager;
            _userManager = userManager;
            _eventLogsService = eventLogsService;
        }

        public async Task<IActionResult> Index()
        {
            var cid = ClientId;
            if (cid == 0) return RedirectToAction("Login", "Account", new { area = "" });

            var balance = await _walletService.GetWalletBalanceAsync(cid);
            var paymentMethods = await _walletService.GetPaymentMethodsAsync(cid);

            ViewBag.CurrentBalance = balance;

            var model = new WalletViewModel
            {
                Balance = balance,
                PaymentMethods = paymentMethods
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPaymentMethod(PaymentMethodFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br/>", ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage));
                TempData["Error"] = errors;
                return RedirectToAction(nameof(Index));
            }

            var result = await _walletService.AddPaymentMethodAsync(ClientId, form.BankName, form.CardNumber, form.CardType);
            if (result.Succeeded)
            {
                await _eventLogsService.LogAsync("Create", "PaymentMethod", ClientId, $"Р”РѕРґР°РЅРѕ РјРµС‚РѕРґ РѕРїР»Р°С‚Рё: {form.BankName} ({form.CardType})");
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaymentMethod(int paymentMethodId)
        {
            var result = await _walletService.DeletePaymentMethodAsync(ClientId, paymentMethodId);
            if (result.Succeeded)
            {
                await _eventLogsService.LogAsync("Delete", "PaymentMethod", paymentMethodId, "Р’РёРґР°Р»РµРЅРѕ РјРµС‚РѕРґ РѕРїР»Р°С‚Рё");
                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopUp(decimal amount)
        {
            var cid = ClientId;
            var result = await _walletService.TopUpWalletAsync(cid, amount);
            if (result.Succeeded)
            {
                await _eventLogsService.LogAsync("Update", "Client", cid, $"РџРѕРїРѕРІРЅРµРЅРѕ Р±РѕРЅСѓСЃРЅРёР№ СЂР°С…СѓРЅРѕРє РЅР° {amount:N0}");

                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    var oldClaim = claims.FirstOrDefault(c => c.Type == "WalletBalance");
                    if (oldClaim != null) await _userManager.RemoveClaimAsync(user, oldClaim);

                    await _userManager.AddClaimAsync(user, new Claim("WalletBalance", result.NewBalance.ToString("N0")));
                    await _signInManager.RefreshSignInAsync(user);
                }

                TempData["Success"] = result.Message;
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}






