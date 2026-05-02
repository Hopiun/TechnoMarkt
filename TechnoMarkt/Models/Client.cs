using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechnoMarkt.Models.Identity;

namespace TechnoMarkt.Models;

[Table("Client")]
public partial class Client
{
    [Key]
    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("first_name")]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [Column("wallet_balance", TypeName = "decimal(10, 2)")]
    public decimal WalletBalance { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}
