using System;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models.InputModels
{
  public sealed class CheckoutInputModel
  {
    [Required(ErrorMessage = "Address Name is required")]
    [StringLength(100, ErrorMessage = "Address Name cannot be longer than 100 characters")]
    public string? AddressName { get; set; }
    [Required(ErrorMessage = "Receiver Name is required")]
    [StringLength(100, ErrorMessage = "Receiver Name cannot be longer than 100 characters")]
    public string? ReceiverName { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(255, ErrorMessage = "Address cannot be longer than 255 characters")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [StringLength(20, ErrorMessage = "Phone Number cannot be longer than 20 characters")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Payment Method is required")]
    public PaymentMethod PaymentMethod { get; set; }

    [Required(ErrorMessage = "Card Holder Name is required")]
    [StringLength(100, ErrorMessage = "Card Holder Name cannot be longer than 100 characters")]
    public string? CardHolderName { get; set; }

    [Required(ErrorMessage = "Card Number is required")]
    [StringLength(255, ErrorMessage = "Card Number cannot be longer than 255 characters")]
    public string? CardNumber { get; set; }

    [Required(ErrorMessage = "Expiry Date is required")]
    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; }

    [Required(ErrorMessage = "CVV is required")]
    [StringLength(255, ErrorMessage = "CVV cannot be longer than 255 characters")]
    public string? Cvv { get; set; }
  }
}