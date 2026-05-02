using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Areas.Client.Profile.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Identity;

namespace TechnoMarkt.Areas.Client.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProfileService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ClientProfileViewModel?> GetProfileAsync(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client == null)
                return null;

            var user = await _userManager.FindByIdAsync(client.UserId.ToString());
            if (user == null)
                return null;

            return new ClientProfileViewModel
            {
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = user.Email ?? string.Empty,
                WalletBalance = client.WalletBalance
            };
        }

        public async Task<(bool Succeeded, string Message)> UpdateProfileAsync(int clientId, string firstName, string lastName)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client == null)
            {
                return (false, "РљР»С–С”РЅС‚Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            client.FirstName = firstName;
            client.LastName = lastName;
            await _context.SaveChangesAsync();

            return (true, "РџСЂРѕС„С–Р»СЊ СѓСЃРїС–СЃРЅРѕ РѕРЅРѕРІР»РµРЅРѕ.");
        }

        public async Task<(bool Succeeded, string Message)> ChangeEmailAsync(int clientId, string newEmail, string currentPassword)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client == null)
            {
                return (false, "РљР»С–С”РЅС‚Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            var user = await _userManager.FindByIdAsync(client.UserId.ToString());
            if (user == null)
            {
                return (false, "РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            if (!await _userManager.CheckPasswordAsync(user, currentPassword))
            {
                return (false, "РќРµРІС–СЂРЅРёР№ РїРѕС‚РѕС‡РЅРёР№ РїР°СЂРѕР»СЊ.");
            }

            var existingUser = await _userManager.FindByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return (false, "Р’РєР°Р·Р°РЅРёР№ Email РІР¶Рµ РІРёРєРѕСЂРёСЃС‚РѕРІСѓС”С‚СЊСЃСЏ.");
            }

            var result = await _userManager.SetEmailAsync(user, newEmail);
            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            user.UserName = newEmail;
            await _userManager.UpdateAsync(user);

            return (true, "Email СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ.");
        }

        public async Task<(bool Succeeded, string Message)> ChangePasswordAsync(int clientId, string currentPassword, string newPassword)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client == null)
            {
                return (false, "РљР»С–С”РЅС‚Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            var user = await _userManager.FindByIdAsync(client.UserId.ToString());
            if (user == null)
            {
                return (false, "РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return (false, string.Join(", ", errors));
            }

            return (true, "РџР°СЂРѕР»СЊ СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ.");
        }

        public async Task<(bool Succeeded, string Message, bool HasActiveOrders)> DeleteAccountAsync(int clientId)
        {
            var client = await _context.Clients
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client == null)
            {
                return (false, "РљР»С–С”РЅС‚Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.", false);
            }

            var hasActiveOrders = client.Orders.Any(o => 
                o.Status != OrderStatus.Completed && 
                o.Status != OrderStatus.Cancelled && 
                o.Status != OrderStatus.Returned);

            if (hasActiveOrders)
            {
                return (false, "РќРµРјРѕР¶Р»РёРІРѕ РІРёРґР°Р»РёС‚Рё Р°РєР°СѓРЅС‚, РїРѕРєРё С” Р°РєС‚РёРІРЅС– Р·Р°РјРѕРІР»РµРЅРЅСЏ.", true);
            }

            var user = await _userManager.FindByIdAsync(client.UserId.ToString());
            if (user == null)
            {
                return (false, "РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.", false);
            }

            client.FirstName = "Deleted";
            client.LastName = "User";

            var hasCompletedOrders = client.Orders.Any(o =>
                o.Status == OrderStatus.Completed || 
                o.Status == OrderStatus.Returned);

            if (!hasCompletedOrders)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return (false, "РџРѕРјРёР»РєР° РїСЂРё РІРёРґР°Р»РµРЅРЅС– Р°РєР°СѓРЅС‚Р°.", false);
            }

            return (true, "РђРєР°СѓРЅС‚ СѓСЃРїС–С€РЅРѕ РІРёРґР°Р»РµРЅРѕ.", false);
        }
    }
}




