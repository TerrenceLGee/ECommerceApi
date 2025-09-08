using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Sales.Request;
using ECommerce.Api.Dtos.Sales.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Models.Enums;

namespace ECommerce.Api.Interfaces.Services;

public interface ISalesService
{
    Task<Result<SaleResponse>> CreateSaleAsync(CreateSaleRequest request, string customerId);
    Task<Result<string>> UpdateSaleStatusAsync(int saleId, SaleStatus updatedStatus);
    Task<Result<string>> CancelSaleAsync(int saleId);
    Task<Result<string>> RefundSaleAsync(int saleId);
    Task<Result<string>> UserCancelSaleAsync(string userId, int saleId);
    Task<Result<PagedList<SaleResponse>>> GetAllSalesAsync(PaginationParams paginationParams);
    Task<Result<SaleResponse>> GetSaleByIdAsync(int id);
    Task<Result<PagedList<SaleResponse>>> GetUserSalesAsync(string userId, PaginationParams paginationParams);
    Task<Result<SaleResponse>> GetUserSaleByIdAsync(string userId, int saleId);
}