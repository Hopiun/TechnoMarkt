using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Manager.Catalog.ViewModels
{
    public class CatalogViewModel
    {
        public MonthYearFilter TopFilter { get; set; } = new();
        public List<CategorySalesDto> TopCategories { get; set; } = [];
        public List<ItemRowDto> TopItems { get; set; } = [];

        public List<CategoryItem> Categories { get; set; } = [];
        public ItemsGridVM<ManagerItemsFilter> Items { get; set; } = new();
    }

    public record CategorySalesDto(int CategoryId, string CategoryName, decimal TotalSales, int CompletedOrdersCount);

    public record ItemRowDto(int ItemId, string ItemName, string CategoryName, decimal UnitPrice, int SoldQuantity, decimal TotalRevenue);
}




