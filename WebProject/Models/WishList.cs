using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class WishList
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public string? UserId { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    public DateTime AddedAt { get; set; }

    // Navigation property
    public Item Item { get; set; }
  }
}