using System.Data.Common;
using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Products.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Repositories;
using ECommerce.Api.Interfaces.Services;
using ECommerce.Api.Mappings;

namespace ECommerce.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            if (!await _categoryRepository.ExistsAsync(request.CategoryId))
            {
                _logger.LogError("Unable to add a product to Category with id {id} because that category is not found.",
                    request.CategoryId);
                return Result<ProductResponse>.Fail(
                    $"Unable to add a product to Category with id {request.CategoryId} because that category is not found");
            }

            var category = request.MapFromCreateProductRequestToProduct();
            await _productRepository.AddAsync(category);
            return Result<ProductResponse>.Ok(category.MapProductToProductResponse());
        }
        catch (DbException ex)
        {
            _logger.LogCritical("There was an error creating the product: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"There was an error creating the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var productToUpdate = await _productRepository.GetByIdAsync(id);

            if (productToUpdate is null)
            {
                _logger.LogError("Product with Id {id} not found", id);
                return Result<ProductResponse>.Fail($"Product with Id {id} not found");
            }

            if (!await _categoryRepository.ExistsAsync(request.CategoryId))
            {
                _logger.LogError(
                    "Unable to update the Product's Category to category with Id {id} because that category is not found.",
                    request.CategoryId);
                return Result<ProductResponse>.Fail(
                    $"Unable to update the Product's Category to category with Id {id} because category is not found");
            }
            
            request.UpdateProductFromUpdateProductRequest(productToUpdate);
            await _productRepository.UpdateAsync(productToUpdate);
            return Result<ProductResponse>.Ok(productToUpdate.MapProductToProductResponse());
        }
        catch (DbException ex)
        {
            _logger.LogCritical("There was an error updating the product: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"There was an error updating the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse>> DeleteProductAsync(int id)
    {
        try
        {
            var productToDelete = await _productRepository.GetByIdAsync(id);

            if (productToDelete is null)
            {
                _logger.LogError("Product with id {id} not found", id);
                return Result<ProductResponse>.Fail($"Product with id {id} not found");
            }

            productToDelete.IsDeleted = true;
            await _productRepository.UpdateAsync(productToDelete);
            return Result<ProductResponse>.Ok(productToDelete.MapProductToProductResponse());

        }
        catch (DbException ex)
        {
            _logger.LogCritical("There was an error deleting the product: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"There was an error deleting the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                _logger.LogError("Product with Id {id} not found.", id);
                return Result<ProductResponse>.Fail($"Product with Id {id} not found");
            }
            
            return Result<ProductResponse>.Ok(product.MapProductToProductResponse());
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving the product: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"There was an error retrieving the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<ProductResponse>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<ProductResponse>>> GetAllProductsAsync(PaginationParams paginationParams)
    {
        try
        {
            var products = await _productRepository.GetAllAsync(paginationParams);

            var productResponseDtos = products.Items
                .Select(p => p.MapProductToProductResponse())
                .ToList();

            var pagedResponse = new PagedList<ProductResponse>(
                productResponseDtos,
                products.TotalCount,
                products.CurrentPage,
                products.PageSize);

            return Result<PagedList<ProductResponse>>.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogCritical("There was an error retrieving all products: {errorMessage}", ex.Message);
            return Result<PagedList<ProductResponse>>.Fail($"There was an error retrieving all products: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("An unexpected error occurred: {errorMessage}", ex.Message);
            return Result<PagedList<ProductResponse>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetCountOfProductsAsync()
    {
        try
        {
            var count = await _productRepository.GetCountOfProductsAsync();

            return Result<int>.Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("There was an unexpected error retrieving the count of products from the database: {errorMessage}", ex.Message);
            return Result<int>.Fail(
                $"There was an unexpected error retrieving the count of products from the database: {ex.Message}");
        }
    }
}