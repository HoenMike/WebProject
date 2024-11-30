using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class OrderItem
  {
    [Key]
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [ForeignKey("Item")]
    public int ItemId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PriceAtTimeOfOrder { get; set; }

  }
}