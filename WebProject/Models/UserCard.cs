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
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string CardHolderName { get; set; }

    [Required]
    [StringLength(255)]
    public string CardNumber { get; set; }

    [Required]
    [Column(TypeName = "date")]
    public DateTime ExpiryDate { get; set; }

    [Required]
    [StringLength(255)]
    public string Cvv { get; set; }

    [Required]
    [StringLength(50)]
    public string CardType { get; set; }

    public bool IsPrimary { get; set; }

  }
}