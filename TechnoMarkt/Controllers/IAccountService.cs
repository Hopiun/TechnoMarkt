using TechnoMarkt.Shared.Auth.ViewModels;
using System.Security.Claims;

namespace TechnoMarkt.Controllers
{
    public interface IAccountService
    {
        Task<(bool Succeeded, string? Error, string? Role)> LoginAsync(LoginVM form);
        Task<(bool Succeeded, string? Error)> SignUpAsync(SignUpVM form);
        Task<string?> LogoutAsync(ClaimsPrincipal user);
    }
}
