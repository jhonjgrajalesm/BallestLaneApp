using BallestLaneApp.Server.Domain.Entities;

namespace BallestLaneApp.Server.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ApplicationUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);

    void Update(ApplicationUser user);

}