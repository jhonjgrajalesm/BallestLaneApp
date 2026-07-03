using BallestLaneApp.Server.Domain.Enums;
using BallestLaneApp.Server.Domain.Entities;

namespace BallestLaneApp.Server.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public TaskItemStatus Status { get; private set; }

    public DateTime DueDate { get; private set; }

    public Guid UserId { get; private set; }

    public ApplicationUser User { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private TaskItem()
    {
        // Required by EF Core
    }

    public TaskItem(
        string title,
        string description,
        DateTime dueDate,
        Guid userId)
    {
        Id = Guid.NewGuid();
        Title = title.Trim();
        Description = description.Trim();
        DueDate = dueDate;
        UserId = userId;
        Status = TaskItemStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string title,
        string description,
        DateTime dueDate)
    {
        if (Status == TaskItemStatus.Completed)
        {
            throw new InvalidOperationException("Completed tasks cannot be edited.");
        }

        Title = title.Trim();
        Description = description.Trim();
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(TaskItemStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsInProgress()
    {
        Status = TaskItemStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        Status = TaskItemStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = TaskItemStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        Status = TaskItemStatus.Pending;
        UpdatedAt = DateTime.UtcNow;
    }
}