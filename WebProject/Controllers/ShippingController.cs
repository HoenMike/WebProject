using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using WebProject.Components.Dialogs;
using WebProject.Data;
using WebProject.Models;
using WebProject.Services;

namespace WebProject.Controllers
{
  public class ShippingController
  {
    private readonly ApplicationDbContext _dbContext;
    private readonly CartController _cartController;
    private readonly ILogger<ShippingController> _logger;
    private readonly IDialogService _dialogService;

    public ShippingController(
        ApplicationDbContext dbContext,
        CartController cartController,
        ILogger<ShippingController> logger,
        IDialogService dialogService)
    {
      _dbContext = dbContext;
      _cartController = cartController;
      _logger = logger;
      _dialogService = dialogService;
    }

    public async Task AssignOrderToShipper(int orderId)
    {
      using var transaction = await _dbContext.Database.BeginTransactionAsync();
      try
      {
        var userId = await _cartController.GetUserId();

        // Use a pessimistic locking approach
        var order = await _dbContext.Orders
            .Where(o => o.Id == orderId)
            .Lock()
            .FirstOrDefaultAsync();

        if (order == null)
        {
          throw new InvalidOperationException("Order not found.");
        }

        // Check if already assigned
        var existingItemShipper = await _dbContext.ItemShippers
            .FirstOrDefaultAsync(itemShipper => itemShipper.OrderId == orderId);

        if (existingItemShipper != null)
        {
          throw new InvalidOperationException("This order is already assigned to another shipper.");
        }

        var itemShipper = new ItemShipper
        {
          OrderId = orderId,
          ShipperId = userId,
          AssignedAt = DateTime.UtcNow
        };

        _dbContext.ItemShippers.Add(itemShipper);

        // Update order status
        order.Status = "Shipper Assigned";

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (InvalidOperationException ex)
      {
        _logger.LogError(ex, "Invalid operation error while assigning order to shipper");
        await transaction.RollbackAsync();
        throw;
      }
      catch (DbUpdateConcurrencyException ex)
      {
        _logger.LogError(ex, "Concurrency error while assigning order to shipper");
        await transaction.RollbackAsync();
        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unexpected error while assigning order to shipper");
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task CancelOrder(int orderId)
    {
      var parameters = new DialogParameters
      {
        ["Message"] = "Are you sure you want to cancel this order?"
      };
      var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
      var dialog = _dialogService.Show<ConfirmationDialog>("Cancel Order", parameters, options);
      var result = await dialog.Result;

      if (result.Canceled || !(result.Data is bool confirmed) || !confirmed)
      {
        return;
      }

      using var transaction = await _dbContext.Database.BeginTransactionAsync();
      try
      {
        // Use a pessimistic locking approach
        var order = await _dbContext.Orders
            .Where(o => o.Id == orderId)
            .Lock()
            .FirstOrDefaultAsync();

        if (order == null)
        {
          throw new InvalidOperationException("Order not found.");
        }

        // Find and remove the ItemShipper entry
        var itemShipper = await _dbContext.ItemShippers
            .FirstOrDefaultAsync(itemShipper => itemShipper.OrderId == orderId);

        if (itemShipper == null)
        {
          throw new InvalidOperationException("Order is not assigned to a shipper.");
        }

        // Remove the ItemShipper entry
        _dbContext.ItemShippers.Remove(itemShipper);

        // Update the order status to Cancelled
        order.Status = "Cancelled";

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (InvalidOperationException ex)
      {
        _logger.LogError(ex, "Invalid operation error while cancelling order");
        await transaction.RollbackAsync();
        throw;
      }
      catch (DbUpdateConcurrencyException ex)
      {
        _logger.LogError(ex, "Concurrency error while cancelling order");
        await transaction.RollbackAsync();
        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unexpected error while cancelling order");
        await transaction.RollbackAsync();
        throw;
      }
    }
  }

  // Extension method for pessimistic locking
  public static class QueryableExtensions
  {
    public static IQueryable<T> Lock<T>(this IQueryable<T> source) where T : class
    {
      return source.TagWith("Lock");
    }
  }
}