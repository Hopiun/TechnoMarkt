using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Areas.Manager.Catalog.ViewModels
{
    public class ManagerItemsFilter : ItemsFilter
    {
        public int? SupplierId { get; set; }
        public SelectList? Suppliers { get; set; }
    }
}

