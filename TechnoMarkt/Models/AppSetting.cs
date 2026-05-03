using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnoMarkt.Models;

[Table("AppSettings")]
public partial class AppSetting
{
    [Key]
    [Column("key")]
    [StringLength(100)]
    public string Key { get; set; } = null!;

    [Column("value")]
    [StringLength(500)]
    public string Value { get; set; } = null!;
}
