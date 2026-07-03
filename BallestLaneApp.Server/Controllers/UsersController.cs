using System.Security.Claims;
using BallestLaneApp.Server.DTOs.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallestLaneApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(ApiResponse<object>.Fail(
                    "Unauthorized request.",
                    new List<string> { "User identifier was not found in the token." }));
            }

            var data = new
            {
                UserId = userId,
                Email = email,
                FullName = name
            };

            return Ok(ApiResponse<object>.Ok(
                data,
                "Current user retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving current user.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Fail(
                    "An unexpected error occurred while retrieving the current user.",
                    new List<string> { "Please try again later." }));
        }
    }
}