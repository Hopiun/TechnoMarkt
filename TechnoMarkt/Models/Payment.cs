using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

public enum PayMethod
{
    Online,
    Cash,
    Card
}

public enum PaymentStatus
{
    Pending,
    Failed,
    Completed,
    Refunded
}

[Table("Payment")]
public partial class Payment
{
    [Key]
    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("payment_method_id")]
    public int? PaymentMethodId { get; set; }

    [Column("amount", TypeName = "decimal(12, 2)")]
    public decimal Amount { get; set; }

    [Column("date", TypeName = "datetime")]
    public DateTime Date { get; set; }

    [Column("method")]
    [StringLength(10)]
    public PayMethod Method { get; set; }

    [Column("status")]
    [StringLength(10)]
    public PaymentStatus Status { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod? PaymentMethod { get; set; }
}

