using BallestLaneApp.Server.Application.DTOs.Users;
using BallestLaneApp.Server.Application.Interfaces.Services;
using BallestLaneApp.Server.DTOs.API;
using Microsoft.AspNetCore.Mvc;

namespace BallestLaneApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }


    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _userService.RegisterAsync(request, cancellationToken);

            return Created(
                $"/api/users/{response.UserId}",
                ApiResponse<AuthResponse>.Ok(response, "User registered successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AuthResponse>.Fail(
                "Validation error.",
                new List<string> { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<AuthResponse>.Fail(
                "User registration failed.",
                new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while registering user.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<AuthResponse>.Fail(
                    "An unexpected error occurred while registering the user.",
                    new List<string> { "Please try again later." }));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _userService.LoginAsync(request, cancellationToken);

            if (response is null)
            {
                return Unauthorized(ApiResponse<AuthResponse>.Fail(
                    "Invalid credentials.",
                    new List<string> { "Email or password is incorrect." }));
            }

            return Ok(ApiResponse<AuthResponse>.Ok(
                response,
                "Login completed successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AuthResponse>.Fail(
                "Validation error.",
                new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while logging in.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<AuthResponse>.Fail(
                    "An unexpected error occurred while logging in.",
                    new List<string> { "Please try again later." }));
        }
    }

    [HttpGet("public")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public IActionResult PublicEndpoint()
    {
        return Ok(ApiResponse<string>.Ok(
            "This endpoint is public.",
            "Public endpoint reached successfully."));
    }
}