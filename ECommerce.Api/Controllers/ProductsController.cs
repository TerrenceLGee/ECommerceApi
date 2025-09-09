using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Products.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts([FromQuery] PaginationParams paginationParams)
    {
        var result = await _productService.GetAllProductsAsync(paginationParams);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var result = await _productService.CreateProductAsync(request);

        if (result.IsFailure)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new {id = result.Value!.Id},
            result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct([FromRoute] int id,
        [FromBody] UpdateProductRequest request)
    {
        var result = await _productService.UpdateProductAsync(id, request);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponse>> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (result.IsFailure)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
}