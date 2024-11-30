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

    [ForeignKey("User")]
    public int UserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [Required]
    [StringLength(50)]
    public required string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    [StringLength(100)]
    public required string PaymentMethod { get; set; }

    [Required]

    [StringLength(500)]
    public required string ShippingAddress { get; set; }

  }
}