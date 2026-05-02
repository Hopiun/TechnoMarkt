using TechnoMarkt.Areas.Client.ClientCabinet;
using TechnoMarkt.Areas.Client.Profile.ViewModels;
using Microsoft.AspNetCore.Mvc;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Client.Profile
{
    public class ProfileController : ClientCabinetController
    {
        private readonly IProfileService _profileService;
        private readonly IEventLogsService _eventLogsService;

        public ProfileController(AppDbContext context, IProfileService profileService, IEventLogsService eventLogsService)
            : base(context)
        {
            _profileService = profileService;
            _eventLogsService = eventLogsService;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _profileService.GetProfileAsync(ClientId);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        public async Task<IActionResult> Edit()
        {
            var profile = await _profileService.GetProfileAsync(ClientId);
            if (profile == null)
            {
                return NotFound();
            }

            var model = new EditProfileFormVM
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _profileService.UpdateProfileAsync(ClientId, model.FirstName, model.LastName);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            await _eventLogsService.LogAsync("Update", "Client", ClientId, $"РћРЅРѕРІР»РµРЅРѕ РїСЂРѕС„С–Р»СЊ: {model.FirstName} {model.LastName}");
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChangeEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _profileService.ChangeEmailAsync(ClientId, model.NewEmail, model.CurrentPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            await _eventLogsService.LogAsync("Update", "Client", ClientId, $"Р—РјС–РЅРµРЅРѕ email РЅР° {model.NewEmail}");
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordFormVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _profileService.ChangePasswordAsync(ClientId, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            await _eventLogsService.LogAsync("Update", "Client", ClientId, "Р—РјС–РЅРµРЅРѕ РїР°СЂРѕР»СЊ");
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var result = await _profileService.DeleteAccountAsync(ClientId);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            await _eventLogsService.LogAsync("Delete", "Client", ClientId, "Р’РёРґР°Р»РµРЅРѕ Р°РєР°СѓРЅС‚ РєР»С–С”РЅС‚Р°");
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }
    }
}






