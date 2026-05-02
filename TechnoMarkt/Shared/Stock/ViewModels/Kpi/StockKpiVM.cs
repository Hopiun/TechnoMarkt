using TechnoMarkt.Models;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;
using TechnoMarkt.Shared.EventLogs.ViewModels.Tables;
using TechnoMarkt.Shared.Transactions.ViewModels.Tables;
using TechnoMarkt.Shared.Common.ViewModels.Tables;

namespace TechnoMarkt.Shared.Stock.ViewModels.Kpi
{
    public class StockKpiVM
    {
        public WarehouseKpiDto Kpi { get; set; } = null!;
        public WarehouseKpiFilter KpiFilter { get; set; } = new();
        public StockTableVM StockTable { get; set; } = new();
    }

    public record WarehouseKpiDto(decimal TotalStock, int TotalPositionsCount, int CriticalStockCount, int StaleSupplyCount);

    public class WarehouseKpiFilter
    {
        public int CriticalStockThreshold { get; set; } = 5;
        public int NotSuppliedDays { get; set; } = 90;
    }
}




