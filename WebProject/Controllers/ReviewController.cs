// ReviewService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebProject.Data;
using WebProject.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebProject.Controllers
{
  public class ReviewController
  {
    private readonly ApplicationDbContext _context;
    private readonly AuthenticationStateProvider _authStateProvider;

    public ReviewController(ApplicationDbContext context, AuthenticationStateProvider authStateProvider)
    {
      _context = context;
      _authStateProvider = authStateProvider;
    }

    private async Task<string> GetUserIdAsync()
    {
      var authState = await _authStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      if (user.Identity?.IsAuthenticated != true)
      {
        throw new InvalidOperationException("You have to login first");
      }

      return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
             throw new InvalidOperationException("Cannot find user identifier");
    }

    public async Task AddReviewAsync(Review review)
    {
      review.UserId = await GetUserIdAsync();
      review.CreatedAt = DateTime.UtcNow;
      _context.Reviews.Add(review);
      await _context.SaveChangesAsync();
    }

    public async Task<List<Review>> GetReviewsByItemIdAsync(int itemId)
    {
      return await _context.Reviews
          .Include(r => r.User)
          .Where(r => r.ItemId == itemId)
          .OrderByDescending(r => r.CreatedAt)
          .ToListAsync();
    }
  }
}