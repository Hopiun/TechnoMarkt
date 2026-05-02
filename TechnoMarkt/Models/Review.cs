using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Review")]
[Index("ClientId", "ItemId", Name = "UQ_Review_Client_Item", IsUnique = true)]
public partial class Review
{
    [Key]
    [Column("review_id")]
    public int ReviewId { get; set; }

    [Column("client_id")]
    public int ClientId { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("rating")]
    public int Rating { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
}
