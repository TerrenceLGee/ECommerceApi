using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Models;
using ECommerce.Api.Models.Enums;
using ECommerce.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Api.Tests;

[TestClass]
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockLogger = new Mock<ILogger<ProductService>>();

        _productService = new ProductService(
            _mockProductRepository.Object,
            _mockCategoryRepository.Object,
            mockLogger.Object);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WhenProductExists_ShouldReturnSuccessResultWithProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Some Description",
            StockKeepingUnit = "ABC-123",
            Price = 99.99m,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 500,
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Test Category" }
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(1);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("Test Product");
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);
        
        // Act
        var result = await _productService.GetProductByIdAsync(99);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("not found");
    }

    [TestMethod]
    public async Task CreateProductAsync_WhenCategoryExists_ShouldCallRepositoryAndReturnSuccess()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            CategoryId = 1, 
            Name = "New Gadget", 
            Description = "Some Description",
            StockKeepingUnit = "ABC-123",
            Price = 99.99m,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 500,
        };

        var productToReturnFromRepo = new Product
        {
            Id = 1,
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            Category = new Category { Id = 1, Name = "Test Category" }
        };

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(productToReturnFromRepo);
        
        // Act
        var result = await _productService.CreateProductAsync(request);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);

        _mockProductRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateProductAsync_WhenCategoryDoesNotExist_ShouldReturnFailureAndNotCallRepository()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            CategoryId = 99,
            Name = "New Gadget",
            Description = "Some Description",
            StockKeepingUnit = "ABC-123",
            Price = 99.99m,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 500,
        };

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(false);
        
        // Act
        var result = await _productService.CreateProductAsync(request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should()
            .Contain(
                $"Unable to add a product to Category with id {request.CategoryId} because that category is not found");

        _mockProductRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateProductAsync_WhenProductExists_ShouldCallRepositoryAndReturnSuccess()
    {
        // Arrange 
        var request = new UpdateProductRequest
        {
            CategoryId = 1,
            Name = "Updated Gadget",
            Description = "Updated Description",
            StockKeepingUnit = "ABC-123",
            IsActive = true,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 500,
        };

        var productToReturnFromRepo = new Product
        {
            Id = 1,
            Name = request.Name,
            StockKeepingUnit = request.StockKeepingUnit,
            Discount = request.Discount,
            Description = request.Description,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            Category = new Category {Id = 1, Name = "Test Category"}
        };

        var productId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(productToReturnFromRepo);
        
        // Act
        var result = await _productService.UpdateProductAsync(productId, request);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(productToReturnFromRepo), Times.Once);
    }

    [TestMethod]
    public async Task UpdateProductAsync_WhenProductDoesNotExist_ShouldReturnFailureAndNotCallRepository()
    {
        // Arrange
        var nonExistentProduct = new Product
        {
            Id = 99,
            CategoryId = 1,
            Name = "Non-Existent Gadget",
            Description = "Does not exist",
            StockKeepingUnit = "000-000",
            IsActive = false,
            Discount = DiscountStatus.None,
            StockQuantity = 0
        };

        var request = new UpdateProductRequest
        {
            CategoryId = 1,
            Name = "NonExistent Gadget",
            Description = "Does not exist",
            StockKeepingUnit = "000-000",
            IsActive = false,
            Discount = DiscountStatus.None,
            StockQuantity = 0
        };

        var productId = 99;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(nonExistentProduct.Id))
            .ReturnsAsync((Product?)null);
        
        // Act 
        var result = await _productService.UpdateProductAsync(productId, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain($"Product with Id {productId} not found");

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(nonExistentProduct), Times.Never);
    }

    [TestMethod]
    public async Task DeleteProductAsync_WhenProductExists_ShouldReturnSuccessAndCallRepository()
    {
        // Arrange
        var productId = 1;

        var productToReturnToDelete = new Product
        {
            Id = productId,
            Name = "Deleted product",
            Description = "Product Deleted",
            DeletedAt = DateTime.UtcNow,
            StockKeepingUnit = "DEL-123-ETE",
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 100,
            CategoryId = 1,
            Category = new Category { Id = 1, Description = "Test Category" }
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(productToReturnToDelete);

        // Act
        var result = await _productService.DeleteProductAsync(productId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(productToReturnToDelete), Times.Once);
    }

    [TestMethod]
    public async Task DeleteProductAsync_WhenProductDoesNotExist_ShouldReturnFailureAndNotCallRepository()
    {
        // Arrange
        var nonExistentProductToDelete = new Product
        {
            Id = 99,
            CategoryId = 1,
            Name = "Non-Existent Gadget",
            Description = "Does not exist",
            StockKeepingUnit = "000-000",
            IsActive = false,
            Discount = DiscountStatus.None,
            StockQuantity = 0
        };

        var productId = nonExistentProductToDelete.Id;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);
        
        // Act 
        var result = await _productService.DeleteProductAsync(productId);
        
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain($"Product with Id {productId} not found");

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(nonExistentProductToDelete), Times.Never);
    }

    [TestMethod]
    public async Task GetAllProductsAsync_WhenProductsExists_ShouldReturnPagedListOfResponses()
    {
        // Arrange
        var product1 = new Product
        {
            Id = 1,
            CategoryId = 1,
            Name = "Product 1",
            Description = "First",
            StockKeepingUnit = "111-111",
            IsActive = true,
            Discount = DiscountStatus.None,
            StockQuantity = 100,
            Category = new Category {Id = 1, Name = "Category 1", Description = "A category"}
        };
        
        var product2 = new Product
        {
            Id = 2,
            CategoryId = 1,
            Name = "Product 2",
            Description = "Second",
            StockKeepingUnit = "222-222",
            IsActive = true,
            Discount = DiscountStatus.None,
            StockQuantity = 200,
            Category = new Category {Id = 1, Name = "Category 1", Description = "A category"}
        };

        var productsToBeReturnedFromRepo = new List<Product>
        {
            product1,
            product2
        };

        var mockPagedList = new PagedList<Product>(productsToBeReturnedFromRepo, 2, 1, 10);

        _mockProductRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(mockPagedList);
        
        // Act
        var result = await _productService.GetAllProductsAsync(new PaginationParams());
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.CurrentPage.Should().Be(1);
        result.Value.TotalCount.Should().Be(2);
        result.Value.PageSize.Should().Be(10);

        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.First().Name.Should().Be("Product 1");

        _mockProductRepository
            .Verify(repo => repo.GetAllAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [TestMethod]
    public async Task GetCountOfProductsAsync_WhenCalled_ShouldReturnSuccessAndCallRepository()
    {
        // Arrange
        int count = 3;

        _mockProductRepository
            .Setup(repo => repo.GetCountOfProductsAsync())
            .ReturnsAsync(count);
        
        // Act
        var result = await _productService.GetCountOfProductsAsync();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(3);
    }
   
}