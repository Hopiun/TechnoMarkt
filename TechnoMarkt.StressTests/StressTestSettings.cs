namespace TechnoMarkt.StressTests;

internal static class StressTestSettings
{
    public static int OrdersCount => GetInt("STRESS_ORDERS_COUNT", 15000);
    public static int WarehousesCount => GetInt("STRESS_WAREHOUSES_COUNT", 8000);
    public static int ConcurrentUsers => GetInt("STRESS_CONCURRENT_USERS", 50);
    public static int MaxReadMs => GetInt("STRESS_MAX_READ_MS", 30000);

    private static int GetInt(string key, int defaultValue)
    {
        var raw = Environment.GetEnvironmentVariable(key);
        return int.TryParse(raw, out var parsed) ? parsed : defaultValue;
    }
}
