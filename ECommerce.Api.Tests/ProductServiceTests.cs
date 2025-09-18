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

    [TestMethod]
    public async Task UpdateProductAsync_WhenProductToUpdateIsNull_ShouldReturnFailureResult()
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
            .ReturnsAsync((Product?)null);

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(true);
        
        // Act
        var result = await _productService.UpdateProductAsync(productToUpdate.Id, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Product with Id {productToUpdate.Id} not found");

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateProductAsync_WhenCategoryDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        var categoryThatDoesNotExist = new Category
        {
            Id = 999,
            Name = "Non-Existent Category",
            Description = "Doesn't Exist",
            IsActive = false
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
            CategoryId = 1,
            Category = new Category {Id = 1, Name = "Category", Description = "Real Category", IsActive = true}
        };

        var request = new UpdateProductRequest
        {
            Name = "Updated Name",
            Description = "Product To Be Updated",
            StockKeepingUnit = "UPD-PRD",
            StockQuantity = 300,
            Discount = DiscountStatus.FivePercent,
            CategoryId = categoryThatDoesNotExist.Id,
            IsActive = true
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productToUpdate.Id))
            .ReturnsAsync(productToUpdate);

        _mockCategoryRepository
            .Setup(repo => repo.ExistsAsync(request.CategoryId))
            .ReturnsAsync(false);
        
        // Act 
        var result = await _productService.UpdateProductAsync(productToUpdate.Id, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Unable to update the Product's Category to category with Id {request.CategoryId} because category is not found");

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateProductAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        //  Arrange
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

        _mockProductRepository
            .Setup(repo => repo.UpdateAsync(productToUpdate))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));
        
        // Act
        var result = await _productService.UpdateProductAsync(productToUpdate.Id, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("Database error occurred");
    }

    [TestMethod]
    public async Task DeleteProductAsync_WhenProductToDeleteIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true
        };
        
        var productToDelete = new Product
        {
            Id = 1,
            Name = "Deleted Product",
            Description = "Product To Be Deleted",
            IsActive = true,
            StockKeepingUnit = "DEL-PRD",
            Price = 39.99m,
            Discount = DiscountStatus.FifteenPercent,
            CategoryId = category.Id,
            Category = category
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productToDelete.Id))
            .ReturnsAsync(productToDelete);
        
        // Act
        var result = await _productService.DeleteProductAsync(productToDelete.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(productToDelete.Id);

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(It.Is<Product>(p => p.IsDeleted == true)), Times.Once);
    }

    [TestMethod]
    public async Task DeleteProductAsync_WhenProductToDeleteIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var productId = 999;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);
        
        // Act 
        var result = await _productService.DeleteProductAsync(productId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Product with Id {productId} not found");

        _mockProductRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [TestMethod]
    public async Task DeleteProductAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true
        };

        var productToDelete = new Product
        {
            Id = 1,
            Name = "Deleted Product",
            Description = "Product To Be Deleted",
            IsActive = true,
            StockKeepingUnit = "DEL-PRD",
            Price = 39.99m,
            Discount = DiscountStatus.FifteenPercent,
            CategoryId = category.Id,
            Category = category
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productToDelete.Id))
            .ReturnsAsync(productToDelete);

        _mockProductRepository
            .Setup(repo => repo.UpdateAsync(productToDelete))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));

        // Act
        var result = await _productService.DeleteProductAsync(productToDelete.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("Database error occurred");
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WhenProductReturnedIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true,
        };

        var productToReturn = new Product
        {
            Id = 1,
            Name = "Returned Product",
            Description = "Product To Be Returned",
            IsActive = true,
            StockKeepingUnit = "RTN-PRD",
            Price = 39.99m,
            Discount = DiscountStatus.FifteenPercent,
            CategoryId = category.Id,
            Category = category
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productToReturn.Id))
            .ReturnsAsync(productToReturn);
        
        // Act
        var result = await _productService.GetProductByIdAsync(productToReturn.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(productToReturn.Id);

        _mockProductRepository
            .Verify(repo => repo.GetByIdAsync(productToReturn.Id), Times.Once);
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WhenProductReturnedIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var productId = 999;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);
        
        // Act 
        var result = await _productService.GetProductByIdAsync(productId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Product with Id {productId} not found");
    }

    [TestMethod]
    public async Task GetProductByIdAsync_WhenRepositoryThrowsArgumentNullException_ShouldBeFailureResult()
    {
        // Arrange
        var productId = 999;

        _mockProductRepository
            .Setup(repo => repo.GetByIdAsync(productId))
            .ThrowsAsync(new ArgumentNullException(nameof(productId)));
        
        // Act
        var result = await _productService.GetProductByIdAsync(productId);
        
        // Arrange
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("There was an error retrieving the product");
    }

    [TestMethod]
    public async Task GetAllProductsAsync_WhenRepositoryDoesNotThrowArgumentNullException_ShouldReturnSuccessMessage()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Basic Category",
            IsActive = true,
        };

        var product1 = new Product
        {
            Id = 1,
            Name = "Product 1",
            Description = "First",
            IsActive = true,
            StockKeepingUnit = "FST-PRD",
            Price = 18.99m,
            Discount = DiscountStatus.FivePercent,
            CategoryId = category.Id,
            Category = category
        };

        var product2 = new Product
        {
            Id = 1,
            Name = "Product 2",
            Description = "Second",
            IsActive = true,
            StockKeepingUnit = "SCD-PRD",
            Price = 39.99m,
            Discount = DiscountStatus.TenPercent,
            CategoryId = category.Id,
            Category = category
        };

        var productsToBeReturned = new List<Product>
        {
            product1,
            product2
        };

        var mockPagedList = new PagedList<Product>(productsToBeReturned, 2, 1, 10);

        _mockProductRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(mockPagedList);
        
        // Act
        var result = await _productService.GetAllProductsAsync(new PaginationParams());
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CurrentPage.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Count.Should().Be(2);
        result.Value.Items.First().Name.Should().Be("Product 1");
    }

    [TestMethod]
    public async Task GetAllProductsAsync_WhenRepositoryThrowsArgumentNullException_ShouldReturnFailureResult()
    {
        // Arrange
        _mockProductRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<PaginationParams>()))
            .ThrowsAsync(new ArgumentNullException(nameof(PaginationParams)));
        
        // Act
        var result = await _productService.GetAllProductsAsync(new PaginationParams());
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("There was an error retrieving all products");
    }

    [TestMethod]
    public async Task GetCountOfProductsAsync_EnsureRepositoryReturnsANumber()
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
        result.Value.Should().Be(count);

        _mockProductRepository
            .Verify(repo => repo.GetCountOfProductsAsync(), Times.Once);
    }
}