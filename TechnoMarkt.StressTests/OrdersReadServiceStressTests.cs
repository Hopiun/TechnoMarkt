using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;

namespace TechnoMarkt.StressTests;

public class OrdersReadServiceStressTests
{
    [Fact]
    [Trait("Category", "Stress")]
    public async Task GetOrdersAsync_ShouldHandleLargeDatasetFastEnough()
    {
        await using var context = StressDataFactory.CreateSeededContext(
            databaseName: $"orders-read-{Guid.NewGuid():N}",
            ordersCount: StressTestSettings.OrdersCount,
            warehousesCount: StressTestSettings.WarehousesCount);

        var service = new OrdersReadService(context);
        var filter = new OrdersFilter
        {
            Search = "Name",
            DateFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            DateTo = DateOnly.FromDateTime(DateTime.UtcNow),
            PaidOnly = true
        };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var rows = await service.GetOrdersAsync(1, filter);
        sw.Stop();

        Assert.NotNull(rows);
        if (StressBudgetPolicy.Enforce)
        {
            Assert.True(sw.ElapsedMilliseconds < StressTestSettings.MaxReadMs,
                $"Orders query exceeded budget: {sw.ElapsedMilliseconds}ms > {StressTestSettings.MaxReadMs}ms");
        }
    }

    [Fact]
    [Trait("Category", "Stress")]
    public async Task GetOrdersAsync_ShouldSupportConcurrentUsersWithoutFailures()
    {
        var exceptions = new List<Exception>();
        var users = Enumerable.Range(1, StressTestSettings.ConcurrentUsers);

        await Parallel.ForEachAsync(users, async (_, _) =>
        {
            try
            {
                await using var context = StressDataFactory.CreateSeededContext(
                    databaseName: $"orders-concurrent-{Guid.NewGuid():N}",
                    ordersCount: StressTestSettings.OrdersCount / 2,
                    warehousesCount: StressTestSettings.WarehousesCount / 2);

                var service = new OrdersReadService(context);
                await service.GetOrdersAsync(1, new OrdersFilter { Status = TechnoMarkt.Models.OrderStatus.Completed });
            }
            catch (Exception ex)
            {
                lock (exceptions)
                    exceptions.Add(ex);
            }
        });

        Assert.Empty(exceptions);
    }
}
