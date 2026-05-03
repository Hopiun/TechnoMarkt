using Microsoft.AspNetCore.Identity;
using TechnoMarkt.Data;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Identity;
using TechnoMarkt.Shared.Auth.ViewModels;
using System.Security.Claims;

namespace TechnoMarkt.Controllers
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEventLogsService _eventLogsService;

        public AccountService(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEventLogsService eventLogsService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _eventLogsService = eventLogsService;
        }

        public async Task<(bool Succeeded, string? Error, string? Role)> LoginAsync(LoginVM form)
        {
            AppUser? user = await _userManager.FindByEmailAsync(form.Email);
            if (user == null)
                return (false, $"Акаунт з email {form.Email} не знайдено", null);

            SignInResult result;
            try
            {
                result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, isPersistent: false, lockoutOnFailure: false);
            }
            catch (FormatException)
            {
                return (false, "Акаунт ще не активовано. Зверніться до адміністратора.", null);
            }

            if (!result.Succeeded)
                return (false, "Невірний email або пароль", null);

            await _eventLogsService.LogAsync("Login", "User", user.Id, $"Користувач {form.Email} увійшов у систему");

            var claims = await _userManager.GetClaimsAsync(user);
            string? role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
            {
                var roles = await _userManager.GetRolesAsync(user);
                role = roles.FirstOrDefault();
            }

            if (role == "Client")
            {
                var client = _context.Clients.FirstOrDefault(c => c.UserId == user.Id);
                if (client != null)
                {
                    var oldBalance = claims.FirstOrDefault(c => c.Type == "WalletBalance");
                    if (oldBalance != null) await _userManager.RemoveClaimAsync(user, oldBalance);
                    var oldFirst = claims.FirstOrDefault(c => c.Type == "FirstName");
                    if (oldFirst != null) await _userManager.RemoveClaimAsync(user, oldFirst);
                    var oldLast = claims.FirstOrDefault(c => c.Type == "LastName");
                    if (oldLast != null) await _userManager.RemoveClaimAsync(user, oldLast);

                    await _userManager.AddClaimsAsync(user, new[]
                    {
                        new Claim("WalletBalance", client.WalletBalance.ToString("N0")),
                        new Claim("FirstName", client.FirstName),
                        new Claim("LastName", client.LastName)
                    });
                    await _signInManager.RefreshSignInAsync(user);
                }
            }

            return (true, null, role);
        }

        public async Task<(bool Succeeded, string? Error)> SignUpAsync(SignUpVM form)
        {
            AppUser? existingUser = await _userManager.FindByEmailAsync(form.Email);
            if (existingUser != null)
                return (false, "Цей email вже використовується");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            AppUser user = new()
            {
                UserName = form.Email,
                Email = form.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, form.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            Client client = new()
            {
                UserId = user.Id,
                FirstName = form.FirstName,
                LastName = form.LastName,
                WalletBalance = 0m
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim(ClaimTypes.Role, "Client"),
                new Claim("ClientId", client.ClientId.ToString()),
                new Claim("WalletBalance", "0"),
                new Claim("FirstName", client.FirstName),
                new Claim("LastName", client.LastName)
            });

            await transaction.CommitAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);
            await _eventLogsService.LogAsync("Create", "Client", client.ClientId, $"Зареєстровано нового клієнта: {form.Email}");

            return (true, null);
        }

        public async Task<string?> LogoutAsync(ClaimsPrincipal user)
        {
            var userEmail = user.Identity?.Name;
            await _signInManager.SignOutAsync();
            if (userEmail != null)
                await _eventLogsService.LogAsync("Logout", "User", $"Користувач {userEmail} вийшов із системи");
            return userEmail;
        }
    }
}
