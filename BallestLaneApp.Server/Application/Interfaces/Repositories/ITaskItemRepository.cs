using BallestLaneApp.Server.Domain.Entities;
using BallestLaneApp.Server.Domain.Enums;

namespace BallestLaneApp.Server.Application.Interfaces.Repositories;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByStatusAsync(
        TaskItemStatus status,
        CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);

    void Update(TaskItem taskItem);

    void Delete(TaskItem taskItem);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> BelongsToUserAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default);

}