namespace EshopApi.Models
{
    public class StockUpdateMessage
    {
        public int ProductId { get; set; }
        public int NewStock { get; set; }
    }
}