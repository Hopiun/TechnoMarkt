using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Store")]
public partial class Store
{
    [Key]
    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("city_id")]
    public int CityId { get; set; }

    [Column("admin_id")]
    public int? AdminId { get; set; }

    [Column("address")]
    [StringLength(200)]
    public string Address { get; set; } = null!;

    [Column("phone")]
    [StringLength(13)]
    public string? Phone { get; set; }

    [ForeignKey("AdminId")]
    public virtual Employee? Admin { get; set; }
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
