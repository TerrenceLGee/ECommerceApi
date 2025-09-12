using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Sales.Request;
using ECommerce.Presentation.Dtos.Sales.Response;

namespace ECommerce.Presentation.Interfaces;

public interface ISalesApiService
{
    Task<Result<SaleResponse?>> CreateSaleAsync(CreateSaleRequest saleRequest);
    Task<Result<string?>> UpdateSaleAsync(int id, UpdateSaleStatusRequest updateRequest);
    Task<Result<string?>> RefundSaleAsync(int id);
    Task<Result<string?>> CancelSaleAsync(int id);
    Task<Result<string?>> UserCancelSaleAsync(int id);
    Task<Result<List<SaleResponse>?>> GetAllSalesAsync(int pageNumber, int pageSize);
    Task<Result<List<SaleResponse>?>> GetAllSalesForUserAsync(int pageNumber, int pageSize);
    Task<Result<SaleResponse?>> GetSaleForUserByIdAsync(int saleId);
    Task<Result<SaleResponse?>> GetSaleByIdAsync(int saleId);
    Task<Result<int>> GetCountOfSalesAsync();
    Task<Result<int>> GetCountOfSalesForUserAsync();
}