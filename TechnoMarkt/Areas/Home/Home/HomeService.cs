using TechnoMarkt.Shared.Orders.Extensions;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Extensions.Query;
using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;


namespace TechnoMarkt.Areas.Home.Home
{
    public class HomeService : IHomeService
    {
        private readonly AppDbContext _context;

        public HomeService(AppDbContext context) => _context = context;
        
        public async Task<List<CategoryCard>> GetMainCategoriesAsync()
        {
            return await _context.Categories.ParentCategories()
                .Select(category => new CategoryCard()
                {
                    Id = category.CategoryId,
                    Name = category.Name
                }).ToListAsync();
        }

        public async Task<List<ItemCardVM>> GetTopItemsAsync()
        {
            var rawData = await _context.OrderLines.InPeriod(DateRange.ThisMonth).Completed()
                .Select(orderLine => new
                {
                    orderLine.ItemId,
                    orderLine.Quantity
                })
                .ToListAsync();

            var topItemIds = rawData
                .GroupBy(x => x.ItemId)
                .OrderByDescending(group => group.Sum(x => x.Quantity))
                .Take(5)
                .Select(group => group.Key)
                .ToList();

            var items = await _context.Items
                .Where(item => topItemIds.Contains(item.ItemId))
                .Include(item => item.Brand)
                .Include(item => item.Category)
                .Include(item => item.Discounts)
                .ToListAsync();

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            var result = items.Select(item =>
            {
                var discount = item.Discounts.FirstOrDefault(d => d.ItemId == item.ItemId && d.DateFrom <= today && d.DateTo >= today)
                               ?? item.Discounts.FirstOrDefault(d => d.CategoryId == item.CategoryId && d.DateFrom <= today && d.DateTo >= today);

                return new ItemCardVM()
                {
                    Id = item.ItemId,
                    ImageUrl = item.ImageUrl ?? string.Empty,
                    Name = item.Name,
                    BrandName = item.Brand.Name,
                    Price = item.Price,
                    StockStatus = StockStatus.InStock,
                    Discount = discount != null ? new DiscountVM()
                    {
                        Id = discount.DiscountId,
                        Percent = (int)discount.Percent,
                        DateFrom = discount.DateFrom,
                        DateTo = discount.DateTo,
                    } : null
                };
            }).ToList();

            return result.OrderBy(x => topItemIds.IndexOf(x.Id)).ToList();
        }
    }
}





