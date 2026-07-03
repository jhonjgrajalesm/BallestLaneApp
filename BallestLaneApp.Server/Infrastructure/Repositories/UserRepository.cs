using BallestLaneApp.Infrastructure.Persistence;
using BallestLaneApp.Server.Application.Interfaces.Persistence;
using BallestLaneApp.Server.Application.Interfaces.Repositories;
using BallestLaneApp.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BallestLaneApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(user => user.Tasks)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();

        return await _context.Users
            .FirstOrDefaultAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();

        return await _context.Users
            .AnyAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task AddAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(ApplicationUser user)
    {
        _context.Users.Update(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}