using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;

namespace TechnoMarkt.StressTests;

internal static class StressDataFactory
{
    public static AppDbContext CreateSeededContext(string databaseName, int ordersCount, int warehousesCount)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        var context = new AppDbContext(options);
        Seed(context, ordersCount, warehousesCount);
        return context;
    }

    private static void Seed(AppDbContext context, int ordersCount, int warehousesCount)
    {
        if (context.Orders.Any() || context.Warehouses.Any())
            return;

        const int storeId = 1;
        const int clientsCount = 1000;
        const int categoriesCount = 30;
        const int itemsCount = 2500;

        var categoryList = Enumerable.Range(1, categoriesCount)
            .Select(i => new Category { CategoryId = i, Name = $"Category {i}" })
            .ToList();
        context.Categories.AddRange(categoryList);

        var brand = new Brand { BrandId = 1, Name = "StressBrand" };
        var supplier = new Supplier { SupplierId = 1, Name = "StressSupplier" };
        context.Brands.Add(brand);
        context.Suppliers.Add(supplier);

        var store = new Store { StoreId = storeId, CityId = 1, Address = "Stress Address" };
        context.Stores.Add(store);

        var clients = Enumerable.Range(1, clientsCount)
            .Select(i => new Client
            {
                ClientId = i,
                UserId = i,
                FirstName = $"Name{i}",
                LastName = $"Surname{i}",
                WalletBalance = 10000m
            })
            .ToList();
        context.Clients.AddRange(clients);

        var items = Enumerable.Range(1, itemsCount)
            .Select(i =>
            {
                var category = categoryList[(i - 1) % categoriesCount];
                return new Item
                {
                    ItemId = i,
                    Name = $"Item {i}",
                    Price = 100 + (i % 700),
                    CategoryId = category.CategoryId,
                    Category = category,
                    BrandId = brand.BrandId,
                    Brand = brand,
                    SupplierId = supplier.SupplierId,
                    Supplier = supplier
                };
            })
            .ToList();
        context.Items.AddRange(items);

        var warehouses = Enumerable.Range(1, warehousesCount)
            .Select(i =>
            {
                var item = items[(i - 1) % itemsCount];
                return new Warehouse
                {
                    WarehouseId = i,
                    StoreId = storeId,
                    Store = store,
                    ItemId = item.ItemId,
                    Item = item,
                    Quantity = 10 + (i % 120),
                    SupplyDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-(i % 60)))
                };
            })
            .ToList();
        context.Warehouses.AddRange(warehouses);

        var orders = Enumerable.Range(1, ordersCount)
            .Select(i =>
            {
                var client = clients[(i - 1) % clientsCount];
                var status = (i % 5) switch
                {
                    0 => OrderStatus.Completed,
                    1 => OrderStatus.New,
                    2 => OrderStatus.Processing,
                    3 => OrderStatus.Ready,
                    _ => OrderStatus.Cancelled
                };

                return new Order
                {
                    OrderId = i,
                    StoreId = storeId,
                    Store = store,
                    ClientId = client.ClientId,
                    Client = client,
                    OrderDate = DateTime.UtcNow.AddMinutes(-i),
                    Status = status,
                    OrderTotal = 500 + (i % 3000)
                };
            })
            .ToList();
        context.Orders.AddRange(orders);

        var payments = orders.Select(order => new Payment
        {
            PaymentId = order.OrderId,
            OrderId = order.OrderId,
            Order = order,
            Amount = order.OrderTotal,
            Date = order.OrderDate.AddMinutes(10),
            Method = order.OrderId % 2 == 0 ? PayMethod.Card : PayMethod.Cash,
            Status = order.Status == OrderStatus.Completed ? PaymentStatus.Completed : PaymentStatus.Pending
        });
        context.Payments.AddRange(payments);

        context.SaveChanges();
    }
}
