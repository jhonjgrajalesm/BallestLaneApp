using BallestLaneApp.Server.Domain.Enums;

namespace BallestLaneApp.Server.Application.DTOs.Tasks;

public class TaskResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskItemStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public Guid UserId { get; set; }

    public string UserEmail { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}