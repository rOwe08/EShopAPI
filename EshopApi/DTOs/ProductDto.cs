using System.ComponentModel.DataAnnotations;
using EshopApi.Models;

namespace EshopApi.DTOs
{
    public record class ProductDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]  
        [StringLength(100)]  
        public string Name { get; set; } = string.Empty;  

        [Required(ErrorMessage = "Image URL is required")]  
        [Url]  
        public string ImageUrl { get; set; } = string.Empty;  
        public decimal Price { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int Stock { get; set; }

        public ProductDto(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            ImageUrl = product.ImageUrl;
            Price = product.Price;
            Description = product.Description;
            Stock = product.Stock;
        }
    }
}
