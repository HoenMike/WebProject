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
  public class WishlistController
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly AuthenticationStateProvider _authStateProvider;

    public WishlistController(ApplicationDbContext dbContext, AuthenticationStateProvider authStateProvider)
    {
      _dbContext = dbContext;
      _authStateProvider = authStateProvider;
    }

    public async Task AddToWishlistAsync(int itemId)
    {
      var userId = await GetUserId();

      var existing = await _dbContext.WishLists.FirstOrDefaultAsync(w => w.UserId == userId && w.ItemId == itemId);
      if (existing == null)
      {
        var wishlistItem = new WishList
        {
          UserId = userId,
          ItemId = itemId,
          AddedAt = DateTime.UtcNow
        };
        _dbContext.WishLists.Add(wishlistItem);
        await _dbContext.SaveChangesAsync();
      }
    }

    public async Task RemoveFromWishlistAsync(int itemId)
    {
      try
      {
        var userId = await GetUserId();
        var wishlistItems = await _dbContext.WishLists.Where(ci => ci.ItemId == itemId && ci.UserId == userId).ToListAsync();
        _dbContext.WishLists.RemoveRange(wishlistItems);
        await _dbContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine($"Error in RemoveFromWishListAsync: {ex.Message}");
      }
    }

    public async Task<List<WishList>> GetWishlistAsync()
    {
      var userId = await GetUserId();
      return await _dbContext.WishLists
          .Include(w => w.Item)
          .Where(w => w.UserId == userId)
          .ToListAsync();
    }

    private async Task<string> GetUserId()
    {
      var authState = await _authStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      if (user.Identity?.IsAuthenticated != true)
      {
        throw new InvalidOperationException("You must be logged in to perform this action.");
      }

      return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
          ?? throw new InvalidOperationException("Cannot find user identifier.");
    }
  }
}