using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Areas.Administrator.Finance.ViewModels;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;

using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Administrator.Finance
{
    public class FinanceService : IFinanceService
    {
        private readonly AppDbContext _context;

        public FinanceService(AppDbContext context) => _context = context;

        public async Task<FinanceKpiDto> GetKpiAsync(int storeId, DateRangeFilter filter)
        {
            DateRange thisYear = DateRange.ThisYear;

            DateRange datePeriod = filter.DateRangeSpecified
                ? new DateRange(filter.DateFrom!.Value.ToDateTime(TimeOnly.MinValue), filter.DateTo!.Value.ToDateTime(TimeOnly.MaxValue))
                : DateRange.ThisMonth;

            decimal yearlyRevenue = await _context.Payments.ByStore(storeId).InPeriod(thisYear).Completed()
                .SumAsync(payment => payment.Amount);

            decimal periodRevenue = await _context.Payments.ByStore(storeId).InPeriod(datePeriod).Completed()
                .SumAsync(payment => payment.Amount);

            IQueryable<Order> completedOrdersInPeriod = _context.Orders.ByStore(storeId).InPeriod(datePeriod).Completed();

            int completedOrdersCount = await completedOrdersInPeriod.CountAsync();

            decimal averageOrder = await completedOrdersInPeriod.AverageAsync(order => (decimal?)order.OrderTotal) ?? 0m;

            return new FinanceKpiDto(yearlyRevenue, periodRevenue, completedOrdersCount, averageOrder);
        }


        public async Task<Dictionary<PayMethod, int>> GetTransactionsByMethodAsync(int storeId, DateRangeFilter filter)
        {
            DateRange datePeriod = filter.DateRangeSpecified
               ? new DateRange(filter.DateFrom!.Value.ToDateTime(TimeOnly.MinValue), filter.DateTo!.Value.ToDateTime(TimeOnly.MaxValue))
               : DateRange.ThisMonth;

            return await _context.Payments.ByStore(storeId).InPeriod(datePeriod).Completed()
                .GroupBy(payment => payment.Method)
                .Select(group => new { Method = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Method, x => x.Count);
        }


        public async Task<Dictionary<DateOnly, decimal>> GetDailySalesAsync(int storeId, DateOnly weekStart)
        {
            DateRange week = new DateRange(weekStart.ToDateTime(TimeOnly.MinValue), weekStart.AddDays(6).ToDateTime(TimeOnly.MaxValue));

            return await _context.Payments.ByStore(storeId).InPeriod(week).Completed()
                .GroupBy(payment => DateOnly.FromDateTime(payment.Date))
                .Select(group => new { Date = group.Key, Total = group.Sum(payment => payment.Amount) })
                .ToDictionaryAsync(x => x.Date, x => x.Total);
        }

        public async Task<List<QuarterlyRevenueDto>> GetQuarterlyRevenueAsync(int storeId, int year)
        {
            DateRange range = new DateRange(new DateTime(year, 1, 1), new DateTime(year, 12, 31, 23, 59, 59));

            var payments = await _context.Payments.ByStore(storeId).InPeriod(range).Completed()
                .Select(payment => new { payment.Date.Month, payment.Amount })
                .ToListAsync();

            return payments
                .GroupBy(p => (p.Month - 1) / 3 + 1)
                .Select(group => new QuarterlyRevenueDto(group.Key, year, group.Sum(p => p.Amount)))
                .OrderBy(x => x.Quarter)
                .ToList();
        }

        public async Task<List<TransactionsTableRow>> GetTransactionsAsync(int storeId, TransactionsFilter filter)
        {
            IQueryable<Payment> query = _context.Payments.Where(payment => payment.Order.StoreId == storeId);

            if (filter.DateFrom.HasValue)
                query = query.Where(payment => payment.Date >= filter.DateFrom.Value.ToDateTime(TimeOnly.MinValue));

            if (filter.DateTo.HasValue)
                query = query.Where(payment => payment.Date <= filter.DateTo.Value.ToDateTime(TimeOnly.MaxValue));

            if (filter.PaymentMethod.HasValue)
                query = query.WithMethod(filter.PaymentMethod.Value);

            if (filter.PaymentStatus.HasValue)
                query = query.WithStatus(filter.PaymentStatus.Value);

            return await query
                .OrderByDescending(payment => payment.Date)
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
    }
}





