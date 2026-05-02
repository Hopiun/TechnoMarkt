namespace TechnoMarkt.StressTests;

internal static class StressBudgetPolicy
{
    public static bool Enforce =>
        string.Equals(Environment.GetEnvironmentVariable("STRESS_ENFORCE_BUDGET"), "true", StringComparison.OrdinalIgnoreCase);
}
