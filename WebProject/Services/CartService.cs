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

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
      try
      {
        var userId = GetUserId();
        return await _dbContext.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
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
        var existingCartItem = await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.ItemId == item.Id && ci.UserId == userId);

        if (existingCartItem != null)
        {
          existingCartItem.Quantity += quantity;
        }
        else
        {
          var newCartItem = new CartItem
          {
            UserId = userId,
            ItemId = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = quantity,
            ThumbnailUrl = item.ThumbnailUrl
          };
          _dbContext.CartItems.Add(newCartItem);
        }

        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in AddToCartAsync: {ex.Message}");
      }
    }

    public async Task RemoveFromCartAsync(int itemId)
    {
      try
      {
        var userId = GetUserId();
        var cartItems = await _dbContext.CartItems.Where(ci => ci.ItemId == itemId && ci.UserId == userId).ToListAsync();
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

    public async Task<decimal> GetTotalPriceAsync()
    {
      try
      {
        var userId = GetUserId();
        return await _dbContext.CartItems.Where(ci => ci.UserId == userId).SumAsync(ci => ci.Price * ci.Quantity);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in GetTotalPriceAsync: {ex.Message}");
        return 0;
      }
    }
  }
}