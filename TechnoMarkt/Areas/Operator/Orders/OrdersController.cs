using TechnoMarkt.Areas.Operator.Orders.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Services;
using TechnoMarkt.Controllers;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Orders.Services;

namespace TechnoMarkt.Areas.Operator.Orders
{
    [Area("Operator")]
    [Authorize(Roles = "Operator")]
    public class OrdersController : EmployeeCabinetController
    {
        private readonly IOperatorOrderService _ordersService;
        private readonly IEventLogsService _eventLogsService;

        public OrdersController(AppDbContext context, IOperatorOrderService ordersService, IEventLogsService eventLogsService) : base(context)
        {
            _ordersService = ordersService;
            _eventLogsService = eventLogsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new OperatorOrdersViewModel()
            {
                Kpi = await _ordersService.GetKpiAsync(StoreId),
                OrderCountByStatus = await _ordersService.GetOrdersCountByStatusAsync(StoreId),
                OrdersTable = new OrdersTableVM()
                {
                    Role = CurrentRole,
                    Rows = await _ordersService.GetOrdersAsync(StoreId, new OrdersFilter())
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Filter(OrdersFilter filter)
        {
            return PartialView("~/Views/Shared/Tables/_Orders.cshtml", new OrdersTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _ordersService.GetOrdersAsync(StoreId, filter)
            });
        }

        [HttpGet]
        public IActionResult Details(int orderId) => ViewComponent("OrderDetails", new { orderId, role = CurrentRole });

        [HttpPost]
        public async Task<IActionResult> AdvanceOrderStatus([FromForm] int orderId)
        {
            (bool succeeded, string message) = await _ordersService.AdvanceOrderStatusAsync(orderId, EmployeeId);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Order", orderId, $"Просунуто статус замовлення #{orderId}");
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder([FromForm] int orderId, [FromForm] PayMethod method, [FromForm] int? paymentMethodId = null)
        {
            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            var clientName = order?.Client != null ? $"{(order.Client.LastName)} {(order.Client.FirstName)}" : "?";

            (bool succeeded, string message) = await _ordersService.CompleteOrderAsync(orderId, EmployeeId, method, paymentMethodId);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Order", orderId, $"Оплачено замовлення #{orderId} на суму {order?.OrderTotal:N0}, клієнт: {clientName}, спосіб: {method}");
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> ReturnOrder([FromForm] int orderId)
        {
            (bool succeeded, string message) = await _ordersService.ReturnOrderAsync(orderId, EmployeeId);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Order", orderId, $"Оформлено повернення замовлення #{orderId}");
            return Ok(new { success = succeeded, message });
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder([FromForm] int orderId)
        {
            (bool succeeded, string message) = await _ordersService.CancelOrderAsync(orderId);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Order", orderId, $"Скасовано замовлення #{orderId}");
            return Ok(new { success = succeeded, message });
        }
    }
}
