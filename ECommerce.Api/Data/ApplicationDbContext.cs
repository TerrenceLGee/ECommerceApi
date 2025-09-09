using ECommerce.Api.Identity;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleProduct> SaleProducts { get; set; }
    public DbSet<Address> Address { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleProduct>()
            .HasKey(sp => new { sp.SaleId, sp.ProductId });

        modelBuilder.Entity<SaleProduct>()
            .HasOne(sp => sp.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(sp => sp.SaleId);

        modelBuilder.Entity<SaleProduct>()
            .HasOne(sp => sp.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(sp => sp.ProductId);

        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Sale>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<SaleProduct>().HasQueryFilter(sp => !sp.Sale.IsDeleted);

        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(s => s.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<SaleProduct>().Property(sp => sp.UnitPrice).HasPrecision(18, 2);

        modelBuilder.Entity<Sale>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}