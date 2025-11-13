using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Models;

namespace MyAspNetCoreApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ExternalApiLog> ExternalApiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasIndex(e => e.SKU).IsUnique();
            });

            // Configure ExternalApiLog entity
            modelBuilder.Entity<ExternalApiLog>(entity =>
            {
                entity.ToTable("ExternalApiLogs");
                entity.HasKey(e => e.Id);
            });

            // Seed some initial data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop Dell XPS 15",
                    Description = "High-performance laptop with 16GB RAM and 512GB SSD",
                    Price = 1299.99m,
                    Category = "Electronics",
                    Brand = "Dell",
                    StockQuantity = 25,
                    SKU = "DELL-XPS15-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ImageUrl = "https://example.com/images/dell-xps15.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "iPhone 15 Pro",
                    Description = "Latest iPhone with A17 Pro chip and titanium design",
                    Price = 999.99m,
                    Category = "Electronics",
                    Brand = "Apple",
                    StockQuantity = 50,
                    SKU = "APPL-IP15P-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ImageUrl = "https://example.com/images/iphone15pro.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "Samsung Galaxy S24",
                    Description = "Flagship Android phone with 256GB storage",
                    Price = 849.99m,
                    Category = "Electronics",
                    Brand = "Samsung",
                    StockQuantity = 40,
                    SKU = "SAMS-GS24-001",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ImageUrl = "https://example.com/images/galaxy-s24.jpg"
                }
            );
        }
    }
}
