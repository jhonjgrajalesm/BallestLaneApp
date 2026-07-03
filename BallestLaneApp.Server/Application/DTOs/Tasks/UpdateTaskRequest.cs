using BallestLaneApp.Server.Domain.Enums;

namespace BallestLaneApp.Server.Application.DTOs.Tasks;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public TaskItemStatus Status { get; set; }
}