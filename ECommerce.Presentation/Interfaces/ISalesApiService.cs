using ECommerce.Presentation.Dtos.Sales.Request;
using ECommerce.Presentation.Dtos.Sales.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface ISalesApiService
{
    Task<SaleResponse?> CreateSaleAsync(CreateSaleRequest saleRequest);
    Task<string?> UpdateSaleAsync(int id, UpdateSaleStatusRequest updateRequest);
    Task<string?> RefundSaleAsync(int id);
    Task<string?> CancelSaleAsync(int id);
    Task<string?> UserCancelSaleAsync(int id);
    Task<PagedList<SaleResponse>?> GetAllSalesAsync(PaginationParams paginationParams);
    Task<PagedList<SaleResponse>?> GetAllSalesForUserAsync(PaginationParams paginationParams);
    Task<SaleResponse?> GetSaleForUserByIdAsync(int saleId);
    Task<SaleResponse?> GetSaleByIdAsync(int saleId);
}