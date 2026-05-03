using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Shared.Common;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Home.Catalog.ViewModels
{
    public class HomeItemsFilter : ItemsFilter
    {
        public int? CityId { get; set; }
        public SelectList? Cities { get; set; }
        
        public List<int>? BrandIds { get; set; }
        public MultiSelectList? Brands { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? MinPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxPrice { get; set; }

        public bool InStockOnly { get; set; } = false;
        public bool WithDiscountsOnly { get; set; } = false;
    }
}

