using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using EshopApi.Data;

namespace EshopApi.Services
{
    public class StockUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public StockUpdateBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var queue = scope.ServiceProvider.GetRequiredService<IStockUpdateQueue>();
                var context = scope.ServiceProvider.GetRequiredService<EshopContext>();

                var message = await queue.DequeueAsync(stoppingToken);
                var product = await context.Products.FindAsync(message.ProductId);

                if (product != null)
                {
                    product.StockQuantity = message.NewStock;
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
        }
    }
}