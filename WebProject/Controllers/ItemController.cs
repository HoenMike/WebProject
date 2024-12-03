using Microsoft.EntityFrameworkCore;
using WebProject.Data;
using WebProject.Models;

namespace WebProject.Controllers
{
  public class ItemController(ApplicationDbContext context)
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<List<Item>> GetItemsAsync()
    {
      return await _context.Items.ToListAsync();
    }

    public async Task<Item?> GetItemAsync(int id)
    {
      return await _context.Items.FindAsync(id);
    }

    public async Task AddItemAsync(Item item)
    {
      _context.Items.Add(item);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(Item updatedItem)
    {
      var existingItem = await _context.Items.FindAsync(updatedItem.Id);
      if (existingItem != null)
      {
        existingItem.Name = updatedItem.Name;
        existingItem.Price = updatedItem.Price;
        existingItem.Description = updatedItem.Description;
        existingItem.ThumbnailUrl = updatedItem.ThumbnailUrl;
        existingItem.StockQuantity = updatedItem.StockQuantity;

        await _context.SaveChangesAsync();
      }
      else
      {
        throw new Exception("Item not found");
      }
    }

    public async Task DeleteItemAsync(Item item)
    {
      _context.Items.Remove(item);
      await _context.SaveChangesAsync();
    }
  }
}