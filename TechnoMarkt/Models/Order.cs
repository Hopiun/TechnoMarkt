using TechnoMarkt.Shared.Orders.Extensions;
using TechnoMarkt.Shared.Transactions.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

public enum OrderStatus
{
    New,
    Processing,
    Ready,
    Completed,
    Returned,
    Cancelled
}

[Table("Order")]
public partial class Order
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("operator_id")]
    public int? OperatorId { get; set; }

    [Column("order_date", TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [Column("delivery_date")]
    public DateOnly? DeliveryDate { get; set; }

    [Column("status")]
    [StringLength(20)]
    public OrderStatus Status { get; set; }

    [Column("order_total", TypeName = "decimal(12, 2)")]
    public decimal OrderTotal { get; set; }

    [ForeignKey("ClientId")]
    public virtual Client Client { get; set; } = null!;

    [ForeignKey("OperatorId")]
    public virtual Employee? Operator { get; set; }
    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("StoreId")]
    public virtual Store Store { get; set; } = null!;
}

