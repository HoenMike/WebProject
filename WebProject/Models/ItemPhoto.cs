using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class ItemPhoto
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

  }
}