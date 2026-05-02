using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TechnoMarkt.Shared.Stock.Services
{
    public class StockReadService : IStockReadService
    {
        private readonly AppDbContext _context;

        public StockReadService(AppDbContext context) => _context = context;

        public async Task<List<StockItemsTableRow>> GetStockItemsAsync(int storeId, StockFilter? filter)
        {
            IQueryable<Warehouse> query = _context.Warehouses.ByStore(storeId);

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Name))
                    query = query.Where(warehouse => warehouse.Item.Name.Contains(filter.Name));

                if (filter.CategoryId.HasValue)
                    query = query.Where(warehouse => warehouse.Item.CategoryId == filter.CategoryId.Value
                                   || warehouse.Item.Category.ParentCategoryId == filter.CategoryId.Value);

                if (filter.CriticalStockThreshold.HasValue)
                    query = query.LowStock(filter.CriticalStockThreshold.Value);

                if (filter.ZeroStockOnly)
                    query = query.OutOfStock();

                if (filter.NotSuppliedDays.HasValue)
                {
                    DateOnly staleDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-filter.NotSuppliedDays.Value));
                    query = query.Where(warehouse => warehouse.SupplyDate.HasValue && warehouse.SupplyDate.Value < staleDate);
                }

                query = filter.SortBy switch
                {
                    StockSortBy.Price => filter.SortDescending
                        ? query.OrderByDescending(warehouse => warehouse.Item.Price)
                        : query.OrderBy(warehouse => warehouse.Item.Price),
                    StockSortBy.Quantity => filter.SortDescending
                        ? query.OrderByDescending(warehouse => warehouse.Quantity)
                        : query.OrderBy(warehouse => warehouse.Quantity),
                    StockSortBy.LastSupplyDate => filter.SortDescending
                        ? query.OrderByDescending(warehouse => warehouse.SupplyDate)
                        : query.OrderBy(warehouse => warehouse.SupplyDate),
                    _ => filter.SortDescending
                        ? query.OrderByDescending(warehouse => warehouse.Item.Name)
                        : query.OrderBy(warehouse => warehouse.Item.Name)
                };
            }

            return await query
                .Select(warehouse => new StockItemsTableRow()
                {
                    WarehouseId = warehouse.WarehouseId,
                    ItemId = warehouse.ItemId,
                    ItemName = warehouse.Item.Name,
                    Category = warehouse.Item.Category.Name,
                    UnitPrice = warehouse.Item.Price,
                    Quantity = warehouse.Quantity,
                    SupplyDate = warehouse.SupplyDate
                })
                .ToListAsync();
        }
    }
}
