using TechnoMarkt.Areas.Operator.Warehouse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TechnoMarkt.Data;
using TechnoMarkt.Services;
using TechnoMarkt.Controllers;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Kpi;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Manager.Warehouse
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class WarehouseController : EmployeeCabinetController
    {
        private readonly IManagerStockService _stockService;
        private readonly IEventLogsService _eventLogsService;

        public WarehouseController(AppDbContext context, IManagerStockService stockService, IEventLogsService eventLogsService) : base(context)
        {
            _stockService = stockService;
            _eventLogsService = eventLogsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] StockFilter? filter, [FromQuery] WarehouseKpiFilter? kpiFilter)
        {
            filter ??= new StockFilter();
            kpiFilter ??= new WarehouseKpiFilter();

            return View(new StockKpiVM()
            {
                Kpi = await _stockService.GetKpiAsync(StoreId, kpiFilter),
                KpiFilter = kpiFilter,
                StockTable = new StockTableVM()
                {
                    Role = CurrentRole,
                    Filter = filter,
                    Rows = await _stockService.GetStockItemsAsync(StoreId, new StockFilter())
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Filter(StockFilter filter)
        {
            StockTableVM tableModel = new StockTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _stockService.GetStockItemsAsync(StoreId, filter),
            };

            return PartialView("~/Views/Shared/Tables/_StockItems.cshtml", tableModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestockItem(int warehouseId, int addedQuantity)
        {
            if (addedQuantity <= 0)
            {
                return BadRequest(new { success = false, message = "Кількість повинна бути більшою за нуль." });
            }

            var wh = await _context.Warehouses
                .Include(w => w.Item)
                .Include(w => w.Store)
                .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId);
            var itemName = wh?.Item?.Name ?? $"ID:{warehouseId}";

            (bool succeeded, string message) = await _stockService.RestockItemAsync(warehouseId, StoreId, addedQuantity);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Warehouse", warehouseId, $"Поповнення складу: '{itemName}', +{addedQuantity} шт. (Магазин ID:{StoreId})");
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustInventory(int warehouseId, int exactQuantity)
        {
            if (exactQuantity < 0)
            {
                return BadRequest(new { success = false, message = "Кількість не може бути від'ємною." });
            }

            var wh = await _context.Warehouses
                .Include(w => w.Item)
                .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId);
            var itemName = wh?.Item?.Name ?? $"ID:{warehouseId}";

            (bool succeeded, string? message) = await _stockService.AdjustInventoryAsync(warehouseId, StoreId, exactQuantity);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Warehouse", warehouseId, $"Коригування залишків: '{itemName}', встановлено {exactQuantity} шт. (Магазин ID:{StoreId})");
            return Ok(new { success = succeeded, message });
        }
    }
}






