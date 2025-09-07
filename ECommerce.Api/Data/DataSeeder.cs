using ECommerce.Api.Identity;
using ECommerce.Api.Models;
using ECommerce.Api.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Data;

public class DataSeeder
{
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        }

        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Customer" });
        }
    }

    public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        var adminEmail = "admin@example.com";
        var adminPassword = "Password123!";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                if (await roleManager.FindByNameAsync("Admin") is not null)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }

    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var categories = new List<Category>
        {
            new() {Name = "Electronics", Description = "Gadgets and devices."},
            new() {Name = "Books", Description = "Paperback and hardcover books."},
            new() {Name = "Apparel", Description = "Clothing and accessories"}
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        
        var products = new List<Product>
        {
            new() { Name = "Laptop Pro", Description = "High-performance laptop", Price = 1299.99m, Discount = DiscountStatus.TenPercent, StockQuantity = 50, CategoryId = categories[0].Id, StockKeepingUnit = "ELEC-LP-PRO" },
            new() { Name = "Wireless Mouse", Description = "Ergonomic wireless mouse", Price = 49.99m, Discount = DiscountStatus.None,StockQuantity = 200, CategoryId = categories[0].Id, StockKeepingUnit = "ELEC-MS-WL" },
            new() { Name = "The C# Players Guide", Description = "A wonderful introduction to programming in C#", Price = 29.99m, Discount = DiscountStatus.None,StockQuantity = 150, CategoryId = categories[1].Id, StockKeepingUnit = "BOOK-CS-JRN" },
            new() { Name = "Web API Development with ASP.NET Core 8", Description = "Guide to building great Web APIs using ASP.NET Core 8", Price = 69.95m, Discount = DiscountStatus.None,StockQuantity = 100, CategoryId = categories[1].Id, StockKeepingUnit = "BOOK-API-DSG" },
            new() { Name = "C# Developer T-Shirt", Description = "100% cotton developer t-shirt", Price = 24.99m, Discount = DiscountStatus.FivePercent,StockQuantity = 300, CategoryId = categories[2].Id, StockKeepingUnit = "APP-TS-DEV" }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}