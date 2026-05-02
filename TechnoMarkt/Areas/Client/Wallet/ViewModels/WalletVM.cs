using System.ComponentModel.DataAnnotations;
using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Client.Wallet.ViewModels
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    }

    public class PaymentMethodFormVM
    {
        [Required(ErrorMessage = "Поле Банк є обов'язковим")]
        [MaxLength(100, ErrorMessage = "Максимальна довжина 100 символів")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Поле Номер картки є обов'язковим")]
        [MinLength(16, ErrorMessage = "Номер картки має містити щонайменше 16 символів"), MaxLength(16)]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Номер картки має містити 16 цифр")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Поле Тип картки є обов'язковим")]
        public string CardType { get; set; }
    }
}


