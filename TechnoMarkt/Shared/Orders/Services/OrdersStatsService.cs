using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.Orders.ViewModels.Kpi;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechnoMarkt.Shared.Orders.Services
{
    public class OrdersStatsService : IOrdersStatsService
    {
        private readonly AppDbContext _context;

        public OrdersStatsService(AppDbContext context) => _context = context;

        public async Task<OrdersKpiDto> GetKpiAsync(int storeId)
        {
            DateRange today = DateRange.Today;

            int todayActiveCount = await _context.Orders.ByStore(storeId).InPeriod(today)
                .CountAsync(order => order.Status != OrderStatus.Returned
                                  && order.Status != OrderStatus.Cancelled);

            var todayPayments = await _context.Payments.ByStore(storeId).InPeriod(today).Completed()
                .Select(payments => new { payments.Amount, payments.Method })
                .ToListAsync();

            decimal todayPaidTotal = todayPayments.Sum(payment => payment.Amount);

            decimal todayPaidCashTotal = todayPayments.Where(payment => payment.Method == PayMethod.Cash).Sum(payment => payment.Amount);
            decimal todayPaidCardTotal = todayPayments
                .Where(payment => payment.Method == PayMethod.Card || payment.Method == PayMethod.Online).Sum(payment => payment.Amount);

            return new OrdersKpiDto(todayActiveCount, todayPaidTotal, todayPaidCashTotal, todayPaidCardTotal);
        }

        public async Task<Dictionary<OrderStatus, int>> GetOrdersCountByStatusAsync(int storeId)
        {
            DateRange today = DateRange.Today;

            return await _context.Orders.ByStore(storeId).InPeriod(today)
                .GroupBy(order => order.Status)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<List<OrdersTableRow>> GetTopOrdersAsync(int storeId, MonthYearFilter filter)
        {
            DateRange monthYearRange = DateRange.FromMonthYear(filter.Month, filter.Year);

            return await _context.Orders.ByStore(storeId).InPeriod(monthYearRange).Completed()
                .OrderByDescending(order => order.OrderTotal)
                .Select(order => new OrdersTableRow()
                {
                    OrderId = order.OrderId,
                    ClientId = order.ClientId,
                    ClientFullName = $"{order.Client.FirstName} {order.Client.LastName}",
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate,
                    OrderStatus = order.Status,
                    OrderTotal = order.OrderTotal
                })
                .Take(5)
                .ToListAsync();
        }
    }
}
