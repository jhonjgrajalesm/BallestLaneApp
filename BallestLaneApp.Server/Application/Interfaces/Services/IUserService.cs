using BallestLaneApp.Server.Application.DTOs.Users;

namespace BallestLaneApp.Server.Application.Interfaces.Services;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default);

    Task<AuthResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}