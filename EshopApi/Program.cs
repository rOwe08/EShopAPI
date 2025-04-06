using EshopApi.Data;
using EshopApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models; 
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;

namespace EshopApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddDbContext<EshopContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("Eshop")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eshop API", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Eshop API", Version = "v2" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddSingleton<IStockUpdateQueue, StockUpdateQueue>();
            builder.Services.AddHostedService<StockUpdateBackgroundService>();

            var app = builder.Build();

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>  
                {  
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eshop API v1");  
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Eshop API v2");  
                    c.RoutePrefix = string.Empty;
                });  
            }

            app.UseExceptionHandler("/error");
            app.Map("/error", (HttpContext context) => 
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                return Results.Problem(title: exception?.Message);
            });
            
            app.MapControllers();
            app.Run();
        }
    }
}
