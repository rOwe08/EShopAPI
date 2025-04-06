using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EshopApi.Controllers;
using EshopApi.Data;
using EshopApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _controller = new ProductsController(_context);
    }

    [Fact]
    public async Task GetProducts_ReturnsAllMockProducts()
    {
        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<ProductDto>>(okResult.Value);

        Assert.Equal(3, returnedProducts.Count);
        Assert.Contains(returnedProducts, p => p.Name == "iPhone 15");
        Assert.Contains(returnedProducts, p => p.Name == "Samsung Galaxy S24");
        Assert.Contains(returnedProducts, p => p.Name == "Google Pixel 8");
    }

    [Fact]
    public async Task GetProductById_ReturnsCorrectProduct()
    {
        // Act
        var result = await _controller.GetProductById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<ProductDto>(okResult.Value);

        Assert.Equal("iPhone 15", product.Name);
        Assert.Equal("Apple smartphone", product.Description);
    }

    [Fact]
    public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Act
        var result = await _controller.GetProductById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
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
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var product = Assert.IsType<Product>(createdAtActionResult.Value);

        Assert.Equal(newProduct.Name, product.Name);
        Assert.Equal(newProduct.ImageUrl, product.ImageUrl);

        var products = await _context.Products.ToListAsync();
        Assert.Equal(4, products.Count);
    }

    [Fact]
    public async Task UpdateStock_ChangesStockValue()
    {
        // Arrange
        var mockQueue = new Mock<IStockUpdateQueue>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IStockUpdateQueue)))
            .Returns(mockQueue.Object);

        var controller = new ProductsController(_context) 
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext 
                { 
                    RequestServices = serviceProvider.Object 
                }
            }
        };

        // Act
        var result = await controller.UpdateStock(1, new UpdateStockDto { Stock = 100 });

        // Assert
        mockQueue.Verify(q => q.Enqueue(It.Is<StockUpdateMessage>(m => 
            m.ProductId == 1 && m.NewStock == 100)), Times.Once);
        Assert.IsType<AcceptedResult>(result);
    }

    [Fact]
    public async Task UpdateStock_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var stockUpdateDto = new UpdateStockDto { Stock = 50 };

        // Act
        var result = await _controller.UpdateStock(999, stockUpdateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStock_QueuesUpdateRequest()
    {
        // Arrange
        var mockQueue = new Mock<IStockUpdateQueue>();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IStockUpdateQueue)))
            .Returns(mockQueue.Object);

        var controller = new ProductsController(_context) 
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext 
                { 
                    RequestServices = serviceProvider.Object 
                }
            }
        };

        // Act
        var result = await controller.UpdateStock(1, new UpdateStockDto { Stock = 50 });

        // Assert
        mockQueue.Verify(q => q.Enqueue(It.IsAny<StockUpdateMessage>()), Times.Once);
        Assert.IsType<AcceptedResult>(result);
    }
}
