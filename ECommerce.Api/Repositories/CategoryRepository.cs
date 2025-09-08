using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(
        ApplicationDbContext context,
        ILogger<CategoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task AddAsync(Category category)
    {
        try
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogCritical("{errorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred");
            throw;
        }
    }

    public async Task UpdateAsync(Category category)
    {
        try
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogCritical("{errorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {errorMessage}", ex.Message);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _context.Categories
                .AnyAsync(c => c.Id == id);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("{errorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {errorMessage}", ex.Message);
            throw;
        }
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("{errorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {errorMessage}", ex.Message);
            throw;
        }
    }

    public async Task<PagedList<Category>> GetCategoriesAsync(PaginationParams paginationParams)
    {
        try
        {
            var query = _context.Categories
                .Include(c => c.Products)
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationParams.EntityName))
            {
                query = query.Where(c => c.Name.ToLower() == paginationParams.EntityName.ToLower());
            }

            query = paginationParams.OrderBy switch
            {
                "nameDesc" => query.OrderByDescending(c => c.Name),
                _ => query.OrderBy(c => c.Name)
            };

            return await PagedList<Category>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("{errorMessage}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {errorMessage}", ex.Message);
            throw;
        }
    }
}