using System.Collections.Concurrent;
using System.Threading.Tasks;
using EshopApi.Models;

namespace EshopApi.Services
{
    public class StockUpdateQueue : IStockUpdateQueue
    {
        private readonly ConcurrentQueue<StockUpdateMessage> _messages = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void Enqueue(StockUpdateMessage message)
        {
            _messages.Enqueue(message);
            _signal.Release();
        }

        public async Task<StockUpdateMessage> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _messages.TryDequeue(out var message);
            return message!;
        }
    }
}