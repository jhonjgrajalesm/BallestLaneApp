using BallestLaneApp.Server.Application.Interfaces.Repositories;

namespace BallestLaneApp.Server.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    ITaskItemRepository Tasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}