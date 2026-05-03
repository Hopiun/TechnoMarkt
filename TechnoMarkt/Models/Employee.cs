using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechnoMarkt.Models.Identity;

namespace TechnoMarkt.Models;

public enum EmployeeRole
{
    Operator,
    Manager,
    Administrator
}

public enum EmployeeStatus
{
    Present,
    Absent,
    OnLeave,
    Former
}

[Table("Employee")]
public partial class Employee
{
    [Key]
    [Column("employee_id")]
    public int EmployeeId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Column("role")]
    [StringLength(20)]
    public EmployeeRole Role { get; set; }

    [StringLength(10)]
    public EmployeeStatus Status { get; set; }

    [Column("salary", TypeName = "decimal(10, 2)")]
    public decimal? Salary { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [ForeignKey("StoreId")]
    public virtual Store Store { get; set; } = null!;
    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
