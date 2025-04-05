using System.Threading.Tasks;
using EshopApi.Models;

namespace EshopApi.Services
{
    public interface IStockUpdateQueue
    {
        void Enqueue(StockUpdateMessage message);
        Task<StockUpdateMessage> DequeueAsync(CancellationToken cancellationToken);
    }
}