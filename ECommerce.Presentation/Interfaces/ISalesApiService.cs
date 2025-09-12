using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Sales.Request;
using ECommerce.Presentation.Dtos.Sales.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface ISalesApiService
{
    Task<Result<SaleResponse?>> CreateSaleAsync(CreateSaleRequest saleRequest);
    Task<Result<string?>> UpdateSaleAsync(int id, UpdateSaleStatusRequest updateRequest);
    Task<Result<string?>> RefundSaleAsync(int id);
    Task<Result<string?>> CancelSaleAsync(int id);
    Task<Result<string?>> UserCancelSaleAsync(int id);
    Task<Result<PagedList<SaleResponse>?>> GetAllSalesAsync(PaginationParams paginationParams);
    Task<Result<PagedList<SaleResponse>?>> GetAllSalesForUserAsync(PaginationParams paginationParams);
    Task<Result<SaleResponse?>> GetSaleForUserByIdAsync(int saleId);
    Task<Result<SaleResponse?>> GetSaleByIdAsync(int saleId);
}