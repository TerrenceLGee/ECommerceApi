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

}