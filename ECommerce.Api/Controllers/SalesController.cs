using System.Security.Claims;
using ECommerce.Api.Common.Results;
using ECommerce.Api.Dtos.Sales.Request;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Identity;
using ECommerce.Api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;

    public SalesController(
        ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _salesService.CreateSaleAsync(request, customerId);

        if (result.IsFailure)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(GetSaleForUserById),
            new {id = result.Value!.Id},
            result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSales([FromQuery] PaginationParams paginationParams)
    {
        var result = await _salesService.GetAllSalesAsync(paginationParams);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSaleById([FromRoute] int id)
    {
        var result = await _salesService.GetSaleByIdAsync(id);

        if (!result.IsSuccess)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("me/sales")]
    [Authorize]
    public async Task<IActionResult> GetSalesByUserId([FromQuery] PaginationParams paginationParams)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _salesService.GetUserSalesAsync(customerId, paginationParams);

        if (result.IsFailure)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("me/sales/{id}")]
    [Authorize]
    public async Task<IActionResult> GetSaleForUserById([FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _salesService.GetUserSaleByIdAsync(customerId, id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSaleStatus([FromRoute] int id, [FromBody] UpdateSaleStatusRequest request)
    {
        var result = await _salesService.UpdateSaleStatusAsync(id, request.UpdatedStatus);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminCancelSale([FromRoute] int id)
    {
        var result = await _salesService.CancelSaleAsync(id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost("me/sales/cancel/{id}")]
    [Authorize]
    public async Task<IActionResult> UserCancelSale(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _salesService.UserCancelSaleAsync(userId!, id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}/refund")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RefundSale(int id)
    {
        var result = await _salesService.RefundSaleAsync(id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
    
}