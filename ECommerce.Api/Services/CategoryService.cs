using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Categories.Request;
using ECommerce.Api.Dtos.Categories.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }
    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        try
        {
            var category = request.MapFromCreateCategoryRequestToCategory();
            await _categoryRepository.AddAsync(category);
            return Result<CategoryResponse>.Ok(category.MapCategoryToCategoryResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogCritical("There was an error creating the category: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an error creating the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        try
        {
            var categoryToUpdate = await _categoryRepository.GetCategoryByIdAsync(id);

            if (categoryToUpdate is null)
            {
                _logger.LogError("Category with Id {id} not found.", id);
                return Result<CategoryResponse>.Fail($"Category with Id {id} not found");
            }
            
            request.MapFromUpdateCategoryRequestToCategory(categoryToUpdate);
            await _categoryRepository.UpdateAsync(categoryToUpdate);

            return Result<CategoryResponse>.Ok(categoryToUpdate.MapCategoryToCategoryResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogCritical("There was an error updating the category: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an error updating the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse>> DeleteCategoryAsync(int id)
    {
        try
        {
            var categoryToDelete = await _categoryRepository.GetCategoryByIdAsync(id);

            if (categoryToDelete is null)
            {
                _logger.LogError("Category with Id {id} not found.", id);
                return Result<CategoryResponse>.Fail($"Category with Id {id} not found");
            }

            categoryToDelete.IsDeleted = true;
            await _categoryRepository.UpdateAsync(categoryToDelete);
            return Result<CategoryResponse>.Ok(categoryToDelete.MapCategoryToCategoryResponse());
        }
        catch (DbUpdateException ex)
        {
            _logger.LogCritical("There was an error deleting the category: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an error deleting the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);

            if (category is null)
            {
                _logger.LogError("Category with Id {id} not found.", id);
                return Result<CategoryResponse>.Fail($"Category with Id {id} not found");
            }

            return Result<CategoryResponse>.Ok(category.MapCategoryToCategoryResponse());
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving the category: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an error retrieving the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error: {errorMessage}", ex.Message);
            return Result<CategoryResponse>.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams paginationParams)
    {
        try
        {
            var categories = await _categoryRepository.GetCategoriesAsync(paginationParams);

            var categoryResponseDtos = categories.Items
                .Select(c => c.MapCategoryToCategoryResponse())
                .ToList();

            var pagedResponse = new PagedList<CategoryResponse>(
                categoryResponseDtos,
                categories.TotalCount,
                categories.CurrentPage,
                categories.PageSize);

            return Result<PagedList<CategoryResponse>>.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving all categories: {errorMessage}", ex.Message);
            return Result<PagedList<CategoryResponse>>.Fail($"There was an error retrieving all categories: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error: {errorMessage}", ex.Message);
            return Result<PagedList<CategoryResponse>>.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCategoriesCountAsync()
    {
        try
        {
            var count = await _categoryRepository.GetCountOfCategoriesAsync();

            return Result<int>.Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving the count of categories from the database: {errorMessage}", ex.Message);
            return Result<int>.Fail(
                $"There was an unexpected error retrieving the count of categories from the database: {ex.Message}");
        }
    }
}