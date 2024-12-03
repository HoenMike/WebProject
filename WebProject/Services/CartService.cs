using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Text.Json;
using WebProject.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WebProject.Services
{
  public class CartService
  {
    private readonly IJSRuntime _jsRuntime;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartKey = "shopping-cart";

    public CartService(IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor)
    {
      _jsRuntime = jsRuntime;
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
        var cartJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", CartKey);
        return string.IsNullOrEmpty(cartJson)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
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
        var cartItems = await GetCartItemsAsync();
        var userId = GetUserId();

        var existingCartItem = cartItems.FirstOrDefault(ci => ci.ItemId == item.Id && ci.UserId == userId);
        if (existingCartItem != null)
        {
          existingCartItem.Quantity += quantity;
        }
        else
        {
          cartItems.Add(new CartItem
          {
            UserId = userId,
            ItemId = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = quantity,
            ThumbnailUrl = item.ThumbnailUrl
          });
        }

        await SaveCartAsync(cartItems);
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
        var cartItems = await GetCartItemsAsync();
        var userId = GetUserId();
        cartItems.RemoveAll(ci => ci.ItemId == itemId && ci.UserId == userId);
        await SaveCartAsync(cartItems);
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
        var cartItems = await GetCartItemsAsync();
        var userId = GetUserId();
        var cartItem = cartItems.FirstOrDefault(ci => ci.ItemId == itemId && ci.UserId == userId);

        if (cartItem != null)
        {
          if (quantity <= 0)
          {
            cartItems.Remove(cartItem);
          }
          else
          {
            cartItem.Quantity = quantity;
          }

          await SaveCartAsync(cartItems);
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
        await _jsRuntime.InvokeAsync<object>("localStorage.removeItem", CartKey);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in ClearCartAsync: {ex.Message}");
      }
    }

    private async Task SaveCartAsync(List<CartItem> cartItems)
    {
      try
      {
        var cartJson = JsonSerializer.Serialize(cartItems);
        await _jsRuntime.InvokeAsync<object>("localStorage.setItem", CartKey, cartJson);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in SaveCartAsync: {ex.Message}");
      }
    }

    public async Task<decimal> GetTotalPriceAsync()
    {
      try
      {
        var cartItems = await GetCartItemsAsync();
        var userId = GetUserId();
        return cartItems.Where(ci => ci.UserId == userId).Sum(ci => ci.Price * ci.Quantity);
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in GetTotalPriceAsync: {ex.Message}");
        return 0;
      }
    }
  }
}