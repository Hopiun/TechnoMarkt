using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("OrderLine")]
public partial class OrderLine
{
    [Key]
    [Column("order_line_id")]
    public int OrderLineId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price_at_moment", TypeName = "decimal(10, 2)")]
    public decimal PriceAtMoment { get; set; }

    [ForeignKey("ItemId")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;
}
