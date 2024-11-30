using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class News
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    public string? Content { get; set; }

    [ForeignKey("User")]
    public int CreatedBy { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

  }
}