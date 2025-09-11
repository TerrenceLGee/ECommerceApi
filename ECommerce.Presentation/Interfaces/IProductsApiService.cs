using ECommerce.Presentation.Dtos.Products.Request;
using ECommerce.Presentation.Dtos.Products.Response;
using ECommerce.Presentation.Dtos.Shared.Pagination;

namespace ECommerce.Presentation.Interfaces;

public interface IProductsApiService
{
    Task<PagedList<ProductResponse>?> GetProductsAsync(PaginationParams paginationParams);
    Task<ProductResponse?> GetProductByIdAsync(int id);
    Task<ProductResponse?> CreateProductAsync(CreateProductRequest request);
    Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<ProductResponse?> DeleteProductAsync(int id);
}