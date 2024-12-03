using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class Order
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [StringLength(100)]
    public string PaymentMethod { get; set; }

    [Required]
    [StringLength(500)]
    public string ShippingAddress { get; set; }

    // Navigation property for OrderItems
    public virtual ICollection<OrderItem> OrderItems { get; set; }
  }

}