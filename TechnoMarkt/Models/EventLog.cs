using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("EventLog")]
[Index(nameof(CreatedAt))]
[Index(nameof(Role))]
[Index(nameof(Action))]
[Index(nameof(Entity))]
public partial class EventLog
{
    [Key]
    [Column("log_id")]
    public int LogId { get; set; }

    [Column("user_id")]
    [StringLength(450)]
    public string? UserId { get; set; }

    [Column("role")]
    [StringLength(20)]
    public string? Role { get; set; }

    [Column("action")]
    [StringLength(50)]
    public string Action { get; set; } = null!;

    [Column("entity")]
    [StringLength(50)]
    public string Entity { get; set; } = null!;

    [Column("entity_id")]
    public int? EntityId { get; set; }

    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
