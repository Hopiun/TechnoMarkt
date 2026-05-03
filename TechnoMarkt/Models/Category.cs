using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TechnoMarkt.Models;

[Table("Category")]
public partial class Category
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("parent_category_id")]
    public int? ParentCategoryId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [ForeignKey("ParentCategoryId")]
    public virtual Category? ParentCategory { get; set; }
}
