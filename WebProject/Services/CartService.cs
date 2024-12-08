using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebProject.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WebProject.Data;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebProject.Services
{
  public class CartService
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly AuthenticationStateProvider _authStateProvider;

    public CartService(ApplicationDbContext dbContext, AuthenticationStateProvider authStateProvider)
    {
      _dbContext = dbContext;
      _authStateProvider = authStateProvider;
    }

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
      try
      {
        var userId = await GetUserId();
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

    private async Task<string> GetUserId()
    {
      var authState = await _authStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      if (user.Identity?.IsAuthenticated != true)
      {
        throw new InvalidOperationException("User is not authenticated");
      }

      return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
          throw new InvalidOperationException("Cannot find user identifier");
    }

    public async Task AddToCartAsync(Item item, int quantity)
    {
      try
      {
        var userId = await GetUserId();
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
          if (item.StockQuantity <= 0)
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
        var userId = await GetUserId();
        var cartItems = await _dbContext.CartItems.Where(ci => ci.ItemId == itemId && ci.UserId == userId).ToListAsync();
        foreach (var cartItem in cartItems)
        {
          var product = await _dbContext.Items.FindAsync(cartItem.ItemId);
          if (product != null)
          {
            product.StockQuantity += cartItem.Quantity;
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
        var userId = await GetUserId();
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
        var userId = await GetUserId();
        var cartItems = await _dbContext.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
        _dbContext.CartItems.RemoveRange(cartItems);
        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in ClearCartAsync: {ex.Message}");
      }
    }

    public async Task CheckoutAsync(List<CartItem> selectedItems)
    {
      try
      {
        var userId = await GetUserId();
        var shippingInfo = await _dbContext.UserShippingInfos.FirstOrDefaultAsync(u => u.UserId == userId);
        var paymentInfo = await _dbContext.UserCards.FirstOrDefaultAsync(u => u.UserId == userId && u.IsPrimary);

        if (shippingInfo == null || paymentInfo == null)
        {
          throw new InvalidOperationException("Missing shipping or payment information");
        }

        var order = new Order
        {
          UserId = userId,
          TotalPrice = selectedItems.Sum(item => item.Price * item.Quantity),
          Status = "Pending",
          CreatedAt = DateTime.UtcNow,
          PaymentMethod = shippingInfo.PaymentMethod.ToString(),
          ShippingAddress = $"{shippingInfo.ReceiverName}, {shippingInfo.Address}, {shippingInfo.PhoneNumber}"
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
        Console.Error.WriteLine($"Error in CheckoutAsync: {ex.Message}");
        throw;
      }
    }

    public async Task<decimal> GetTotalPriceAsync()
    {
      try
      {
        var userId = await GetUserId();
        var cartItems = await GetCartItemsAsync();
        return cartItems.Sum(item => item.Price * item.Quantity);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in GetTotalPriceAsync: {ex.Message}");
        return 0;
      }
    }

    public async Task<Item> GetProductByIdAsync(int itemId)
    {
      return await _dbContext.Items.FindAsync(itemId);
    }
  }
}