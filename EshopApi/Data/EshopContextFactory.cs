using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EshopApi.Data
{
    public class EshopContextFactory : IDesignTimeDbContextFactory<EshopContext>
    {
        public EshopContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EshopContext>();
            
            optionsBuilder.UseSqlite("Data Source=eshop.db");

            return new EshopContext(optionsBuilder.Options);
        }
    }
}
