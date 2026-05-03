using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Client.Wallet
{
    public interface IWalletService
    {
        Task<List<PaymentMethod>> GetPaymentMethodsAsync(int clientId);
        Task<(bool Succeeded, string Message)> AddPaymentMethodAsync(int clientId, string bankName, string cardNumber, string cardType);
        Task<(bool Succeeded, string Message)> DeletePaymentMethodAsync(int clientId, int paymentMethodId);
        Task<(bool Succeeded, decimal NewBalance, string Message)> TopUpWalletAsync(int clientId, decimal amount);
        Task<decimal> GetWalletBalanceAsync(int clientId);
    }
}
