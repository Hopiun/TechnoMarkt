using TechnoMarkt.Areas.Manager.Suppliers.ViewModels;
using Microsoft.AspNetCore.Mvc;
using TechnoMarkt.Data;
using TechnoMarkt.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
namespace TechnoMarkt.Areas.Manager.Suppliers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class SuppliersController : EmployeeCabinetController
    {
        private readonly ISuppliersService _suppliersService;
        private readonly IEventLogsService _eventLogsService;

        public SuppliersController(AppDbContext context, ISuppliersService suppliersService, IEventLogsService eventLogsService) : base(context)
        {
            _suppliersService = suppliersService;
            _eventLogsService = eventLogsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SuppliersFilter filter = new SuppliersFilter();
            filter.Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryId", "Name");

            return View(new SuppliersTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _suppliersService.GetSuppliersAsync(filter)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Filter(SuppliersFilter filter)
        {
            filter.Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryId", "Name");

            SuppliersTableVM tableModel = new SuppliersTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _suppliersService.GetSuppliersAsync(filter)
            };

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Tables/_Suppliers.cshtml", tableModel);
        }

        [HttpGet]
        public async Task<IActionResult> SupplierItemsList(int supplierId)
        {
            Dictionary<string, List<SupplierItemsRow>> supplierItems = await _suppliersService
                .GetSupplierItemsAsync(supplierId);

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Tables/_SupplierItems.cshtml", supplierItems);
        }

        [HttpGet]
        public async Task<IActionResult> SupplierForm(int? supplierId)
        {
            SupplierFormVM? form = await _suppliersService.GetSupplierFormAsync(supplierId);

            if (supplierId.HasValue && form == null)
                return NotFound();

            return PartialView("~/Areas/Manager/_Shared/Views/Shared/Forms/_SupplierForm.cshtml", form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(SupplierFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            (bool succeeded, string message) = await _suppliersService.AddSupplierAsync(form);
            if (succeeded)
                await _eventLogsService.LogAsync("Create", "Supplier", null, $"Додано постачальника: '{form.Name}', Контакт: {form.Contact ?? "—"}");
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(int supplierId, SupplierFormVM form)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = $"Помилка валідації: {errors}" });
            }

            (bool succeeded, string message) = await _suppliersService.UpdateSupplierAsync(supplierId, form);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Supplier", supplierId, $"Оновлено постачальника (ID:{supplierId}): '{form.Name}', Контакт: {form.Contact ?? "—"}");
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSupplier([FromForm] int supplierId)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            (bool succeeded, string message) = await _suppliersService.DeleteSupplierAsync(supplierId);
            if (succeeded)
                await _eventLogsService.LogAsync("Delete", "Supplier", supplierId, $"Видалено постачальника: '{supplier?.Name ?? "ID:" + supplierId}'");
            return Ok(new { success = succeeded, message });
        }
    }
}
