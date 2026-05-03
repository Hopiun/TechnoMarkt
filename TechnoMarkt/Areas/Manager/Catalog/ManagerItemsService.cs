using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnoMarkt.Data;
using TechnoMarkt.Models;

using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Extensions.Query;
using TechnoMarkt.Shared.Catalog.ViewModels;
using TechnoMarkt.Shared.Common.ViewModels.Forms;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Areas.Manager.Extensions;
using WarehouseModel = TechnoMarkt.Models.Warehouse;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    public class ManagerItemsService : IManagerItemsService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const int CRITICAL_STOCK_THRESHOLD = 5;
        private const string ITEM_IMAGES_PATH = "images/items";

        public ManagerItemsService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<(List<ItemCardVM> Items, int TotalCount)> GetItemsAsync(int? categoryId, int storeId, ManagerItemsFilter? filter)
        {
            IQueryable<Item> query = _context.Items.ByCategory(categoryId)
                .Include(item => item.Brand)
                .Include(item => item.Warehouses)
                .Include(item => item.Discounts)
                .Include(item => item.Category)
                    .ThenInclude(category => category.Discounts);

            if (filter != null)
            {
                query = query
                    .SearchByName(filter.Name)
                    .BySupplier(filter.SupplierId);

                query = filter.SortAscending
                    ? query.SortByAscending(filter.SortBy)
                    : query.SortByDescending(filter.SortBy);
            }
            else
            {
                filter = new ManagerItemsFilter();
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
                StockQuantity = item.Warehouses.FirstOrDefault(warehouse => warehouse.StoreId == storeId)?.Quantity ?? 0,
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
                StockQuantity = item.Warehouses.FirstOrDefault(warehouse => warehouse.StoreId == storeId)?.Quantity ?? 0,
                StockStatus = GetStockStatus(item, storeId),
                Discount = GetDiscount(item)
            };
        }

        private StockStatus GetStockStatus(Item item, int storeId)
        {
            int? stockQuantity = item.Warehouses.FirstOrDefault(warehouse => warehouse.StoreId == storeId)?.Quantity;

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

        public async Task<List<ItemRowDto>> GetTopItemsAsync(int storeId, MonthYearFilter filter)
        {
            DateRange month = DateRange.FromMonthYear(filter.Month, filter.Year);

            var rawData = await _context.OrderLines.ByStore(storeId).InPeriod(month).Completed()
                .Select(orderLine => new
                {
                    orderLine.ItemId,
                    ItemName = orderLine.Item.Name,
                    CategoryName = orderLine.Item.Category.Name,
                    orderLine.PriceAtMoment,
                    orderLine.Quantity
                })
                .ToListAsync();

            return rawData
                .GroupBy(x => new
                {
                    x.ItemId,
                    x.ItemName,
                    x.CategoryName
                })
                .Select(group => new ItemRowDto(
                    group.Key.ItemId,
                    group.Key.ItemName,
                    group.Key.CategoryName,
                    group.First().PriceAtMoment,
                    group.Sum(x => x.Quantity),
                    group.Sum(x => x.Quantity * x.PriceAtMoment)
                ))
                .OrderByDescending(x => x.SoldQuantity)
                .Take(10)
                .ToList();
        }

        public async Task<ItemFormVM?> GetItemFormAsync(int? itemId)
        {
            List<SelectListItem> categories = await _context.Categories
                .Select(category => new SelectListItem()
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.Name
                })
                .ToListAsync();

            List<SelectListItem> suppliers = await _context.Suppliers
                .Select(supplier => new SelectListItem()
                {
                    Value = supplier.SupplierId.ToString(),
                    Text = supplier.Name
                })
                .ToListAsync();

            List<SelectListItem> brands = await _context.Brands
                .Select(brand => new SelectListItem()
                {
                    Value = brand.BrandId.ToString(),
                    Text = brand.Name
                })
                .ToListAsync();

            if (!itemId.HasValue)
                return new ItemFormVM()
                {
                    Action = FormAction.Add,
                    Categories = new SelectList(categories, "Value", "Text"),
                    Suppliers = new SelectList(suppliers, "Value", "Text"),
                    Brands = new SelectList(brands, "Value", "Text")
                };

            Item? item = await _context.Items.FindAsync(itemId);
            if (item == null) return null;

            return new ItemFormVM()
            {
                Action = FormAction.Edit,
                Id = item.ItemId,
                Name = item.Name,
                CategoryId = item.CategoryId,
                BrandId = item.BrandId,
                SupplierId = item.SupplierId,
                Price = item.Price,
                Description = item.Description,
                Weight = item.Weight,
                Dimensions = item.Dimensions,
                CurrentImageUrl = item.ImageUrl,
                Categories = new SelectList(categories, "Value", "Text", item.CategoryId),
                Suppliers = new SelectList(suppliers, "Value", "Text", item.SupplierId),
                Brands = new SelectList(brands, "Value", "Text", item.BrandId)
            };
        }

        public async Task<(bool Succeeded, string NotificationMessage)> AddItemAsync(ItemFormVM newItem, int storeId)
        {
            bool nameExists = await _context.Items
                .AnyAsync(item => item.CategoryId == newItem.CategoryId
                                  && item.Name.ToLower() == newItem.Name.ToLower());

            if (nameExists)
                return (false, $"Товар з назвою '{newItem.Name}' вже існує в цій категорії.");

            try
            {
                string? imageUrl = null;
                if (newItem.Image != null)
                    imageUrl = await SaveItemImageAsync(newItem.Image);

                Item item = new Item()
                {
                    Name = newItem.Name,
                    CategoryId = newItem.CategoryId!.Value,
                    BrandId = newItem.BrandId!.Value,
                    SupplierId = newItem.SupplierId!.Value,
                    Price = newItem.Price,
                    Description = newItem.Description,
                    Weight = newItem.Weight,
                    Dimensions = newItem.Dimensions,
                    ImageUrl = imageUrl
                };

                await using var transaction = await _context.Database.BeginTransactionAsync();

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                var allStoreIds = await _context.Stores.Select(s => s.StoreId).ToListAsync();
                foreach (var sId in allStoreIds)
                {
                    _context.Warehouses.Add(new WarehouseModel()
                    {
                        ItemId = item.ItemId,
                        StoreId = sId,
                        Quantity = 0,
                        SupplyDate = DateOnly.FromDateTime(DateTime.Now)
                    });
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (true, $"Товар '{item.Name}' (ID: {item.ItemId}) успішно додано.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час додавання товару.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> UpdateItemAsync(ItemFormVM updatedItem)
        {
            Item? item = await _context.Items.FindAsync(updatedItem.Id);
            if (item == null)
                return (false, $"Товар з ID:{updatedItem.Id} не знайдено.");

            bool nameExists = await _context.Items
                .AnyAsync(item => item.ItemId != updatedItem.Id &&
                               item.CategoryId == updatedItem.CategoryId &&
                               item.Name.ToLower() == updatedItem.Name.ToLower());

            if (nameExists)
                return (false, $"Товар з назвою '{updatedItem.Name}' вже існує в цій категорії.");

            try
            {
                if (updatedItem.Image != null)
                {
                    DeleteItemImage(item.ImageUrl);
                    item.ImageUrl = await SaveItemImageAsync(updatedItem.Image);
                }

                item.Name = updatedItem.Name;
                item.CategoryId = updatedItem.CategoryId!.Value;
                item.BrandId = updatedItem.BrandId!.Value;
                item.SupplierId = updatedItem.SupplierId!.Value;
                item.Price = updatedItem.Price;
                item.Description = updatedItem.Description;
                item.Weight = updatedItem.Weight;
                item.Dimensions = updatedItem.Dimensions;

                _context.Items.Update(item);
                await _context.SaveChangesAsync();

                return (true, $"Товар '{item.Name}' (ID: {item.ItemId}) успішно оновлено.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час оновлення товару.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> DeleteItemAsync(int itemId)
        {
            Item? item = await _context.Items
                .Include(item => item.Warehouses)
                .FirstOrDefaultAsync(item => item.ItemId == itemId);

            if (item == null)
                return (false, $"Товар з ID:{itemId} не знайдено.");

            bool hasActiveOrders = await _context.OrderLines
                .AnyAsync(orderLine => orderLine.ItemId == itemId &&
                                orderLine.Order.Status != OrderStatus.Completed &&
                                orderLine.Order.Status != OrderStatus.Cancelled &&
                                orderLine.Order.Status != OrderStatus.Returned);

            if (hasActiveOrders)
                return (false, $"Неможливо видалити '{item.Name}': товар присутній в активних замовленнях.");

            try
            {
                DeleteItemImage(item.ImageUrl);

                if (item.Warehouses.Any())
                {
                    _context.Warehouses.RemoveRange(item.Warehouses);
                }

                _context.Items.Remove(item);
                await _context.SaveChangesAsync();

                return (true, $"Товар '{item.Name}' (ID: {itemId}) успішно видалено.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час видалення товару.");
            }
        }

        private async Task<string> SaveItemImageAsync(IFormFile image)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            string folderPath = Path.Combine(_env.WebRootPath, ITEM_IMAGES_PATH);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);

            await using FileStream stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"/{ITEM_IMAGES_PATH}/{fileName}";
        }

        private void DeleteItemImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            string fullPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}


