using TechnoMarkt.Shared.Stock.Extensions;
using TechnoMarkt.Models;
using TechnoMarkt.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace TechnoMarkt.Shared.Stock.Services
{
    public class StockEditService : IStockEditService
    {
        private readonly AppDbContext _context;

        public StockEditService(AppDbContext context) => _context = context;

        public async Task<(bool Succeeded, string? NotificationText)> RestockItemAsync(int warehouseId, int storeId, int addedQuantity)
        {
            Warehouse? warehouse = await _context.Warehouses.ByStore(storeId)
                .FirstOrDefaultAsync(warehouse => warehouse.WarehouseId == warehouseId);
            if (warehouse == null)
                return (false, $"Запис позиції на складі з ID:{warehouseId} не знайдено");

            warehouse.Quantity += addedQuantity;
            warehouse.SupplyDate = DateOnly.FromDateTime(DateTime.Now);

            await _context.SaveChangesAsync();
            return (true, $"Поставка успішно оформлена. Поточний залишок: {warehouse.Quantity}");
        }

        public async Task<(bool Succeeded, string? NotificationText)> AdjustInventoryAsync(int warehouseId, int storeId, int exactQuantity)
        {
            Warehouse? warehouse = await _context.Warehouses.ByStore(storeId)
                .FirstOrDefaultAsync(warehouse => warehouse.WarehouseId == warehouseId);
            if (warehouse == null)
                return (false, $"Запис позиції на складі з ID:{warehouseId} не знайдено");

            warehouse.Quantity = exactQuantity;

            await _context.SaveChangesAsync();
            return (true, $"Залишок позиції складу з ID:{warehouseId} успішно відкориговано на {exactQuantity} од.");
        }
    }
}