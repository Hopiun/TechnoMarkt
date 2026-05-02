using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Client.Wallet
{
    public class WalletService : IWalletService
    {
        private readonly AppDbContext _context;

        public WalletService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethodsAsync(int clientId)
        {
            return await _context.PaymentMethods
                .Where(p => p.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<(bool Succeeded, string Message)> AddPaymentMethodAsync(int clientId, string bankName, string cardNumber, string cardType)
        {
            if (await _context.PaymentMethods.AnyAsync(p => p.CardNumber == cardNumber))
            {
                return (false, "Картка з таким номером вже існує.");
            }

            var method = new PaymentMethod
            {
                ClientId = clientId,
                BankName = bankName,
                CardNumber = cardNumber,
                CardType = cardType
            };

            _context.PaymentMethods.Add(method);
            await _context.SaveChangesAsync();

            return (true, "Картку успішно додано.");
        }

        public async Task<(bool Succeeded, string Message)> DeletePaymentMethodAsync(int clientId, int paymentMethodId)
        {
            var method = await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.PaymentMethodId == paymentMethodId && p.ClientId == clientId);

            if (method == null)
            {
                return (false, "Картку не знайдено.");
            }

            var payments = await _context.Payments.Where(p => p.PaymentMethodId == paymentMethodId).ToListAsync();
            foreach (var payment in payments)
            {
                payment.PaymentMethodId = null;
            }

            _context.PaymentMethods.Remove(method);
            await _context.SaveChangesAsync();

            return (true, "Картку успішно видалено.");
        }

        public async Task<(bool Succeeded, decimal NewBalance, string Message)> TopUpWalletAsync(int clientId, decimal amount)
        {
            if (amount <= 0)
            {
                return (false, 0, "Сума поповнення має бути більшою за нуль.");
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (client == null)
            {
                return (false, 0, "Клієнта не знайдено.");
            }

            client.WalletBalance += amount;
            await _context.SaveChangesAsync();

            return (true, client.WalletBalance, $"Бонусний рахунок успішно поповнено на ₴{amount:N0}.");
        }

        public async Task<decimal> GetWalletBalanceAsync(int clientId)
        {
            return await _context.Clients
                .Where(c => c.ClientId == clientId)
                .Select(c => c.WalletBalance)
                .FirstOrDefaultAsync();
        }
    }
}

