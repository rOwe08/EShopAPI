using Microsoft.EntityFrameworkCore;
using EshopApi.Entities;

namespace EshopApi.Data
{
    public class EshopContext(DbContextOptions<EshopContext> options)  
        : DbContext(options)
    {
        public DbSet<Product> Products => Set<Product>();   
    }
}