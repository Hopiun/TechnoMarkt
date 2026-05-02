using TechnoMarkt.Models;
using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Shared.Common;
using TechnoMarkt.Shared.Common.ViewModels;

namespace TechnoMarkt.Shared.Catalog.ViewModels
{
    public class ItemsGridVM<TFilter> where TFilter : new()
    {
        public ItemsGridVM() { }
        public string Role { get; set; } = string.Empty;
        public SelectedCategoryItem? SelectedCategory { get; set; }
        public TFilter Filter { get; set; } = new TFilter();
        public List<ItemCardVM> Items { get; set; } = [];
        
        public PaginationVM Pagination { get; set; } = new PaginationVM();
    }

    public enum StockStatus
    {
        InStock,
        Low,
        OutOfStock
    }

    public enum ItemsSortBy
    {
        Name,
        Price
    }

    public class ItemsFilter
    {
        public string? Name { get; set; }

        public ItemsSortBy SortBy { get; set; }
        public bool SortAscending { get; set; } = true;

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 8;
    }
}
