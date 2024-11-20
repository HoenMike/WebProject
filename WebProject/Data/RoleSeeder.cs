using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WebProject.Data
{
  public static class RoleSeeder
  {
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
      string[] roleNames = { "Admin", "User" };
      foreach (var roleName in roleNames)
      {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
          await roleManager.CreateAsync(new IdentityRole(roleName));
        }
      }
    }
  }
}