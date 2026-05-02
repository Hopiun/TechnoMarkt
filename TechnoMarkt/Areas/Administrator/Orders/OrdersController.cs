using TechnoMarkt.Areas.Administrator.Orders.ViewModels;
using TechnoMarkt.Areas.Operator.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Services;
using TechnoMarkt.Controllers;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Orders.Services;

namespace TechnoMarkt.Areas.Administrator.Orders
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class OrdersController : EmployeeCabinetController
    {
        private readonly IAdministratorOrderService _ordersService;

        public OrdersController(AppDbContext context, IAdministratorOrderService ordersService) : base(context)
        {
            _ordersService = ordersService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] MonthYearFilter? topOrderChartFilter)
        {
            topOrderChartFilter ??= new MonthYearFilter();

            return View(new AdminOrdersViewModel()
            {
                Kpi = await _ordersService.GetKpiAsync(StoreId),
                TopOrderChartFilter = topOrderChartFilter,
                TopOrders = await _ordersService.GetTopOrdersAsync(StoreId, topOrderChartFilter),
                OrdersCountByStatus = await _ordersService.GetOrdersCountByStatusAsync(StoreId),
                OrdersTable = new OrdersTableVM()
                {
                    Role = CurrentRole,
                    Rows = await _ordersService.GetOrdersAsync(StoreId, new OrdersFilter()),
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Filter(OrdersFilter filter)
        {
            OrdersTableVM tableModel = new OrdersTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _ordersService.GetOrdersAsync(StoreId, filter)
            };

            return PartialView("~/Views/Shared/Tables/_Orders.cshtml", tableModel);
        }

        [HttpGet]
        public IActionResult Details(int orderId) => ViewComponent("OrderDetails", new { orderId, CurrentRole });
    }
}
