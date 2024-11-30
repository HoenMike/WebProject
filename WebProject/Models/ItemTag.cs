using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class ItemTag
  {
    [Key, Column(Order = 0)]
    [ForeignKey("Item")]
    public int ItemId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Tag")]
    public int TagId { get; set; }

    public Item Item { get; set; }
    public Tag Tag { get; set; }
  }
}