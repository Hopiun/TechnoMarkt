using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Country")]
public partial class Country
{
    [Key]
    [Column("country_id")]
    public int CountryId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}
