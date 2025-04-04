using Microsoft.EntityFrameworkCore;
using EshopApi.Entities;

namespace EshopApi.Data
{
    public class EshopContext(DbContextOptions<EshopContext> options)  : DbContext(options)
    {
        public virtual  DbSet<Product> Products => Set<Product>();   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "iPhone 15", ImageUrl = "someUrl1", Price = 999.99m, Description = "Apple smartphone" },
                new Product { Id = 2, Name = "Samsung Galaxy S24", ImageUrl = "someUrl3", Price = 899.99m, Description = "Samsung smartphone" },
                new Product { Id = 3, Name = "Google Pixel 8", Price = 799.99m, ImageUrl = "someUrl3", Description = "Google smartphone" }
            );
        }
    }
}