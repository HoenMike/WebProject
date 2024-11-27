namespace WebProject.Models
{
  public class Blog
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content {get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
  }
}