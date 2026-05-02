using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Shared.Common;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Home.Catalog.ViewModels
{
    public class HomeCatalogViewModel
    {
        public SelectList Cities { get; set; } = null!;
        public int? SelectedCityId { get; set; }
        public SelectList Stores { get; set; } = null!;
        public int? SelectedStoreId { get; set; }
        public SelectList Brands { get; set; } = null!;
        public List<CategoryItem> Categories { get; set; } = [];
        public ItemsGridVM<HomeItemsFilter> Items { get; set; } = null!;
    }
}



