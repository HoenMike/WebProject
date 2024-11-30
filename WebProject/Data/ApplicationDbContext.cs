using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProject.Models;

namespace WebProject.Data
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<UserCard> UserCards { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<NewsTag> NewsTags { get; set; }
    public DbSet<ItemTag> ItemTags { get; set; }
    public DbSet<ItemPhoto> ItemPhotos { get; set; }
    public DbSet<WishList> WishLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<ItemTag>()
          .HasKey(it => new { it.ItemId, it.TagId });

      modelBuilder.Entity<NewsTag>()
          .HasKey(nt => new { nt.NewsId, nt.TagId });
    }
  }
}