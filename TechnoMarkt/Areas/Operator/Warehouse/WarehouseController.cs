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
using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Operator.Warehouse
{
    [Area("Operator")]
    [Authorize(Roles = "Operator")]
    public class WarehouseController : EmployeeCabinetController
    {
        private readonly IOperatorStockService _stockService;

        public WarehouseController(AppDbContext context, IOperatorStockService stockService) : base(context)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            StockFilter filter = new StockFilter();

            return View(new StockTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _stockService.GetStockItemsAsync(StoreId, filter),
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




