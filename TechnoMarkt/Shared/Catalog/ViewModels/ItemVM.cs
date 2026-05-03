using TechnoMarkt.Models;
using TechnoMarkt.Shared.Catalog.ViewModels;

namespace TechnoMarkt.Shared.Catalog.ViewModels
{
    public class ItemCardVM
    {
        public string Role { get; set; } = string.Empty;
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public StockStatus StockStatus { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public DiscountVM? Discount { get; set; }
        public decimal FinalPrice => Price * (1 - (Discount?.Percent ?? 0m) / 100);
    }

    public class ItemVM : ItemCardVM
    {
        public string CategoryName { get; set; } = null!;
        public string SupplierName { get; set; } = null!;

        public string? Description { get; set; }
        public double? Weight { get; set; }
        public string? Dimensions { get; set; }
    }
}



