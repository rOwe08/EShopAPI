using EshopApi.Data;
using EshopApi.DTOs;
using EshopApi.Entities;
using EshopApi.Services;
using EshopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EshopApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProductsController : ControllerBase
    {
        private readonly EshopContext _context;

        public ProductsController(EshopContext context)
        {
            _context = context;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new ProductDto(p))
                .ToListAsync();

            return Ok(products);
        }

        [ApiVersion("2.0")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsV2(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await _context.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto(p))
                .ToListAsync();

            return Ok(new PagedResponse<ProductDto>(products, page, pageSize));
        }

        /// <summary>
        /// Create new product(only with name and image url)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                ImageUrl = productDto.ImageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        
        /// <summary>
        /// Get a single product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            return Ok(new ProductDto(product));
        }

        /// <summary>
        /// Update only the product stock.
        /// </summary>
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, UpdateStockDto stockDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var queue = HttpContext.RequestServices.GetRequiredService<IStockUpdateQueue>();
            queue.Enqueue(new StockUpdateMessage
            {
                ProductId = id,
                NewStock = stockDto.Stock
            });

            return Accepted();
        }
    }
}
