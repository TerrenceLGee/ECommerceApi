using System.Security.Claims;
using ECommerce.Api.Dtos.Address.Request;
using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAddresses([FromQuery] PaginationParams paginationParams)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }
        var result = await _addressService.GetAllAddressesAsync(customerId, paginationParams);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetAddressById([FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.GetAddressByIdAsync(customerId, id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressRequest request)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.AddAddressAsync(customerId, request);

        if (result.IsFailure)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(
            nameof(GetAddressById),
            new {id = result.Value!.Id},
            result.Value);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request, [FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.UpdateAddressAsync(customerId, id, request);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAddress([FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.DeleteAddressAsync(customerId, id);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
}