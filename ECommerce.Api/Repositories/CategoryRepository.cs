using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id);
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagedList<Category>> GetCategoriesAsync(PaginationParams paginationParams)
    {
        var query = _context.Categories
            .Include(c => c.Products)
            .AsQueryable();

        if (!string.IsNullOrEmpty(paginationParams.Filter))
        {
            query = query.Where(c => c.Name.ToLower() == paginationParams.Filter.ToLower());
        }

        query = paginationParams.OrderBy switch
        {
            "nameDesc" => query.OrderByDescending(c => c.Name),
            _ => query.OrderBy(c => c.Name)
        };

        return await PagedList<Category>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
    }
}