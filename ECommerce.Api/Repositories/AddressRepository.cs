using ECommerce.Api.Data;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<PagedList<Address>> GetAllAddressesAsync(string customerId, PaginationParams paginationParams)
    {
        var query = _context.Addresses
            .Include(a => a.Customer)
            .Where(a => a.ApplicationUserId == customerId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(paginationParams.Filter))
        {
            query = query.Where(a => a.AddressType.ToLower() == paginationParams.Filter.ToLower());
        }

        query = paginationParams.OrderBy switch
        {
            "idAsc" => query.OrderBy(a => a.Id),
            "idDesc" => query.OrderByDescending(a => a.Id),
            _ => query.OrderBy(p => p.AddressType)
        };

        return await PagedList<Address>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<Address?> GetAddressByIdAsync(string customerId, int addressId)
    {
        return await _context.Addresses
            .Where(a => a.ApplicationUserId == customerId)
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == addressId);
    }
}