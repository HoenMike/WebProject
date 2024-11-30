using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class Item
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(500)]
    public string ThumbnailUrl { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    public int StockQuantity { get; set; }

    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
  }
}