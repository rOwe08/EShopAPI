using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EshopApi.Controllers;
using EshopApi.Data;
using EshopApi.DTOs;
using Moq;
using EshopApi.Services;
using Microsoft.AspNetCore.Http;
using EshopApi.Models;

public class ProductsControllerTests
{
    private readonly ProductsController _controller;
    private readonly EshopContext _context;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<EshopContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new EshopContext(options);
        SeedTestData();
        _controller = new ProductsController(_context);
    }

    private void SeedTestData()
    {
        _context.Products.AddRange(
            new Product { Id = 1, Name = "iPhone 15", ImageUrl = "iphone.jpg", Stock = 10 },
            new Product { Id = 2, Name = "Samsung Galaxy S24", ImageUrl = "samsung.jpg", Stock = 15 },
            new Product { Id = 3, Name = "Google Pixel 8", ImageUrl = "pixel.jpg", Stock = 20 }
        );
        _context.SaveChanges();
    }

    private ProductsController CreateControllerWithMockQueue()
    {
        var mockQueue = new Mock<IStockUpdateQueue>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IStockUpdateQueue)))
            .Returns(mockQueue.Object);

        return new ProductsController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider.Object
                }
            }
        };
    }

    #region V1 Tests

    [Fact]
    public async Task GetProductsV1_ReturnsAllProducts()
    {
        // Act
        var result = await _controller.GetProductsV1();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<ProductDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedProducts = Assert.IsType<List<ProductDto>>(okResult.Value);

        Assert.Equal(3, returnedProducts.Count);
        Assert.Contains(returnedProducts, p => p.Name == "iPhone 15");
        Assert.Contains(returnedProducts, p => p.Name == "Samsung Galaxy S24");
        Assert.Contains(returnedProducts, p => p.Name == "Google Pixel 8");
    }

    [Fact]
    public async Task UpdateStockV1_UpdatesStockSynchronously()
    {
        // Arrange
        const int newStock = 25;

        // Act
        var result = await _controller.UpdateStockV1(1, new UpdateStockDto { Stock = newStock });

        // Assert
        Assert.IsType<NoContentResult>(result);
        var product = await _context.Products.FindAsync(1);
        Assert.Equal(newStock, product.Stock);
    }

    #endregion

    #region V2 Tests

    [Fact]
    public async Task GetProductsV2_ReturnsPaginatedProducts()
    {
        // Act
        var result = await _controller.GetProductsV2(page: 1, pageSize: 2);

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<ProductDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var response = Assert.IsType<PagedResponse<ProductDto>>(okResult.Value);

        Assert.Equal(2, response.Data.Count());
        Assert.Equal(3, response.TotalCount);
    }

    [Fact]
    public async Task UpdateStockV2_QueuesUpdateRequest()
    {
        // Arrange
        var controller = CreateControllerWithMockQueue();
        const int newStock = 30;

        // Act
        var result = await controller.UpdateStockV2(1, new UpdateStockDto { Stock = newStock });

        // Assert
        Assert.IsType<AcceptedResult>(result);
    }

    [Fact]
    public async Task UpdateStockV2_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var controller = CreateControllerWithMockQueue();

        // Act
        var result = await controller.UpdateStockV2(999, new UpdateStockDto { Stock = 50 });

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region Shared Tests

    [Fact]
    public async Task GetProductById_ReturnsCorrectProduct()
    {
        // Act
        var result = await _controller.GetProductById(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProductDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var product = Assert.IsType<ProductDto>(okResult.Value);

        Assert.Equal("iPhone 15", product.Name);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Act
        var result = await _controller.GetProductById(999);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProductDto>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async Task CreateProduct_AddsNewProduct()
    {
        // Arrange
        var newProduct = new ProductCreateDto
        {
            Name = "Test Product",
            ImageUrl = "test.jpg"
        };

        // Act
        var result = await _controller.CreateProduct(newProduct);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ProductDto>>(result);
        var createdAtResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
        Assert.Equal(nameof(ProductsController.GetProductById), createdAtResult.RouteName);
        Assert.Equal(4, await _context.Products.CountAsync());
    }

    #endregion
}