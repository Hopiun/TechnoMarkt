using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Shared.Stock.ViewModels.Tables;

namespace TechnoMarkt.StressTests;

public class StockReadServiceStressTests
{
    [Fact]
    [Trait("Category", "Stress")]
    public async Task GetStockItemsAsync_ShouldFilterLargeDatasetFastEnough()
    {
        await using var context = StressDataFactory.CreateSeededContext(
            databaseName: $"stock-read-{Guid.NewGuid():N}",
            ordersCount: StressTestSettings.OrdersCount,
            warehousesCount: StressTestSettings.WarehousesCount);

        var service = new StockReadService(context);
        var filter = new StockFilter
        {
            Name = "Item",
            CriticalStockThreshold = 20,
            NotSuppliedDays = 15,
            SortBy = StockSortBy.Quantity,
            SortDescending = true
        };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var rows = await service.GetStockItemsAsync(1, filter);
        sw.Stop();

        Assert.NotNull(rows);
        if (StressBudgetPolicy.Enforce)
        {
            Assert.True(sw.ElapsedMilliseconds < StressTestSettings.MaxReadMs,
                $"Stock query exceeded budget: {sw.ElapsedMilliseconds}ms > {StressTestSettings.MaxReadMs}ms");
        }
    }
}
