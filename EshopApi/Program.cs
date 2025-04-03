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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.MapControllers();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();
            app.MigrateDb();
            app.Run();
        }
    }
}
