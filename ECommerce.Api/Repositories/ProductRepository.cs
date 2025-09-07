using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(
        ApplicationDbContext context,
        ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Product product)
    {
        try
        {
            await _context.Products.AddAsync(product);
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

    public async Task UpdateAsync(Product product)
    {
        try
        {
            _context.Products.Update(product);
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

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
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

    public async Task<PagedList<Product>> GetAllAsync(PaginationParams paginationParams)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(paginationParams.EntityName))
            {
                query = query.Where(p => p.Category.Name.ToLower() == paginationParams.EntityName.ToLower());
            }

            query = paginationParams.OrderBy switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            return await PagedList<Product>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
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

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        try
        {
            return await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
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