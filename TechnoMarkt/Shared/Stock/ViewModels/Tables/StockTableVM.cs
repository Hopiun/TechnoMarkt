using TechnoMarkt.Shared.Common.ViewModels.Tables;
using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Stock.ViewModels.Tables
{
    public class StockTableVM : TableVM<StockItemsTableRow, StockFilter> { }

    public class StockItemsTableRow
    {
        public int WarehouseId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public DateOnly? SupplyDate { get; set; }

        public decimal TotalValue => Quantity * UnitPrice;
    }

    public enum StockSortBy
    {
        Name,
        Price,
        Quantity,
        LastSupplyDate
    }

    public class StockFilter
    {
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public StockSortBy SortBy { get; set; } = StockSortBy.Name;
        public bool SortDescending { get; set; } = false;

        public int? CriticalStockThreshold { get; set; }
        public bool ZeroStockOnly { get; set; } = false;
        public int? NotSuppliedDays { get; set; }
    }
}





