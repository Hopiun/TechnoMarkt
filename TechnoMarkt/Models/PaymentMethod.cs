using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("PaymentMethod")]
[Index("CardNumber", Name = "UQ_PaymentMethod_Card", IsUnique = true)]
public partial class PaymentMethod
{
    [Key]
    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("bank_name")]
    [StringLength(100)]
    public string BankName { get; set; } = null!;

    [Column("card_number")]
    [StringLength(16)]
    [Unicode(false)]
    public string CardNumber { get; set; } = null!;

    [Column("card_type")]
    [StringLength(10)]
    [Unicode(false)]
    public string CardType { get; set; } = null!;

    [ForeignKey("ClientId")]
    public virtual Client Client { get; set; } = null!;
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
