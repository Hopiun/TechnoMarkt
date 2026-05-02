using TechnoMarkt.Areas.Operator.Warehouse;
using Microsoft.AspNetCore.Mvc;
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
using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Administrator.Warehouse
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class WarehouseController : EmployeeCabinetController
    {
        private readonly IAdministratorStockService _stockService;

        public WarehouseController(AppDbContext context, IAdministratorStockService stockService) : base(context)
        {
            _stockService = stockService;
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
    }
}






