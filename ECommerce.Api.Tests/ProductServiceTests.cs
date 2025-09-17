using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Models;
using ECommerce.Api.Models.Enums;
using ECommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public async Task CreateProductAsync_WhenCategoryExists_ShouldReturnSuccessResultAndCallRepository()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true
        };

        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "New Product To Add",
            StockKeepingUnit = "NEW-PRD",
            Price = 15.99m,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 300,
            CategoryId = 1
        };

        var productToReturnFromRepo = new Product
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            StockKeepingUnit = request.StockKeepingUnit,
            Price = request.Price,
            Discount = request.Discount,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            Category = category
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
        result.Value.Id.Should().Be(productToReturnFromRepo.Id);
        result.Value.Category.Id.Should().Be(category.Id);
        result.Value.Name.Should().Be($"{productToReturnFromRepo.Name}");

        _mockProductRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateCategoryAsync_WhenCategoryDoesNotExist_ShouldReturnFailureResultAndNotCallRepository()
    {
        // Arrange
        var nonExistentCategory = new Category
        {
            Id = 99,
            Name = "Doesn't exist",
            Description = "Not available",
            IsActive = false
        };

        var nonExistentCategoryId = nonExistentCategory.Id;
        
        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "New Product To Add",
            StockKeepingUnit = "NEW-PRD",
            Price = 15.99m,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 300,
            CategoryId = nonExistentCategoryId
        };

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(nonExistentCategoryId))
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
    public async Task CreateCategoryAsync_WhenRepositoryReturnsDbUpdateException_ShouldReturnFailureResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true
        };

        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "New Product To Add",
            StockKeepingUnit = "NEW-PRD",
            Price = 15.99m,
            Discount = DiscountStatus.FivePercent,
            StockQuantity = 300,
            CategoryId = 1
        };


        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);
        
        _mockProductRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));

 
        // Act
        var result = await _productService.CreateProductAsync(request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("Database error occurred");

        _mockProductRepository
            .Verify(repo => repo.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateProductAsync_WhenProductToUpdateIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true
        };

        var productToUpdate = new Product
        {
            Id = 1,
            Name = "Original Product",
            Description = "Original Product Description",
            IsActive = true,
            StockKeepingUnit = "ORG-PRD",
            Price = 39.99m,
            Discount = DiscountStatus.None,
            StockQuantity = 150,
            CategoryId = category.Id,
            Category = category
        };
        
        var request = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Product To Be Updated",
            StockKeepingUnit = "UPD-PRD",
            StockQuantity = 300,
            Discount = DiscountStatus.FivePercent,
            CategoryId = category.Id,
            IsActive = true
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productToUpdate.Id))
            .ReturnsAsync(productToUpdate);

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.UpdateProductAsync(productToUpdate.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.Category.Id.Should().Be(1);

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }
}