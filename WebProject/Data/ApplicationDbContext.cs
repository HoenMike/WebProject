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
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<UserShippingInfo> UserShippingInfos { get; set; }
    public DbSet<UserCard> UserCards { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ItemPhoto> ItemPhotos { get; set; }
    public DbSet<WishList> WishLists { get; set; }
    public DbSet<Promote> Promotes { get; set; }
    public DbSet<ItemShipper> ItemShippers { get; set; }

  }
}