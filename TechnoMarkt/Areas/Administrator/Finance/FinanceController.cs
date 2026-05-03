using TechnoMarkt.Areas.Administrator.Finance.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechnoMarkt.Data;
using TechnoMarkt.Controllers;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Administrator.Finance
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class FinanceController : EmployeeCabinetController
    {
        private readonly IFinanceService _financeService;
        private readonly ReportService _reportService;

        public FinanceController(AppDbContext context, IFinanceService financeService, ReportService reportService) : base(context)
        {
            _financeService = financeService;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DateRangeFilter? dateFilter)
        {
            dateFilter ??= new DateRangeFilter();

            DateOnly weekStart = DateOnly.FromDateTime(DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1));

            return View(new FinanceViewModel()
            {
                DateFilter = dateFilter,
                Kpi = await _financeService.GetKpiAsync(StoreId, dateFilter),
                TransactionByMethod = await _financeService.GetTransactionsByMethodAsync(StoreId, dateFilter),
                WeekStart = weekStart,
                DailySales = await _financeService.GetDailySalesAsync(StoreId, weekStart),
                QuarterlyYear = DateTime.Now.Year,
                QuarterlyRevenue = await _financeService.GetQuarterlyRevenueAsync(StoreId, DateTime.Now.Year),
                TransactionsTable = new TransactionsTableVM()
                {
                    Role = CurrentRole,
                    Filter = new TransactionsFilter(),
                    Rows = await _financeService.GetTransactionsAsync(StoreId, new TransactionsFilter()),
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReport([FromQuery] DateRangeFilter dateFilter, [FromQuery] DateOnly? weekStart, [FromQuery] int? quarterlyYear, [FromQuery] TransactionsFilter filter)
        {
            dateFilter ??= new DateRangeFilter();
            DateOnly actualWeekStart = weekStart ?? DateOnly.FromDateTime(DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1));
            int actualQuarterlyYear = quarterlyYear ?? DateTime.Now.Year;
            filter ??= new TransactionsFilter();

            FinanceViewModel model = new FinanceViewModel()
            {
                DateFilter = dateFilter,
                Kpi = await _financeService.GetKpiAsync(StoreId, dateFilter),
                TransactionByMethod = await _financeService.GetTransactionsByMethodAsync(StoreId, dateFilter),
                WeekStart = actualWeekStart,
                DailySales = await _financeService.GetDailySalesAsync(StoreId, actualWeekStart),
                QuarterlyYear = actualQuarterlyYear,
                QuarterlyRevenue = await _financeService.GetQuarterlyRevenueAsync(StoreId, actualQuarterlyYear),
                TransactionsTable = new TransactionsTableVM()
                {
                    Role = CurrentRole,
                    Filter = filter,
                    Rows = await _financeService.GetTransactionsAsync(StoreId, filter),
                }
            };

            byte[] pdf = await _reportService.GeneratePdfReportAsync("/Areas/Administrator/Views/Reports/FinanceReport.cshtml", model);

            return File(pdf, "application/pdf", $"finance-report-{DateTime.Now:yyyy-MM-dd}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> Filter(TransactionsFilter filter)
        {
            TransactionsTableVM tableModel = new TransactionsTableVM()
            {
                Role = CurrentRole,
                Filter = filter,
                Rows = await _financeService.GetTransactionsAsync(StoreId, filter)
            };

            return PartialView("~/Areas/Administrator/_Shared/Views/Shared/Tables/_Transactions.cshtml", tableModel);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentMethodsChart([FromQuery] DateRangeFilter filter)
        {
            var data = await _financeService.GetTransactionsByMethodAsync(StoreId, filter);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> DailySalesChart([FromQuery] DateOnly weekStart)
        {
            var data = await _financeService.GetDailySalesAsync(StoreId, weekStart);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> QuarterlyRevenueChart([FromQuery] int year)
        {
            var data = await _financeService.GetQuarterlyRevenueAsync(StoreId, year);
            return Json(data);
        }
    }
}






