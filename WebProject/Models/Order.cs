using System;
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
    public string Status { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public string PaymentMethod { get; set; }

    [Required]
    public string ShippingAddress { get; set; }

    [ForeignKey("UserCard")]
    public int? UserCardId { get; set; }

    public UserCard? UserCard { get; set; }
  }
}