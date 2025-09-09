using ECommerce.Api.Dtos.Categories.Request;
using ECommerce.Api.Dtos.Categories.Response;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCategories([FromQuery] PaginationParams paginationParams)
    {
        var result = await _categoryService.GetAllCategoriesAsync(paginationParams);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateCategoryAsync(request);

        if (result.IsFailure)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromRoute] int id,
        [FromBody] UpdateCategoryRequest request)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, request);

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
    public async Task<ActionResult<CategoryResponse>> DeleteCategory([FromRoute] int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
}