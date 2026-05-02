using TechnoMarkt.Areas.Administrator.Settings.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechnoMarkt.Data;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Controllers;

namespace TechnoMarkt.Areas.Administrator.Settings
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class SettingsController : EmployeeCabinetController
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(AppDbContext context, ISettingsService settingsService) : base(context)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() => View(await _settingsService.GetSettingsPageAsync());

        [HttpPost]
        public async Task<IActionResult> FilterEventLogs(EventLogsFilter filter)
        {
            var tableModel = await _settingsService.GetEventLogsTableAsync(filter ?? new EventLogsFilter(), pageSize: 50);
            return PartialView("~/Areas/Administrator/_Shared/Views/Shared/Tables/_EventLogsTable.cshtml", tableModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(Dictionary<string, string> settings)
        {
            try
            {
                var result = await _settingsService.UpdateSettingsAsync(settings);
                TempData["Success"] = result.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка оновлення: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EventLogs(EventLogsFilter? filter, int currentPage = 1)
        {
            filter ??= new EventLogsFilter();
            var tableModel = await _settingsService.GetEventLogsTableAsync(filter, pageSize: 20, currentPage: currentPage);
            return View(new SettingsViewModel { EventLogsTable = tableModel });
        }

        [HttpGet]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                var result = await _settingsService.CreateBackupAsync();
                if (!result.Succeeded || result.FileBytes == null || string.IsNullOrWhiteSpace(result.FileName))
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction("Index");
                }
                return File(result.FileBytes, "application/sql", result.FileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка створення бекапу: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreBackup(IFormFile backupFile)
        {
            try
            {
                var result = await _settingsService.RestoreBackupAsync(backupFile);
                if (result.Succeeded) TempData["Success"] = result.Message;
                else TempData["Error"] = result.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Помилка відновлення: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}







