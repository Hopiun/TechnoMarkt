using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Supplier")]
public partial class Supplier
{
    [Key]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    [Column("name")]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Column("country_id")]
    public int? CountryId { get; set; }

    [Column("contact")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Contact { get; set; }

    [Column("rating", TypeName = "decimal(3, 2)")]
    public decimal? Rating { get; set; }

    [ForeignKey("CountryId")]
    public virtual Country? Country { get; set; }
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
