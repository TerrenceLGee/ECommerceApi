using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AddressRepository> _logger;

    public AddressRepository(
        ApplicationDbContext context,
        ILogger<AddressRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAddressAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAddressAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAddressAsync(Address address)
    {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedList<Address>> GetAllAsync(string customerId, PaginationParams paginationParams)
    {
        var query = _context.Addresses
            .Include(a => a.Customer)
            .Where(a => a.CustomerId == customerId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(paginationParams.Filter))
        {
            query = query.Where(a => a.Description != null && a.Description!.ToLower() == paginationParams.Filter.ToLower());
        }

        query = paginationParams.OrderBy switch
        {
            "idAsc" => query.OrderBy(a => a.Id),
            "idDesc" => query.OrderByDescending(a => a.Id),
            _ => query.OrderBy(p => p.Description)
        };

        return await PagedList<Address>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<Address?> GetAddressById(string customerId, int addressId)
    {
        return await _context.Addresses
            .Include(a => a.Customer)
            .Where(a => a.CustomerId == customerId)
            .FirstOrDefaultAsync(a => a.Id == addressId);
    }
}