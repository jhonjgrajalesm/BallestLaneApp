using BallestLaneApp.Server.Application.DTOs.Tasks;
using BallestLaneApp.Server.Application.Interfaces.Persistence;
using BallestLaneApp.Server.Application.Interfaces.Services;
using BallestLaneApp.Server.Domain.Entities;
using BallestLaneApp.Server.Domain.Enums;

namespace BallestLaneApp.Server.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskItemService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var tasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);

        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<TaskResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _unitOfWork.Tasks.GetByUserIdAsync(
            userId,
            cancellationToken);

        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<TaskResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(
            id,
            cancellationToken);

        return task is null ? null : MapToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var user = await _unitOfWork.Users.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Cannot create tasks for inactive users.");
        }

        var taskItem = new TaskItem(
            request.Title,
            request.Description,
            request.DueDate,
            request.UserId);

        await _unitOfWork.Tasks.AddAsync(taskItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdTask = await _unitOfWork.Tasks.GetByIdAsync(
            taskItem.Id,
            cancellationToken);

        return MapToResponse(createdTask!);
    }

    public async Task<TaskResponse?> UpdateAsync(
        Guid id,
        Guid userId,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateUpdateRequest(request);

        var taskItem = await _unitOfWork.Tasks.GetByIdAsync(
            id,
            cancellationToken);

        if (taskItem is null)
        {
            return null;
        }

        if (taskItem.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update your own tasks.");
        }

        taskItem.UpdateDetails(
            request.Title,
            request.Description,
            request.DueDate);

        taskItem.ChangeStatus(request.Status);

        _unitOfWork.Tasks.Update(taskItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(taskItem);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var taskItem = await _unitOfWork.Tasks.GetByIdAsync(
            id,
            cancellationToken);

        if (taskItem is null)
        {
            return false;
        }

        if (taskItem.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own tasks.");
        }

        _unitOfWork.Tasks.Delete(taskItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static void ValidateCreateRequest(CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        if (request.Title.Length > 150)
        {
            throw new ArgumentException("Task title cannot exceed 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException("Task description is required.");
        }

        if (request.Description.Length > 1000)
        {
            throw new ArgumentException("Task description cannot exceed 1000 characters.");
        }

        if (request.DueDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Due date cannot be in the past.");
        }

        if (request.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.");
        }
    }

    private static void ValidateUpdateRequest(UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.");
        }

        if (request.Title.Length > 150)
        {
            throw new ArgumentException("Task title cannot exceed 150 characters.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException("Task description is required.");
        }

        if (request.Description.Length > 1000)
        {
            throw new ArgumentException("Task description cannot exceed 1000 characters.");
        }

        if (request.DueDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Due date cannot be in the past.");
        }

        if (!Enum.IsDefined(typeof(TaskItemStatus), request.Status))
        {
            throw new ArgumentException("Invalid task status.");
        }
    }

    private static TaskResponse MapToResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            UserId = task.UserId,
            UserEmail = task.User?.Email ?? string.Empty,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}