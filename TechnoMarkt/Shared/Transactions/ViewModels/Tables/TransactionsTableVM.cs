using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Shared.Transactions.ViewModels.Tables
{
    public class TransactionsTableVM : TableVM<TransactionsTableRow, TransactionsFilter> { }

    public record TransactionsTableRow(int PaymentId, int OrderId, decimal Amount, PayMethod Method, DateTime PaidAt, PaymentStatus Status);

    public class TransactionsFilter : DateRangeFilter
    {
        public PayMethod? PaymentMethod { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}





