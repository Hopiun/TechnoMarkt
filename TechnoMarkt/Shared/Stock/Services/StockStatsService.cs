using TechnoMarkt.Shared.Stock.ViewModels.Kpi;
using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TechnoMarkt.Shared.Stock.Services
{
    public class StockStatsService : IStockStatsService
    {
        private readonly AppDbContext _context;

        public StockStatsService(AppDbContext context) => _context = context;

        public async Task<WarehouseKpiDto> GetKpiAsync(int storeId, WarehouseKpiFilter filter)
        {
            DateOnly staleDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-filter.NotSuppliedDays));

            var items = await _context.Warehouses.ByStore(storeId)
                .Select(warehouse => new
                {
                    warehouse.Quantity,
                    UnitPrice = warehouse.Item.Price,
                    warehouse.SupplyDate
                })
                .ToListAsync();

            decimal totalStock = items.Sum(warehouse => warehouse.Quantity * warehouse.UnitPrice);
            int inStockCount = items.Count(warehouse => warehouse.Quantity > 0);
            int criticalStockCount = items.Count(warehouse => warehouse.Quantity > 0 && warehouse.Quantity < filter.CriticalStockThreshold);
            int staleSupplyCount = items.Count(warehouse => warehouse.SupplyDate.HasValue && warehouse.SupplyDate.Value < staleDate);

            return new WarehouseKpiDto(totalStock, inStockCount, criticalStockCount, staleSupplyCount);
        }
    }
}
