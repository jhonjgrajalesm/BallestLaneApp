using BallestLaneApp.Server.Application.DTOs.Users;
using BallestLaneApp.Server.Application.Interfaces.Persistence;
using BallestLaneApp.Server.Application.Interfaces.Security;
using BallestLaneApp.Server.Application.Interfaces.Services;
using BallestLaneApp.Server.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BallestLaneApp.Server.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;

    public UserService(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = new PasswordHasher<ApplicationUser>();
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRegisterRequest(request);

        var emailExists = await _unitOfWork.Users.EmailExistsAsync(
            request.Email,
            cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new ApplicationUser(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash: string.Empty);

        var passwordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        user.ChangePassword(passwordHash);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Token = token
        };
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateLoginRequest(request);

        var user = await _unitOfWork.Users.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Token = token
        };
    }

    private static void ValidateRegisterRequest(RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new ArgumentException("First name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new ArgumentException("Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (!request.Email.Contains('@'))
        {
            throw new ArgumentException("Email format is invalid.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }

        if (request.Password.Length < 6)
        {
            throw new ArgumentException("Password must contain at least 6 characters.");
        }
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }
    }
}