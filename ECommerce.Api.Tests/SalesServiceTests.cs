using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Sales.Request;
using ECommerce.Api.Dtos.Address;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Mappings;
using ECommerce.Api.Models;
using ECommerce.Api.Models.Enums;
using ECommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Moq;

namespace ECommerce.Api.Tests;

[TestClass]
public class SalesServiceTests
{
    private readonly Mock<ISalesRepository> _mockSalesRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ISalesService _salesService;

    public SalesServiceTests()
    {
        _mockSalesRepository = new Mock<ISalesRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        var mockLogger = new Mock<ILogger<SalesService>>();

        _salesService = new SalesService(
            _mockSalesRepository.Object,
            _mockProductRepository.Object,
            mockLogger.Object);
    }

    [TestMethod]
    public async Task CreateSaleAsync_WhenNoProductIsNull_ShouldReturnSuccessResult()
    {
        // Arrange 
        var customerId = "customerId123";

        var productsToReturnFromRepo = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                Description = "First",
                StockKeepingUnit = "FRS-PRD",
                Price = 44.37m,
                Discount = DiscountStatus.FivePercent,
                StockQuantity = 300,
                CategoryId = 1,
                Category = new Category {Id = 1, Name = "Category 1", Description = "First"}
            },
            new ()
            {
                Id = 2,
                Name = "Product 2",
                Description = "Second",
                StockKeepingUnit = "SND-PRD",
                Price = 66.99m,
                Discount = DiscountStatus.FifteenPercent,
                StockQuantity = 600,
                CategoryId = 2,
                Category = new Category {Id = 2, Name = "Category 2", Description = "Second"}
            },
            new ()
            {
                Id = 3,
                Name = "Product 3",
                Description = "Third",
                StockKeepingUnit = "TRD-PRD",
                Price = 9.99m,
                Discount = DiscountStatus.None,
                StockQuantity = 150,
                CategoryId = 3,
                Category = new Category {Id = 3, Name = "Category 3", Description = "Third"}
            }
        };

        var saleItems = new List<SaleItemRequest>
        {
            new()
            {
                ProductId = 1,
                Quantity = 10,
            },
            new()
            {
                ProductId = 2,
                Quantity = 20,
            },
            new()
            {
                ProductId = 3,
                Quantity  = 30
            }
        };

        var request = new CreateSaleRequest
        {
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            Notes = "Buying some items",
            Items = saleItems
        };

        var productIds = request.Items
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        _mockProductRepository
            .Setup(repo => repo.GetByIdsAsync(productIds))
            .ReturnsAsync(productsToReturnFromRepo);

        _mockSalesRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Sale>()))
            .Callback<Sale>(sale =>
            {
                sale.Id = 1;

                foreach (var item in sale.SaleItems)
                {
                    item.SaleId = sale.Id;
                }
            });
        
        // Act
        var result = await _salesService.CreateSaleAsync(request, customerId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);
        result.Value.CustomerId.Should().Contain(customerId);

        _mockSalesRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Sale>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateSaleAsync_WhenOneProductIsNotFound_ShouldReturnFailureResult()
    {
        // Arrange
        var customerId = "customerId123";
        var validProductId = 1;
        var invalidProductId = 99;

        var saleItems = new List<SaleItemRequest>
        {
            new()
            {
                ProductId = validProductId,
                Quantity = 10
            },
            new()
            {
                ProductId = invalidProductId,
                Quantity = 14
            }
        };

        var request = new CreateSaleRequest
        {
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            Notes = "Buying some items",
            Items = saleItems
        };

        var productsFromRepo = new List<Product>
        {
            new()
            {
                Id = validProductId,
                Name = "Valid Product",
                Price = 10m,
                StockQuantity = 50
            }
        };

        _mockProductRepository
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(productsFromRepo);
        
        // Act
        var result = await _salesService.CreateSaleAsync(request, customerId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Product with Id {invalidProductId} not found");

        _mockSalesRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Sale>()), Times.Never);
    }

    [TestMethod]
    public async Task CreateSaleAsync_WhenProductQuantityIsLessThanSaleQuantity_ShouldReturnFailureResult()
    {
        // Arrange
        var customerId = "customerId123";

        var productsToReturnFromRepo = new List<Product>()
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                Description = "First",
                StockKeepingUnit = "FRS-PRD",
                Price = 44.37m,
                Discount = DiscountStatus.FivePercent,
                StockQuantity = 10,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Category 1", Description = "First" }
            },
            new()
            {
                Id = 2,
                Name = "Product 2",
                Description = "Second",
                StockKeepingUnit = "SND-PRD",
                Price = 66.99m,
                Discount = DiscountStatus.FifteenPercent,
                StockQuantity = 10,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Category 2", Description = "Second" }
            },
            new()
            {
                Id = 3,
                Name = "Product 3",
                Description = "Third",
                StockKeepingUnit = "TRD-PRD",
                Price = 9.99m,
                Discount = DiscountStatus.None,
                StockQuantity = 10,
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Category 3", Description = "Third" }
            }
        };

        var saleItems = new List<SaleItemRequest>
        {
            new()
            {
                ProductId = 1,
                Quantity = 30,
            },
            new()
            {
                ProductId = 2,
                Quantity = 20,
            },
            new()
            {
                ProductId = 3,
                Quantity = 30
            }
        };

        var request = new CreateSaleRequest
        {
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            Notes = "Buying some items",
            Items = saleItems
        };

        var productIds = request.Items
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        _mockProductRepository
            .Setup(repo => repo.GetByIdsAsync(productIds))
            .ReturnsAsync(productsToReturnFromRepo);
        
        // Act
        var result = await _salesService.CreateSaleAsync(request, customerId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Insufficient stock");

        _mockSalesRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Sale>()), Times.Never);
    }

    [TestMethod]
    public async Task CreateSaleAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        // Arrange
        var customerId = "customerId123";

        var productsToReturnFromRepo = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Product 1",
                Description = "First",
                StockKeepingUnit = "FRS-PRD",
                Price = 44.37m,
                Discount = DiscountStatus.FivePercent,
                StockQuantity = 300,
                CategoryId = 1,
                Category = new Category { Id = 1, Name = "Category 1", Description = "First" }
            },
            new()
            {
                Id = 2,
                Name = "Product 2",
                Description = "Second",
                StockKeepingUnit = "SND-PRD",
                Price = 66.99m,
                Discount = DiscountStatus.FifteenPercent,
                StockQuantity = 600,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Category 2", Description = "Second" }
            },
            new()
            {
                Id = 3,
                Name = "Product 3",
                Description = "Third",
                StockKeepingUnit = "TRD-PRD",
                Price = 9.99m,
                Discount = DiscountStatus.None,
                StockQuantity = 150,
                CategoryId = 3,
                Category = new Category { Id = 3, Name = "Category 3", Description = "Third" }
            }
        };

        var saleItems = new List<SaleItemRequest>
        {
            new()
            {
                ProductId = 1,
                Quantity = 10,
            },
            new()
            {
                ProductId = 2,
                Quantity = 20
            },
            new()
            {
                ProductId = 3,
                Quantity = 30
            }
        };

        var request = new CreateSaleRequest
        {
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            Notes = "Buying some items",
            Items = saleItems
        };

        var productIds = request.Items
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        _mockProductRepository
            .Setup(repo => repo.GetByIdsAsync(productIds))
            .ReturnsAsync(productsToReturnFromRepo);

        _mockSalesRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Sale>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));
        
        // Act
        var result = await _salesService.CreateSaleAsync(request, customerId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("Database error occurred");
    }

    [TestMethod]
    public async Task UpdateSaleStatusAsync_WhenSaleToUpdateIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Processing,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };

        var updatedStatus = SaleStatus.Completed;

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);
        
        // Act
        var result = await _salesService.UpdateSaleStatusAsync(saleToReturnFromRepo.Id, updatedStatus);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should()
            .Contain(
                $"The status of Sale #{saleToReturnFromRepo.Id} has now been changed to: {updatedStatus.GetDisplayName()}");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.Is<Sale>(s => s.Status == updatedStatus)), Times.Once);
    }

    [TestMethod]
    public async Task UpdateSaleStatusAsync_WhenSaleToUpdateIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var saleId = 1;
        var updatedStatus = SaleStatus.Completed;

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Sale?)null);
        
        // Act 
        var result = await _salesService.UpdateSaleStatusAsync(saleId, updatedStatus);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Sale with Id {saleId} not found");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Sale>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateSaleStatusAsync_WhenRepositoryThrowsArgumentNullException_ShouldReturnFailureResult()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Processing,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };

        var updatedStatus = SaleStatus.Completed;

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);

        _mockSalesRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Sale>()))
            .ThrowsAsync(new ArgumentNullException(nameof(Sale)));
        
        // Act
        var result = await _salesService.UpdateSaleStatusAsync(saleToReturnFromRepo.Id, updatedStatus);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("There was an error updating the Sale status");
    }

    [TestMethod]
    public async Task CancelSaleAsync_WhenSaleToCancelIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Processing,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };

        var canceledStatus = SaleStatus.Canceled;
        
        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);
        
        // Act
        var result = await _salesService.CancelSaleAsync(saleToReturnFromRepo.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().Contain($"Sale has been canceled, Status is now: {canceledStatus}");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Sale>()), Times.Once);
    }

    [TestMethod]
    public async Task CancelSaleAsync_WhenSaleToCancelIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var saleId = 1;

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(saleId))
            .ReturnsAsync((Sale?)null);
        
        // Act
        var result = await _salesService.CancelSaleAsync(saleId);
        
        // Assert 
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Be($"Sale with Id {saleId} not found");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Sale>()), Times.Never);
    }

    [TestMethod]
    public async Task CancelSaleAsync_WhenSaleStatusIsWrongForCancellation_ShouldReturnFailureResult()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Refunded,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);
        
        // Act
        var result = await _salesService.CancelSaleAsync(saleToReturnFromRepo.Id);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Be($"Unable to cancel a Sale with status: {saleToReturnFromRepo.Status}");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Sale>()), Times.Never);
    }

    [TestMethod]
    public async Task CancelSaleAsync_WhenRepositoryThrowsArgumentNullException_ShouldReturnFailureResult()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Processing,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);
        
        _mockSalesRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Sale>()))
            .ThrowsAsync(new ArgumentNullException(nameof(Sale)));
        
        // Act
        var result = await _salesService.CancelSaleAsync(saleToReturnFromRepo.Id);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("There was an error canceling the Sale");
    }
    
    [TestMethod]
    public async Task RefundSaleAsync_WhenSaleToRefundIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Completed,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };
        
        var refundStatus = SaleStatus.Refunded;
        
        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);
        
        // Act
        var result = await _salesService.RefundSaleAsync(saleToReturnFromRepo.Id);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().Contain($"Sale has been refunded. Status is now {refundStatus}");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.Is<Sale>(s => s.Status == refundStatus)), Times.Once);
    }
    
    [TestMethod]
    public async Task RefundSaleAsync_WhenSaleToRefundIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var saleId = 1;

        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Sale?)null);
        
        // Act 
        var result = await _salesService.RefundSaleAsync(saleId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Sale with Id {saleId} not found");

        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Sale>()), Times.Never);
    }
    
    [TestMethod]
    public async Task RefundProductAsync_WhenSaleStatusIsNotCompleted_ShouldReturnFailureMessage()
    {
        // Arrange
        var saleToReturnFromRepo = new Sale
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            CustomerId = "customerId123",
            TotalPrice = 45.99m,
            Status = SaleStatus.Pending,
            Notes = "Needed to buy a few items",
            IsDeleted = false,
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            SaleItems = new List<SaleProduct>()
        };
        
        _mockSalesRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(saleToReturnFromRepo);
        
        // Act
        var result = await _salesService.RefundSaleAsync(saleToReturnFromRepo.Id);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Unable to refund a Sale with status: {saleToReturnFromRepo.Status}");
        
        _mockSalesRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Sale>()), Times.Never);
    }
}