using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Warehouse")]
[Index("StoreId", "ItemId", Name = "UQ_Warehouse_Store_Item", IsUnique = true)]
public partial class Warehouse
{
    [Key]
    [Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("supply_date")]
    public DateOnly? SupplyDate { get; set; }

    [ForeignKey("ItemId")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("StoreId")]
    public virtual Store Store { get; set; } = null!;
}
