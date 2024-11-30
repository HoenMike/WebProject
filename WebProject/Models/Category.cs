using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
  public class Category
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [ForeignKey("ParentCategory")]
    public int? ParentCategoryId { get; set; }

    // Self-referencing relationship for hierarchical categories
    public Category ParentCategory { get; set; }
    public ICollection<Category> ChildCategories { get; set; }
    public ICollection<Item> Items { get; set; }
  }
}