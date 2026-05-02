using TechnoMarkt.Models;
namespace TechnoMarkt.Shared.Catalog.ViewModels
{
    public class DiscountVM
    {
        public int Id { get; set; }
        public int Percent { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
    }
}


