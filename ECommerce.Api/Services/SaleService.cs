using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Sales.Request;
using ECommerce.Api.Dtos.Sales.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Mappings;
using ECommerce.Api.Models;
using ECommerce.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services;

public class SaleService : ISalesService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SaleService> _logger;
    
    public SaleService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        ILogger<SaleService> logger)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, string customerId)
    {
        try
        {
            var productIds = request.Items
                .Select(i => i.ProductId)
                .Distinct();
            var products = await _productRepository.GetByIdsAsync(productIds);

            foreach (var item in request.Items)
            {
                var product = products
                    .FirstOrDefault(p => p.Id == item.ProductId);

                if (product is null)
                {
                    _logger.LogError("Product with Id {id} not found.", item.ProductId);
                    return Result<SaleResponse>.Fail($"Product with Id {item.ProductId} not found.");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    _logger.LogError("Insufficient stock for '{name}'. Available: {.stockQuantity}, Requested: {Quantity}",
                        product.Name, product.StockQuantity, item.Quantity);
                    
                    return Result<SaleResponse>.Fail(
                        $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                }
            }

            var newSale = request.MapFromCreateSaleRequestToSale(customerId);

            var totalPrice = 0.0m;

            foreach (var item in request.Items)
            {
                var product = products
                    .First(p => p.Id == item.ProductId);
                var saleProduct = new SaleProduct
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    DiscountPrice = product.Price - ((((int)product.Discount) / 100.0m) * product.Price)
                };

                newSale.SaleItems.Add(saleProduct);
                product.StockQuantity -= item.Quantity;
                totalPrice += saleProduct.FinalPrice;

            }

            newSale.TotalPrice = totalPrice;

            await _saleRepository.AddAsync(newSale);

            var response = newSale.MapFromSaleToSaleResponse();

            return Result<SaleResponse>.Ok(response);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogCritical("There was an error creating the Sale: {errorMessage}", ex);
            return Result<SaleResponse>.Fail($"There was an error creating the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<SaleResponse>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
        
    }

    public async Task<Result> UpdateSaleStatusAsync(int saleId, SaleStatus updatedStatus)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> CancelSaleAsync(int saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RefundSaleAsync(int saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UserCancelSaleAsync(string userId, int saleId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<SaleResponse>>> GetAllSalesAsync(PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<SaleResponse>> GetSaleByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<SaleResponse>>> GetUserSalesAsync(string userId, PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<SaleResponse>> GetUserSaleByIdAsync(string userId, int saleId)
    {
        throw new NotImplementedException();
    }
}