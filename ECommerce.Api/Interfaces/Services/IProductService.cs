using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Products.Response;
using ECommerce.Api.Dtos.Shared.Pagination;

namespace ECommerce.Api.Interfaces.Services;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request);
    Task<Result<ProductResponse>> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<Result<ProductResponse>> DeleteProductAsync(int id);
    Task<Result<ProductResponse>> GetProductByIdAsync(int id);
    Task<Result<PagedList<ProductResponse>>> GetAllProductsAsync(PaginationParams paginationParams);
    Task<Result<int>> GetCountOfProductsAsync();
}