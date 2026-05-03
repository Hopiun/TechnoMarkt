using TechnoMarkt.Areas.Administrator.Dashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TechnoMarkt.Data;
using TechnoMarkt.Services;
using TechnoMarkt.Controllers;

namespace TechnoMarkt.Areas.Administrator.Dashboard
{
    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class DashboardController : EmployeeCabinetController
    {
        private readonly IDashboardService _dashboardService;
        private readonly ReportService _reportService;

        public DashboardController(AppDbContext context, IDashboardService dashboardService, ReportService reportService) : base(context)
        {
            _dashboardService = dashboardService;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(new DashboardViewModel()
            {
                Kpi = await _dashboardService.GetKpiAsync(StoreId),
                SalesByCategory = await _dashboardService.GetSalesByCategoryAsync(StoreId),
                RecentTransactions = await _dashboardService.GetRecentTransactionsAsync(StoreId),
                DailySales = await _dashboardService.GetDailySalesAsync(StoreId)
            });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReport()
        {
            DashboardViewModel model = new DashboardViewModel()
            {
                Kpi = await _dashboardService.GetKpiAsync(StoreId),
                SalesByCategory = await _dashboardService.GetSalesByCategoryAsync(StoreId),
                RecentTransactions = await _dashboardService.GetRecentTransactionsAsync(StoreId),
                DailySales = await _dashboardService.GetDailySalesAsync(StoreId)
            };

            byte[] pdf = await _reportService.GeneratePdfReportAsync("/Areas/Administrator/Views/Reports/DashboardReport.cshtml", model);

            return File(pdf, "application/pdf", $"dashboard-report-{DateTime.Now:yyyy-MM-dd}.pdf");
        }
    }
}



