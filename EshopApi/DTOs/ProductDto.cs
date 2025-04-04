namespace EshopApi.DTOs
{
    public record class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
