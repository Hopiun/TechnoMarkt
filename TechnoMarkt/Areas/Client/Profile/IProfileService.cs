using TechnoMarkt.Areas.Client.Profile.ViewModels;

namespace TechnoMarkt.Areas.Client.Profile
{
    public interface IProfileService
    {
        Task<ClientProfileViewModel?> GetProfileAsync(int clientId);
        Task<(bool Succeeded, string Message)> UpdateProfileAsync(int clientId, string firstName, string lastName);
        Task<(bool Succeeded, string Message)> ChangeEmailAsync(int clientId, string newEmail, string currentPassword);
        Task<(bool Succeeded, string Message)> ChangePasswordAsync(int clientId, string currentPassword, string newPassword);
        Task<(bool Succeeded, string Message, bool HasActiveOrders)> DeleteAccountAsync(int clientId);
    }
}
