using EshopApi.Data;
using EshopApi.DTOs;
using EshopApi.Services;
using EshopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning;

namespace EshopApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    public class ProductsController(EshopContext context) : ControllerBase
    {
        private readonly EshopContext _context = context;

        /// <summary>
        /// Get full list of products (API v1)
        /// </summary>
        [HttpGet, MapToApiVersion("1.0"), ApiExplorerSettings(GroupName = "v1")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsV1()
        {
            var products = await _context.Products
                .Select(p => new ProductDto(p))
                .ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Get paginated list of products (API v2)
        /// </summary>
        [HttpGet, MapToApiVersion("2.0"), ApiExplorerSettings(GroupName = "v2")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsV2(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be >= 1");

            var query = _context.Products.AsQueryable();
            var totalCount = await query.CountAsync();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto(p))
                .ToListAsync();

            return Ok(new PagedResponse<ProductDto>(products, page, pageSize, totalCount));
        }

        /// <summary>
        /// Create new product (only name and image URL required)
        /// </summary>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
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

            var createdProduct = new ProductDto(product);

            return CreatedAtRoute(
                routeName: nameof(GetProductById),
                routeValues: new { id = product.Id, version = "1.0" },
                value: createdProduct
            );
        }

        /// <summary>
        /// Get a single product by ID
        /// </summary>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpGet("{id}", Name = nameof(GetProductById))]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            return product == null
                ? NotFound()
                : Ok(new ProductDto(product));
        }

        /// <summary>
        /// Synchronous stock update
        /// </summary>
        [HttpPatch("{id}/stock")]
        [MapToApiVersion(1.0)]
        public async Task<IActionResult> UpdateStockV1(int id, UpdateStockDto stockDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Stock = stockDto.Stock;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Update only the product stock (async, queued)
        /// </summary>
        [MapToApiVersion("2.0")]
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStockV2(int id, UpdateStockDto stockDto)
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