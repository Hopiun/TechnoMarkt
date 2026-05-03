using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Item")]
public partial class Item
{
    [Key]
    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("brand_id")]
    public int BrandId { get; set; }

    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price", TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column("rating")]
    public double? Rating { get; set; }

    [Column("weight")]
    public double? Weight { get; set; }

    [Column("dimensions")]
    [StringLength(50)]
    public string? Dimensions { get; set; }

    [Column("image_url")]
    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    [ForeignKey("BrandId")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    [ForeignKey("SupplierId")]
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
