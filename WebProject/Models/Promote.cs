using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class Promote
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public required string ImageUrl { get; set; }
    public bool IsActive { get; set; }

  }
}