using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Areas.Home.Catalog.ViewModels;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;

using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Extensions.Query;
using TechnoMarkt.Areas.Home.Extensions;

namespace TechnoMarkt.Areas.Home.Catalog
{
    public class HomeItemsService : IItemsReadService<HomeItemsFilter>
    {
        private readonly AppDbContext _context;
        private const int CRITICAL_STOCK_THRESHOLD = 5;

        public HomeItemsService(AppDbContext context) => _context = context;

        public async Task<(List<ItemCardVM> Items, int TotalCount)> GetItemsAsync(int? categoryId, int storeId, HomeItemsFilter? filter)
        {
            IQueryable<Item> query = _context.Items
                .ByCategory(categoryId)
                .Include(item => item.Brand)
                .Include(item => item.Warehouses)
                .Include(item => item.Discounts)
                .Include(item => item.Category)
                    .ThenInclude(category => category.Discounts);

            if (filter != null)
            {
                query = query
                    .SearchByName(filter.Name)
                    .ByBrands(filter.BrandIds)
                    .ByMinPrice(filter.MinPrice)
                    .ByMaxPrice(filter.MaxPrice);

                if (filter.InStockOnly)
                    query = query.InStock(storeId);

                if (filter.WithDiscountsOnly)
                    query = query.WithActiveDiscounts();

                query = filter.SortAscending
                    ? query.SortByAscending(filter.SortBy)
                    : query.SortByDescending(filter.SortBy);
            }
            else
            {
                filter = new HomeItemsFilter();
            }

            int totalCount = await query.CountAsync();

            List<Item> items = await query
                .Skip((filter.CurrentPage - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            List<ItemCardVM> resultItems = items.Select(item => new ItemCardVM()
            {
                Id = item.ItemId,
                ImageUrl = item.ImageUrl ?? string.Empty,
                Name = item.Name,
                BrandName = item.Brand.Name,
                Price = item.Price,
                StockQuantity = item.Warehouses.FirstOrDefault(w => w.StoreId == storeId)?.Quantity ?? 0,
                StockStatus = GetStockStatus(item, storeId),
                Discount = GetDiscount(item)
            }).ToList();

            return (resultItems, totalCount);
        }

        public async Task<ItemVM> GetItemDetailsAsync(int itemId, int storeId)
        {
            Item? item = await _context.Items
                .Include(item => item.Category)
                    .ThenInclude(category => category.Discounts)
                .Include(item => item.Brand)
                .Include(item => item.Supplier)
                .Include(item => item.Discounts)
                .Include(item => item.Warehouses)
                .FirstOrDefaultAsync(item => item.ItemId == itemId);

            if (item == null) return null!;

            return new ItemVM()
            {
                Id = item.ItemId,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                BrandName = item.Brand.Name,
                CategoryName = item.Category.Name,
                SupplierName = item.Supplier.Name,
                Description = item.Description,
                Weight = item.Weight,
                Dimensions = item.Dimensions,
                Price = item.Price,
                StockQuantity = item.Warehouses.FirstOrDefault(w => w.StoreId == storeId)?.Quantity ?? 0,
                StockStatus = GetStockStatus(item, storeId),
                Discount = GetDiscount(item)
            };
        }

        private StockStatus GetStockStatus(Item item, int storeId)
        {
            int? stockQuantity = item.Warehouses.FirstOrDefault(w => w.StoreId == storeId)?.Quantity;

            return stockQuantity switch
            {
                > CRITICAL_STOCK_THRESHOLD => StockStatus.InStock,
                > 0 and <= CRITICAL_STOCK_THRESHOLD => StockStatus.Low,
                _ => StockStatus.OutOfStock
            };
        }

        private DiscountVM? GetDiscount(Item item)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            
            Discount? discount = item.Discounts
                .FirstOrDefault(d => d.ItemId == item.ItemId && d.DateFrom <= today && d.DateTo >= today);
            
            if (discount == null && item.Category?.Discounts != null)
            {
                discount = item.Category.Discounts
                    .FirstOrDefault(d => d.CategoryId == item.CategoryId && d.DateFrom <= today && d.DateTo >= today);
            }
            
            if (discount == null)
            {
                discount = item.Discounts
                    .FirstOrDefault(d => d.CategoryId == item.CategoryId && d.DateFrom <= today && d.DateTo >= today);
            }

            return discount != null ? new DiscountVM()
            {
                Id = discount.DiscountId,
                Percent = (int)discount.Percent,
                DateFrom = discount.DateFrom,
                DateTo = discount.DateTo,
            } : null;
        }
    }
}








