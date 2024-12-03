using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebProject.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WebProject.Data;

namespace WebProject.Services
{
  public class CartService
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
      _dbContext = dbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserId()
    {
      return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User is not authenticated");
    }

    public async Task<Item?> GetProductByIdAsync(int itemId)
    {
      return await _dbContext.Items.FindAsync(itemId);
    }

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
      try
      {
        var userId = GetUserId();
        var cartItems = await _dbContext.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
        foreach (var item in cartItems)
        {
          var product = await _dbContext.Items.FindAsync(item.ItemId);
          if (product != null)
          {
            item.ThumbnailUrl = product.ThumbnailUrl;
            item.Price = product.Price;
          }
        }
        return cartItems;
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in GetCartItemsAsync: {ex.Message}");
        return new List<CartItem>();
      }
    }
    public async Task AddToCartAsync(Item item, int quantity)
    {
      try
      {
        var userId = GetUserId();
        var cartItem = await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ItemId == item.Id);

        if (item.StockQuantity < quantity)
        {
          throw new InvalidOperationException("Not enough stock available");
        }

        if (cartItem == null)
        {
          cartItem = new CartItem
          {
            UserId = userId,
            ItemId = item.Id,
            Name = item.Name,
            Quantity = quantity,
            Price = item.Price,
            ThumbnailUrl = item.ThumbnailUrl
          };
          _dbContext.CartItems.Add(cartItem);
        }
        else
        {
          if (cartItem.Quantity + quantity > item.StockQuantity)
          {
            throw new InvalidOperationException("Not enough stock available");
          }
          cartItem.Quantity += quantity;
        }

        item.StockQuantity -= quantity;
        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in AddToCartAsync: {ex.Message}");
        throw;
      }
    }

    public async Task RemoveFromCartAsync(int itemId)
    {
      try
      {
        var userId = GetUserId();
        var cartItems = await _dbContext.CartItems.Where(ci => ci.ItemId == itemId && ci.UserId == userId).ToListAsync();
        foreach (var cartItem in cartItems)
        {
          var product = await _dbContext.Items.FindAsync(cartItem.ItemId);
          if (product != null)
          {
            product.StockQuantity += cartItem.Quantity; // Increase stock quantity
          }
        }
        _dbContext.CartItems.RemoveRange(cartItems);
        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in RemoveFromCartAsync: {ex.Message}");
      }
    }

    public async Task UpdateQuantityAsync(int itemId, int quantity)
    {
      try
      {
        var userId = GetUserId();
        var cartItem = await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == itemId && ci.UserId == userId);

        if (cartItem != null)
        {
          if (quantity <= 0)
          {
            _dbContext.CartItems.Remove(cartItem);
          }
          else
          {
            cartItem.Quantity = quantity;
          }

          await _dbContext.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in UpdateQuantityAsync: {ex.Message}");
        throw;
      }
    }

    public async Task UpdateProductStockAsync(Item product)
    {
      try
      {
        _dbContext.Items.Update(product);
        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in UpdateProductStockAsync: {ex.Message}");
        throw;
      }
    }

    public async Task ClearCartAsync()
    {
      try
      {
        var userId = GetUserId();
        var cartItems = await _dbContext.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
        _dbContext.CartItems.RemoveRange(cartItems);
        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in ClearCartAsync: {ex.Message}");
      }
    }

    public async Task DemoCheckoutAsync(List<CartItem> selectedItems)
    {
      try
      {
        var userId = GetUserId();
        var order = new Order
        {
          UserId = userId,
          TotalPrice = selectedItems.Sum(item => item.Price * item.Quantity),
          Status = "Completed",
          CreatedAt = DateTime.UtcNow,
          PaymentMethod = "Demo",
          ShippingAddress = "Demo Address"
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        foreach (var cartItem in selectedItems)
        {
          var orderItem = new OrderItem
          {
            OrderId = order.Id,
            ItemId = cartItem.ItemId,
            Quantity = cartItem.Quantity,
            PriceAtTimeOfOrder = cartItem.Price
          };

          _dbContext.OrderItems.Add(orderItem);
          _dbContext.CartItems.Remove(cartItem);
        }

        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in DemoCheckoutAsync: {ex.Message}");
        throw;
      }
    }

    public async Task<decimal> GetTotalPriceAsync()
    {
      try
      {
        var userId = GetUserId();
        var cartItems = await GetCartItemsAsync();
        return cartItems.Sum(item => item.Price * item.Quantity);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in GetTotalPriceAsync: {ex.Message}");
        return 0;
      }
    }
  }
}