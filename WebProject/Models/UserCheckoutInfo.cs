using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public enum PaymentMethod
  {
    Cash,
    CreditCard
  }

  public class UserCheckoutInfo
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    public required string UserId { get; set; }

    [Required]
    public required string ReceiverName { get; set; }

    [Required]
    public required string Address { get; set; }

    [Required]
    public required string PhoneNumber { get; set; }

    [Required]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod PaymentMethod { get; set; }


  }
}