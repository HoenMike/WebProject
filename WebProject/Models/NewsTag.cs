using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class NewsTag
  {
    [Key, Column(Order = 0)]
    [ForeignKey("News")]
    public int NewsId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Tag")]
    public int TagId { get; set; }

    public News News { get; set; }
    public Tag Tag { get; set; }
  }
}