namespace WebProject.Models
{
  public class Item
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
  }
}