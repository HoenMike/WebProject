using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class CartItem
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    public required string UserId { get; set; }

    [Required]
    [ForeignKey("Item")]
    public required int ItemId { get; set; }
    public string? Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ThumbnailUrl { get; set; }
    [NotMapped]
    public bool IsSelected { get; set; }
  }
}