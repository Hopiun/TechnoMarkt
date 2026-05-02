namespace TechnoMarkt.Areas.Manager.Suppliers.ViewModels
{
    public class SupplierItemsRow
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public double? Weight { get; set; }
        public decimal Price { get; set; }
    }
}
