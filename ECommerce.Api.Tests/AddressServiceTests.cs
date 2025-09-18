using ECommerce.Api.Dtos.Address.Request;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Identity;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Models;
using ECommerce.Api.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ECommerce.Api.Tests;

[TestClass]
public class AddressServiceTests
{
    private readonly Mock<IAddressRepository> _mockAddressRepository;
    private readonly IAddressService _addressService;

    public AddressServiceTests()
    {
        _mockAddressRepository = new Mock<IAddressRepository>();
        var mockLogger = new Mock<ILogger<AddressService>>();

        _addressService = new AddressService(
            _mockAddressRepository.Object,
            mockLogger.Object);
    }

    [TestMethod]
    public async Task AddAddressAsync_WhenRepositoryDoesNotThrowDbException_ShouldReturnSuccessResult()
    {
        // Arrange
        var customerId = "CustomerId123";

        var request = new CreateAddressRequest
        {
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            AddressType = "Home"
        };

        // Act
        var result = await _addressService.AddAddressAsync(customerId, request);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _mockAddressRepository
            .Verify(repo => repo.AddAddressAsync(It.Is<Address>(a => a.ApplicationUserId == customerId)), Times.Once);
    }

    [TestMethod]
    public async Task AddAddressAsync_WhenRepositoryThrowsDbException_ShouldReturnFailureResult()
    {
        // Arrange
        var customerId = "CustomerId123";
        
        var request = new CreateAddressRequest
        {
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            AddressType = "Home"
        };
        
        _mockAddressRepository
            .Setup(repo => repo.AddAddressAsync(It.IsAny<Address>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));
        
        // Act
        var result = await _addressService.AddAddressAsync(customerId, request);
        
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("Database error occurred");
    }

    [TestMethod]
    public async Task UpdateAddressAsync_WhenAddressToUpdateIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var customerId = "CustomerId123";
        var addressId = 1;

        var request = new UpdateAddressRequest
        {
            StreetNumber = "123456",
            StreetName = "Updated Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            AddressType = "Updated Type"
        };

        var addressToReturn = new Address
        {
            Id = addressId,
            ApplicationUserId = customerId,
            Customer = new ApplicationUser(),
            StreetNumber = request.StreetNumber,
            StreetName = request.StreetName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
            AddressType = request.AddressType
        };

        _mockAddressRepository
            .Setup(repo => repo.GetAddressByIdAsync(customerId, addressId))
            .ReturnsAsync(addressToReturn);
        
        // Act 
        var result = await _addressService.UpdateAddressAsync(customerId, addressId, request);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(addressId);

        _mockAddressRepository
            .Verify(repo => repo.UpdateAddressAsync(It.IsAny<Address>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAddressAsync_WhenAddressToUpdateIsNull_ShouldReturnFailureResult()
    {
        // Arrange 
        var customerId = "CustomerId123";
        var addressId = 1;

        var request = new UpdateAddressRequest
        {
            StreetNumber = "123456",
            StreetName = "Updated Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            AddressType = "Updated Type",
        };

        _mockAddressRepository
            .Setup(repo => repo.GetAddressByIdAsync(customerId, addressId))
            .ReturnsAsync((Address?)null);
        
        // Act 
        var result = await _addressService.UpdateAddressAsync(customerId, addressId, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain($"Address with Id {addressId} not found");

        _mockAddressRepository
            .Verify(repo => repo.UpdateAddressAsync(It.IsAny<Address>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateAddressAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        // Arrange
        var customerId = "CustomerId123";
        var addressId = 1;

        var request = new UpdateAddressRequest
        {
            StreetNumber = "123456",
            StreetName = "Update Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            AddressType = "Updated Type"
        };

        var addressToReturn = new Address
        {
            Id = addressId,
            ApplicationUserId = customerId,
            Customer = new ApplicationUser(),
            StreetNumber = request.StreetNumber,
            StreetName = request.StreetName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
            AddressType = request.AddressType
        };

        _mockAddressRepository
            .Setup(repo => repo.GetAddressByIdAsync(customerId, addressId))
            .ReturnsAsync(addressToReturn);

        _mockAddressRepository
            .Setup(repo => repo.UpdateAddressAsync(It.IsAny<Address>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));
        
        // Act
        var result = await _addressService.UpdateAddressAsync(customerId, addressId, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
        result.ErrorMessage.Should().Contain("Database error occurred");
    }

    [TestMethod]
    public async Task DeleteAddressAsync_WhenAddressToDeleteIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var customerId = "customerId123";
        var addressId = 1;
        
        var addressToReturn = new Address
        {
            Id = addressId,
            ApplicationUserId = customerId,
            Customer = new ApplicationUser(),
            StreetNumber = "123456",
            StreetName = "Main Street",
            City = "New York City",
            State = "New York",
            Country = "USA",
            ZipCode = "654321",
            AddressType = "Home"
        };

        _mockAddressRepository
            .Setup(repo => repo.GetAddressByIdAsync(customerId, addressId))
            .ReturnsAsync(addressToReturn);
        
        // Act
        var result = await _addressService.DeleteAddressAsync(customerId, addressId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(addressId);

        _mockAddressRepository
            .Verify(repo => repo.DeleteAddressAsync(It.IsAny<Address>()), Times.Once);
    }
}