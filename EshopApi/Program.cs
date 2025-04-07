using EshopApi.Data;
using EshopApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

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
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IStockUpdateQueue, StockUpdateQueue>();
            builder.Services.AddHostedService<StockUpdateBackgroundService>();

            var app = builder.Build();

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseExceptionHandler("/error");
            app.Map("/error", (HttpContext context) => 
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                return Results.Problem(title: exception?.Message);
            });

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        options.SwaggerEndpoint(url, $"Eshop API {description.GroupName}");
                    }
                });
            }

            app.MapControllers();

            app.Run();
        }
    }
}
