using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class ItemShipper
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Order")]
    public required int OrderId { get; set; }

    [Required]
    [ForeignKey("User")]
    public required string ShipperId { get; set; }

    public DateTime AssignedAt { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
  }
}