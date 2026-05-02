using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Areas.Manager.Suppliers.ViewModels
{
    public class SuppliersTableVM : TableVM<SuppliersTableRow, SuppliersFilter> { }

    public class SuppliersTableRow
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal? Rating { get; set; }
    }

    public class SuppliersFilter
    {
        public string? Search { get; set; }
        public decimal? MinRating { get; set; }
        public int? CountryId { get; set; }

        public SelectList? Countries { get; set; }
    }
}




