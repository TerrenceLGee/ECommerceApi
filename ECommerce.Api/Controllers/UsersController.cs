using ECommerce.Api.Dtos.Shared.Pagination;
using ECommerce.Api.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams paginationParams)
    {
        var result = await _userService.GetAllUsersAsync(paginationParams);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById([FromRoute] string userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{userId}/addresses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAddressesByUserId([FromRoute] string userId,
        [FromQuery] PaginationParams paginationParams)
    {
        var result = await _userService.GetUserAddressesByIdAsync(userId, paginationParams);

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("count")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCountOfUsers()
    {
        var result = await _userService.GetUserCountAsync();

        if (result.IsFailure)
        {
            return result.ErrorMessage!.Contains("Not found")
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }
}