using BallestLaneApp.Server.Application.DTOs.Tasks;

namespace BallestLaneApp.Server.Application.Interfaces.Services;

public interface ITaskItemService
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<TaskResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TaskResponse> CreateAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<TaskResponse?> UpdateAsync(
        Guid id,
        Guid userId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}