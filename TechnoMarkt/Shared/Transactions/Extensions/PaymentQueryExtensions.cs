using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Utilities;

namespace TechnoMarkt.Shared.Transactions.Extensions
{
    public static class PaymentQueryExtensions
    {
        public static IQueryable<Payment> ByStore(this IQueryable<Payment> query, int storeId) =>
            query.Where(payment => payment.Order.StoreId == storeId);

        public static IQueryable<Payment> InPeriod(this IQueryable<Payment> query, DateRange range) =>
            query.Where(payment => payment.Date >= range.From && payment.Date <= range.To);

        public static IQueryable<Payment> WithMethod(this IQueryable<Payment> query, PayMethod method) =>
            query.Where(payment => payment.Method == method);

        public static IQueryable<Payment> WithStatus(this IQueryable<Payment> query, PaymentStatus status) =>
            query.Where(payment => payment.Status == status);

        public static IQueryable<Payment> Completed(this IQueryable<Payment> query) =>
            query.WithStatus(PaymentStatus.Completed);
    }
}

