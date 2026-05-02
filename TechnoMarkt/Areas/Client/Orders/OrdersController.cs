using TechnoMarkt.Areas.Client.ClientCabinet;
using TechnoMarkt.Areas.Client.Orders.ViewModels;
using TechnoMarkt.Areas.Operator.Orders;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechnoMarkt.Data;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;

namespace TechnoMarkt.Areas.Client.Orders
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public class OrdersController : ClientCabinetController
    {
        private readonly IClientOrdersService _ordersService;
        private readonly IEventLogsService _eventLogsService;

        public OrdersController(IClientOrdersService ordersService, AppDbContext context, IEventLogsService eventLogsService) : base(context)
        {
            _ordersService = ordersService;
            _eventLogsService = eventLogsService;
        }

        private int ClientId => int.Parse(User.Claims.First(c => c.Type == "ClientId").Value);

        public async Task<IActionResult> Index()
        {
            var filter = new ClientOrdersFilter();
            var orders = await _ordersService.GetOrdersAsync(ClientId, filter);

            var viewModel = new ClientOrdersViewModel()
            {
                OrdersTable = new ClientOrdersTableVM()
                {
                    Role = "Client",
                    Rows = orders,
                    Filter = null
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(ClientOrdersFilter filter)
        {
            var orders = await _ordersService.GetOrdersAsync(ClientId, filter);
            
            var tableFilter = new ClientOrdersFilter() 
            {
                Status = filter.Status,
                DateFrom = filter.DateFrom,
                DateTo = filter.DateTo
            };

            return PartialView("_ClientOrdersTable", new ClientOrdersTableVM()
            {
                Role = "Client",
                Rows = orders,
                Filter = tableFilter
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var (succeeded, notification) = await _ordersService.CancelOrderAsync(orderId, ClientId);
            if (succeeded)
                await _eventLogsService.LogAsync("Update", "Order", orderId, $"РљР»С–С”РЅС‚ СЃРєР°СЃСѓРІР°РІ Р·Р°РјРѕРІР»РµРЅРЅСЏ #{orderId}");
            return Json(new { success = succeeded, message = notification });
        }
        
        [HttpGet]
        public IActionResult Details(int orderId) => ViewComponent("OrderDetails", new { orderId, role = "Client" });
    }
}








