// Review.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebProject.Data;

namespace WebProject.Models
{
  public class Review
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public string? UserId { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; } = 0;

    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Item? Item { get; set; }

    // Update the navigation property to use ApplicationUser
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
  }
}