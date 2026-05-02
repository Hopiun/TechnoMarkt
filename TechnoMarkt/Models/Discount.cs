using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Discount")]
public partial class Discount
{
    [Key]
    [Column("discount_id")]
    public int DiscountId { get; set; }

    [Column("item_id")]
    public int? ItemId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("percent")]
    public int Percent { get; set; }

    [Column("date_from")]
    public DateOnly DateFrom { get; set; }

    [Column("date_to")]
    public DateOnly DateTo { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    [ForeignKey("ItemId")]
    public virtual Item? Item { get; set; }
}
