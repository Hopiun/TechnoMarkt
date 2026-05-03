using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("City")]
public partial class City
{
    [Key]
    [Column("city_id")]
    public int CityId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("region")]
    [StringLength(100)]
    public string Region { get; set; } = null!;
}
