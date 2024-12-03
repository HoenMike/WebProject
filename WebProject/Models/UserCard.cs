using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class UserCard
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; }

    [Required]
    [StringLength(100)]
    public required string CardHolderName { get; set; }

    [Required]
    [StringLength(255)]
    public required string CardNumber { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public required DateTime ExpiryDate { get; set; }

    [Required]
    [StringLength(255)]
    public required string Cvv { get; set; }

    [StringLength(50)]
    public string? CardType { get; set; }

    public bool IsPrimary { get; set; }

  }
}