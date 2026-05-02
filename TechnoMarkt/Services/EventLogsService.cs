using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Models;
using Microsoft.Extensions.Logging;

namespace TechnoMarkt.Services
{
    public class EventLogsService : IEventLogsService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EventLogsService> _logger;

        public EventLogsService(
            IDbContextFactory<AppDbContext> contextFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<EventLogsService> logger)
        {
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(string action, string entity, int? entityId, string description)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                string? userId = null;
                string? role = null;

                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    role = httpContext.User.FindFirstValue(ClaimTypes.Role);
                }

                
                await using var context = await _contextFactory.CreateDbContextAsync();

                context.EventLogs.Add(new EventLog
                {
                    UserId = userId,
                    Role = role,
                    Action = action,
                    Entity = entity,
                    EntityId = entityId,
                    Description = description,
                    CreatedAt = DateTime.Now
                });

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write EventLog: {Action} {Entity} {Description}",
                    action, entity, description);
            }
        }

        public async Task LogAsync(string action, string entity, string description)
            => await LogAsync(action, entity, null, description);
    }
}

