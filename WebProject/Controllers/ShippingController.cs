using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using WebProject.Data;
using WebProject.Models;
using WebProject.Services;

namespace WebProject.Controllers
{
  public class ShippingController
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly CartController _cartController;

    public ShippingController(ApplicationDbContext dbContext, CartController cartController)
    {
      _dbContext = dbContext;
      _cartController = cartController;
    }

    public async Task AssignOrderToShipper(int orderId)
    {
      var userId = await _cartController.GetUserId();

      var existingItemShipper = await _dbContext.ItemShippers
          .FirstOrDefaultAsync(itemShipper => itemShipper.OrderId == orderId && itemShipper.ShipperId == userId);

      if (existingItemShipper != null)
      {
        throw new InvalidOperationException("This order is already assigned to you.");
      }

      var itemShipper = new ItemShipper
      {
        OrderId = orderId,
        ShipperId = userId,
        AssignedAt = DateTime.UtcNow
      };

      _dbContext.ItemShippers.Add(itemShipper);

      var order = await _dbContext.Orders.FindAsync(orderId);
      if (order != null)
      {
        order.Status = "On Delivery";
      }

      await _dbContext.SaveChangesAsync();
    }
  }
}