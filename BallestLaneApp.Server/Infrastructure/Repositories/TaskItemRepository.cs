using BallestLaneApp.Infrastructure.Persistence;
using BallestLaneApp.Server.Application.Interfaces.Repositories;
using BallestLaneApp.Server.Domain.Entities;
using BallestLaneApp.Server.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BallestLaneApp.Server.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _context;

    public TaskItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(task => task.User)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Include(task => task.User)
            .OrderBy(task => task.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .OrderBy(task => task.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByStatusAsync(
        TaskItemStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(task => task.Status == status)
            .OrderBy(task => task.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        TaskItem taskItem,
        CancellationToken cancellationToken = default)
    {
        await _context.Tasks.AddAsync(taskItem, cancellationToken);
    }

    public void Update(TaskItem taskItem)
    {
        _context.Tasks.Update(taskItem);
    }

    public void Delete(TaskItem taskItem)
    {
        _context.Tasks.Remove(taskItem);
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AnyAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<bool> BelongsToUserAsync(
        Guid taskId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AnyAsync(
                task => task.Id == taskId && task.UserId == userId,
                cancellationToken);
    }

}