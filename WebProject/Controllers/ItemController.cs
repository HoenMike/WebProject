using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebProject.Data;
using WebProject.Models;

namespace WebProject.Controllers
{
  public class ItemController
  {
    private readonly ApplicationDbContext _context;

    public ItemController(ApplicationDbContext context)
    {
      _context = context;
    }

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

    public async Task UpdateItemAsync(Item item)
    {
      _context.Items.Update(item);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(Item item)
    {
      _context.Items.Remove(item);
      await _context.SaveChangesAsync();
    }
  }
}