using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Brand")]
public partial class Brand
{
    [Key]
    [Column("brand_id")]
    public int BrandId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("country_id")]
    public int? CountryId { get; set; }

    [ForeignKey("CountryId")]
    public virtual Country? Country { get; set; }
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
