using ECommerce.Api.Dtos.Categories.Request;
using ECommerce.Api.Dtos.Shared.Pagination;
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
public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly ICategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockLogger = new Mock<ILogger<CategoryService>>();

        _categoryService = new CategoryService(
            _mockCategoryRepository.Object,
            mockLogger.Object);
    }

    [TestMethod]
    public async Task CreateCategoryAsync_WhenRepositoryDoesNotThrowException_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "New Category",
            Description = "Newly added description",
            IsActive = true,
        };

        
        // Act
        var result = await _categoryService.CreateCategoryAsync(request);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _mockCategoryRepository
            .Verify(repo => repo.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateCategoryAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        var request = new CreateCategoryRequest
        {
            Name = "New Category",
            Description = "Newly create description",
            IsActive = true,
        };

        _mockCategoryRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Category>()))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));
        
        // Act 
        var result = await _categoryService.CreateCategoryAsync(request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("There was an error creating the category");
    }

    [TestMethod]
    public async Task UpdateCategoryAsync_WhenCategoryToUpdateIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Name = "Category To Update",
            Description = "Updated Category",
            IsActive = true,
        };

        var categoryToReturnFromRepo = new Category
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        };

        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(categoryToReturnFromRepo);
        
        // Act
        var result = await _categoryService.UpdateCategoryAsync(categoryId, request);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);

        _mockCategoryRepository
            .Verify(repo => repo.UpdateAsync(categoryToReturnFromRepo), Times.Once);
    }

    [TestMethod]
    public async Task UpdateCategoryAsync_WhenCategoryToUpdateIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Name = "Category To Update",
            Description = "Updated Category",
            IsActive = true
        };

        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);
        
        // Act
        var result = await _categoryService.UpdateCategoryAsync(categoryId, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain($"Category with Id {categoryId} not found");

        _mockCategoryRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateCategoryAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        // Arrange 
        var request = new UpdateCategoryRequest
        {
            Name = "Category To Update",
            Description = "Updated Category",
            IsActive = true
        };

        var categoryToUpdate = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Category will be updated",
            IsActive = true,
        };

        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(categoryToUpdate);

        _mockCategoryRepository
            .Setup(repo => repo.UpdateAsync(categoryToUpdate))
            .ThrowsAsync(new DbUpdateException("Database error occurred"));
        
        // Act
        var result = await _categoryService.UpdateCategoryAsync(categoryId, request);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("There was an error updating the category");
    }

    [TestMethod]
    public async Task DeleteCategoryAsync_WhenCategoryToDeleteIsNotNull_ShouldPerformSoftDeleteAndReturnSuccessResult()
    {
        // Arrange
        var categoryToReturnFromRepo = new Category
        {
            Id = 1,
            Name = "Category To Be Deleted",
            Description = "Category To Set Deletion Status To True",
            IsActive = true,
            IsDeleted = false,
        };

        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(categoryToReturnFromRepo);
        
        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);

        _mockCategoryRepository
            .Verify(repo => repo.UpdateAsync(It.Is<Category>(c => c.IsDeleted == true)), Times.Once);
    }

    [TestMethod]
    public async Task DeleteCategoryAsync_WhenCategoryToDeleteIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);
        
        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain($"Category with Id {categoryId} not found");

        _mockCategoryRepository
            .Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [TestMethod]
    public async Task DeleteCategoryAsync_WhenRepositoryThrowsDbUpdateException_ShouldReturnFailureResult()
    {
        // Arrange
        var categoryToDelete = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "Will be deleted soon",
            IsActive = true
        };
        
        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(categoryToDelete);

        _mockCategoryRepository
            .Setup(repo => repo.UpdateAsync(categoryToDelete))
            .Throws(new DbUpdateException("Database error occurred"));
        
        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("There was an error deleting the category");
    }

    [TestMethod]
    public async Task GetCategoryByIdAsync_WhenCategoryToReturnIsNotNull_ShouldReturnSuccessResult()
    {
        // Arrange
        var categoryToReturnFromRepo = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "A Category",
            IsActive = true
        };

        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync(categoryToReturnFromRepo);
        
        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(1);

        _mockCategoryRepository
            .Verify(repo => repo.GetCategoryByIdAsync(categoryId), Times.Once);
    }

    [TestMethod]
    public async Task GetCategoryByIdAsync_WhenCategoryToReturnIsNull_ShouldReturnFailureResult()
    {
        // Arrange
        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);
        
        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain($"Category with Id {categoryId} not found");
    }

    [TestMethod]
    public async Task GetCategoryByIdAsync_WhenRepositoryThrowsArgumentNullException_ShouldReturnFailureResult()
    {
        // Arrange
        var categoryId = 1;

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoryByIdAsync(categoryId))
            .ThrowsAsync(new ArgumentNullException(nameof(categoryId)));
        
        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain($"There was an error retrieving the category");
    }

    [TestMethod]
    public async Task GetAllCategoriesAsync_WhenRepositoryDoesNotThrowArgumentNullException_ShouldReturnSuccessResult()
    {
        // Arrange
        var category1 = new Category
        {
            Id = 1,
            Name = "Category 1",
            Description = "First",
            IsActive = true,
        };

        var category2 = new Category
        {
            Id = 2,
            Name = "Category 2",
            Description = "Second",
            IsActive = true
        };

        var categoriesToBeReturnedFromRepo = new List<Category>
        {
            category1,
            category2
        };

        var mockPagedList = new PagedList<Category>(categoriesToBeReturnedFromRepo, 2, 1, 10);

        _mockCategoryRepository
            .Setup(repo => repo.GetCategoriesAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(mockPagedList);
        
        // Act
        var result = await _categoryService.GetAllCategoriesAsync(new PaginationParams());
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.CurrentPage.Should().Be(1);
        result.Value.TotalCount.Should().Be(2);
        result.Value.PageSize.Should().Be(10);

        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.First().Name.Should().Be("Category 1");

        _mockCategoryRepository
            .Verify(repo => repo.GetCategoriesAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [TestMethod]
    public async Task GetAllCategoriesAsync_WhenRepositoryThrowsArgumentNullException_ShouldReturnFailureResult()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(repo => repo.GetCategoriesAsync(It.IsAny<PaginationParams>()))
            .ThrowsAsync(new ArgumentNullException(nameof(PaginationParams)));
        
        // Act
        var result = await _categoryService.GetAllCategoriesAsync(new PaginationParams());
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("There was an error retrieving all categories");
    }

    [TestMethod]
    public async Task GetCategoriesCountAsync_EnsureRepositoryReturnsANumber()
    {
        // Arrange
        int count = 3;

        _mockCategoryRepository
            .Setup(repo => repo.GetCountOfCategoriesAsync())
            .ReturnsAsync(count);
        
        // Act
        var result = await _categoryService.GetCategoriesCountAsync();
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(3);

        _mockCategoryRepository
            .Verify(repo => repo.GetCountOfCategoriesAsync(), Times.Once);
    }
}