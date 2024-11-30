using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
  public class Tag
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; }

  }
}