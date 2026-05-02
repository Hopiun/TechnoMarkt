using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Areas.Administrator.Dashboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;

using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context) => _context = context;

        public async Task<DashboardKpiDto> GetKpiAsync(int storeId)
        {
            const int criticalQuantityThreshold = 5;
            DateRange month = DateRange.ThisMonth;

            decimal monthlyRevenue = await _context.Payments.ByStore(storeId).InPeriod(month).Completed()
                .SumAsync(payment => payment.Amount);

            IQueryable<Order> completedOrders = _context.Orders.ByStore(storeId).InPeriod(month).Completed();

            decimal averageOrder = await completedOrders.Select(order => (decimal?)order.OrderTotal).AverageAsync() ?? 0m;

            int completedOrdersCount = await completedOrders.CountAsync();

            int criticalStockCount = await _context.Warehouses.ByStore(storeId)
                .CountAsync(warehouse => warehouse.Quantity <= criticalQuantityThreshold);

            return new DashboardKpiDto(monthlyRevenue, averageOrder, completedOrdersCount, criticalStockCount);
        }

        public async Task<Dictionary<string, decimal>> GetSalesByCategoryAsync(int storeId)
        {
            DateRange month = DateRange.ThisMonth;

            return await _context.OrderLines.ByStore(storeId).InPeriod(month).Completed()
                .GroupBy(orderLine => orderLine.Item.Category.ParentCategory != null ?
                orderLine.Item.Category.ParentCategory.Name : orderLine.Item.Category.Name)
                .Select(group => new
                {
                    Category = group.Key,
                    Total = group.Sum(orderLine => orderLine.PriceAtMoment * orderLine.Quantity)
                })
                .ToDictionaryAsync(x => x.Category, x => x.Total);
        }

        public async Task<List<TransactionsTableRow>> GetRecentTransactionsAsync(int storeId)
        {
            DateRange today = DateRange.Today;

            return await _context.Payments.ByStore(storeId).InPeriod(today)
                .OrderByDescending(payment => payment.Date)
                .Take(5)
                .Select(payment => new TransactionsTableRow(
                    payment.PaymentId,
                    payment.OrderId,
                    payment.Amount,
                    payment.Method,
                    payment.Date,
                    payment.Status
                ))
                .ToListAsync();
        }

        public async Task<Dictionary<DateOnly, decimal>> GetDailySalesAsync(int storeId)
        {
            DateRange week = DateRange.ThisWeek;

            return await _context.Payments.ByStore(storeId).InPeriod(week).Completed()
                .GroupBy(payment => DateOnly.FromDateTime(payment.Date))
                .Select(group => new
                {
                    Date = group.Key,
                    Total = group.Sum(payment => payment.Amount)
                })
                .ToDictionaryAsync(x => x.Date, x => x.Total);
        }
    }
}





