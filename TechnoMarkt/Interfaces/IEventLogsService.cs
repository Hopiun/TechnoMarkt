namespace TechnoMarkt.Interfaces
{
    public interface IEventLogsService
    {
        Task LogAsync(string action, string entity, int? entityId, string description);
        Task LogAsync(string action, string entity, string description);
    }
}
