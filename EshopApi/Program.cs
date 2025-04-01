using EshopApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EshopApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("Eshop");

            builder.Services.AddDbContext<EshopContext>(options =>
                options.UseSqlite(connectionString));

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.MigrateDb();
            app.Run();
        }
    }
}
