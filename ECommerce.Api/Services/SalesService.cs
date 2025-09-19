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
using Microsoft.OpenApi.Extensions;

namespace ECommerce.Api.Services;

public class SalesService : ISalesService
{
    private readonly ISalesRepository _salesRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<SalesService> _logger;
    
    public SalesService(
        ISalesRepository salesRepository,
        IProductRepository productRepository,
        ILogger<SalesService> logger)
    {
        _salesRepository = salesRepository;
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
                    DiscountPrice = product.Price - ((((int)product.Discount) / 100.0m) * product.Price),
                    Product = product
                };

                newSale.SaleItems.Add(saleProduct);
                product.StockQuantity -= item.Quantity;
                totalPrice += saleProduct.FinalPrice;

            }

            newSale.TotalPrice = totalPrice;

            await _salesRepository.AddAsync(newSale);

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

    public async Task<Result<string>> UpdateSaleStatusAsync(int saleId, SaleStatus updatedStatus)
    {
        try
        {
            var saleToUpdate = await _salesRepository.GetByIdAsync(saleId);

            if (saleToUpdate is null)
            {
                _logger.LogError("Sale with Id {id} not found.", saleId);
                return Result<string>.Fail($"Sale with Id {saleId} not found");
            }

            saleToUpdate.Status = updatedStatus;
            saleToUpdate.UpdatedAt = DateTime.UtcNow;
            await _salesRepository.UpdateAsync(saleToUpdate);

            return Result<string>.Ok(
                $"The status of Sale #{saleId} has now been changed to: {saleToUpdate.Status.GetDisplayName()}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error updating the Sale status: {errorMessage}", ex);
            return Result<string>.Fail($"There was an error updating the Sale status: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<string>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<string>> CancelSaleAsync(int saleId)
    {
        try
        {
            var saleToCancel = await _salesRepository.GetByIdAsync(saleId);

            if (saleToCancel is null)
            {
                _logger.LogError("Sale with Id {id} not found.", saleId);
                return Result<string>.Fail($"Sale with Id {saleId} not found");
            }

            if (saleToCancel.Status != SaleStatus.Pending && saleToCancel.Status != SaleStatus.Processing)
            {
                _logger.LogError("Unable to cancel a Sale with status: {saleToCancel}", saleToCancel.Status);
                return Result<string>.Fail($"Unable to cancel a Sale with status: {saleToCancel.Status}");
            }

            saleToCancel.Status = SaleStatus.Canceled;
            await _salesRepository.UpdateAsync(saleToCancel);
            return Result<string>.Ok($"Sale has been canceled, Status is now: {saleToCancel.Status}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error canceling the Sale: {errorMessage}", ex);
            return Result<string>.Fail($"There was an error canceling the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<string>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<string>> RefundSaleAsync(int saleId)
    {
        try
        {
            var saleToRefund = await _salesRepository.GetByIdAsync(saleId);

            if (saleToRefund is null)
            {
                _logger.LogError("Sale with Id {id} not found.", saleId);
                return Result<string>.Fail($"Sale with Id {saleId} not found");
            }

            if (saleToRefund.Status != SaleStatus.Completed)
            {
                _logger.LogError("Unable to refund a Sale with status: {saleToRefund}", saleToRefund.Status);
                return Result<string>.Fail($"Unable to refund a Sale with status: {saleToRefund.Status}");
            }

            saleToRefund.Status = SaleStatus.Refunded;
            await _salesRepository.UpdateAsync(saleToRefund);
            return Result<string>.Ok($"Sale has been refunded. Status is now {saleToRefund.Status}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error refunding the Sale: {errorMessage}", ex);
            return Result<string>.Fail($"There was an error refunding the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<string>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<string>> UserCancelSaleAsync(string userId, int saleId)
    {
        try
        {
            var userSaleToCancel = await _salesRepository.GetUserSaleByIdAsync(userId, saleId);

            if (userSaleToCancel is null)
            {
                _logger.LogError("Sale with Id {id} not found.", saleId);
                return Result<string>.Fail($"Sale with Id {saleId} not found");
            }

            if (userSaleToCancel.Status != SaleStatus.Pending && userSaleToCancel.Status != SaleStatus.Processing)
            {
                _logger.LogError("Unable to cancel a Sale with status: {saleToCancel}", userSaleToCancel.Status);
                return Result<string>.Fail($"Unable to cancel a sale with status: {userSaleToCancel.Status}");
            }

            userSaleToCancel.Status = SaleStatus.Canceled;
            await _salesRepository.UpdateAsync(userSaleToCancel);
            return Result<string>.Ok($"Sale has been canceled, Status is now: {userSaleToCancel.Status}");
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error canceling the Sale: {errorMessage}", ex);
            return Result<string>.Fail($"There was an error canceling the Sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<string>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<SaleResponse>>> GetAllSalesAsync(PaginationParams paginationParams)
    {
        try
        {
            var sales = await _salesRepository.GetAllAsync(paginationParams);
            var saleResponseDtos = sales.Items
                .Select(s => s.MapFromSaleToSaleResponse())
                .ToList();

            var pagedResponse = new PagedList<SaleResponse>(
                saleResponseDtos,
                sales.TotalCount,
                sales.CurrentPage,
                sales.PageSize);

            return Result<PagedList<SaleResponse>>.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sales: {errorMessage}", ex);
            return Result<PagedList<SaleResponse>>.Fail($"There was an error retrieving sales: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<PagedList<SaleResponse>>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<SaleResponse>> GetSaleByIdAsync(int id)
    {
        try
        {
            var sale = await _salesRepository.GetByIdAsync(id);

            if (sale is null)
            {
                _logger.LogError("Sale with Id {id} not found", id);
                return Result<SaleResponse>.Fail($"Sale with Id {id} found found");
            }

            return Result<SaleResponse>.Ok(sale.MapFromSaleToSaleResponse());
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sale: {errorMessage}", ex);
            return Result<SaleResponse>.Fail($"There was an error retrieving sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<SaleResponse>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<SaleResponse>>> GetUserSalesAsync(string userId, PaginationParams paginationParams)
    {
        try
        {
            var sales = await _salesRepository.GetByUserIdAsync(userId, paginationParams);

            var userSaleResponseDtos = sales.Items
                .Select(s => s.MapFromSaleToSaleResponse())
                .ToList();

            var pagedResponse = new PagedList<SaleResponse>(
                userSaleResponseDtos,
                sales.TotalCount,
                sales.CurrentPage,
                sales.PageSize);

            return Result<PagedList<SaleResponse>>.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sales: {errorMessage}", ex);
            return Result<PagedList<SaleResponse>>.Fail($"There was an error retrieving sales: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<PagedList<SaleResponse>>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<SaleResponse>> GetUserSaleByIdAsync(string userId, int saleId)
    {
        try
        {
            var sale = await _salesRepository.GetUserSaleByIdAsync(userId, saleId);

            if (sale is null)
            {
                _logger.LogError("Sale with Id {id} not found", saleId);
                return Result<SaleResponse>.Fail($"Sale with Id {saleId} not found");
            }

            return Result<SaleResponse>.Ok(sale.MapFromSaleToSaleResponse());
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving sale: {errorMessage}", ex);
            return Result<SaleResponse>.Fail($"There was an error retrieving sale: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error that occurred:  {errorMessage}", ex);
            return Result<SaleResponse>.Fail($"There was an unexpected error that occurred: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCountOfSalesAsync()
    {
        try
        {
            var count = await _salesRepository.GetCountOfSalesAsync();

            return Result<int>.Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving the count of sales from the database: {errorMessage}", ex.Message);
            return Result<int>.Fail(
                $"There was an unexpected error retrieving the count of sales from the database: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCountOfUserSalesAsync(string userId)
    {
        try
        {
            var count = await _salesRepository.GetCountOfUserSalesAsync(userId);

            return Result<int>.Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving the count of sales from the database: {errorMessage}", ex.Message);
            return Result<int>.Fail(
                $"There was an unexpected error retrieving the count of sales from the database: {ex.Message}");
        }
    }
}