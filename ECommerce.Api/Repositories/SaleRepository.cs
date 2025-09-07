using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SaleRepository> _logger;

    public SaleRepository(
        ApplicationDbContext context,
        ILogger<SaleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Sale sale)
    {
        try
        {
            await _context.Sales.AddAsync(sale);
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

    public async Task UpdateAsync(Sale sale)
    {
        try
        {
            _context.Sales.Update(sale);
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

    public async Task<Sale?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
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

    public async Task<Sale?> GetUserSaleByIdAsync(string userId, int saleId)
    {
        try
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Where(s => s.CustomerId == userId)
                .FirstOrDefaultAsync(s => s.Id == saleId);
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

    public async Task<PagedList<Sale>> GetAllAsync(PaginationParams paginationParams)
    {
        try
        {
            var query = _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .AsQueryable();

            query = paginationParams.OrderBy switch
            {
                "dateAsc" => query.OrderBy(s => s.CreatedAt),
                "dateDesc" => query.OrderByDescending(s => s.CreatedAt),
                "customerIdAsync" => query.OrderBy(s => s.CustomerId),
                "customerIdDesc" => query.OrderByDescending(s => s.CustomerId),
                "saleIdDesc" => query.OrderByDescending(s => s.Id),
                _ => query.OrderBy(s => s.Id)
            };

            return await PagedList<Sale>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
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

    public async Task<PagedList<Sale>> GetByUserIdAsync(string userId, PaginationParams paginationParams)
    {
        try
        {
            var query = _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .Where(s => s.CustomerId == userId);

            query = paginationParams.OrderBy switch
            {
                "dateAsc" => query.OrderBy(s => s.CreatedAt),
                "dateDesc" => query.OrderByDescending(s => s.CreatedAt),
                "saleIdDesc" => query.OrderByDescending(s => s.Id),
                _ => query.OrderBy(s => s.Id)
            };

            return await PagedList<Sale>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
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