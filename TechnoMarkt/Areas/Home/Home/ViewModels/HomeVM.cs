using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Home.Home.ViewModels
{
    public class HomeViewModel
    {
        public SelectList Cities { get; set; } = null!;
        public int? SelectedCityId { get; set; }
        public SelectList Stores { get; set; } = null!;
        public int? SelectedStoreId { get; set; }
        public List<CategoryCard> Categories { get; set; } = [];
        public List<ItemCardVM> TopItems { get; set; } = [];
        public string? ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientWallet { get; set; }
    }
}



