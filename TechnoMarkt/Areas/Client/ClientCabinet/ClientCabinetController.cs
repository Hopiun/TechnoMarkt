using TechnoMarkt.Data;
using TechnoMarkt.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Areas.Client.ClientCabinet
{
    [Area("Client")]
    [Authorize(Roles = "Client")]
    public abstract class ClientCabinetController : Controller
    {
        protected readonly AppDbContext _context;

        protected int ClientId
        {
            get
            {
                var claim = User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;
                return claim != null ? int.Parse(claim) : 0;
            }
        }

        public ClientCabinetController(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var clientId = ClientId;
            if (clientId != 0)
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
                if (client != null)
                {
                    ViewData["ClientFullName"] = $"{client.LastName} {client.FirstName}";
                    ViewData["ClientEmail"] = User.Identity?.Name;
                    ViewData["ClientWallet"] = client.WalletBalance.ToString("N0");
                }
            }

            await next();
        }
    }
}

