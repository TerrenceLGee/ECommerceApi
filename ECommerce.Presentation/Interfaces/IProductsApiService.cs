using ECommerce.Presentation.Common.Results;
using ECommerce.Presentation.Dtos.Products.Request;
using ECommerce.Presentation.Dtos.Products.Response;

namespace ECommerce.Presentation.Interfaces;

public interface IProductsApiService
{
    Task<Result<List<ProductResponse>?>> GetProductsAsync(int pageNumber, int pageSize);
    Task<Result<ProductResponse?>> GetProductByIdAsync(int id);
    Task<Result<ProductResponse?>> CreateProductAsync(CreateProductRequest request);
    Task<Result<ProductResponse?>> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<Result<ProductResponse?>> DeleteProductAsync(int id);
    Task<Result<int>> GetCountOfProductsAsync();
}