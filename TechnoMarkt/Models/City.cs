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
    [Column("сity_id")]
    public int СityId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("region")]
    [StringLength(100)]
    public string Region { get; set; } = null!;
}
