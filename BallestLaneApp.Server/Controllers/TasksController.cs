using System.Security.Claims;
using BallestLaneApp.Server.Application.DTOs.Tasks;
using BallestLaneApp.Server.Application.Interfaces.Services;
using BallestLaneApp.Server.DTOs.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallestLaneApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskItemService taskItemService,
        ILogger<TasksController> logger)
    {
        _taskItemService = taskItemService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TaskResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TaskResponse>>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TaskResponse>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyTasks(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            var tasks = await _taskItemService.GetByUserIdAsync(
                userId,
                cancellationToken);

            return Ok(ApiResponse<IReadOnlyList<TaskResponse>>.Ok(
                tasks,
                "Tasks retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<IReadOnlyList<TaskResponse>>.Fail(
                "Unauthorized request.",
                new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving tasks.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<IReadOnlyList<TaskResponse>>.Fail(
                    "An unexpected error occurred while retrieving tasks.",
                    new List<string> { "Please try again later." }));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            var task = await _taskItemService.GetByIdAsync(id, cancellationToken);

            if (task is null)
            {
                return NotFound(ApiResponse<TaskResponse>.Fail(
                    "Task not found.",
                    new List<string> { $"Task with id '{id}' was not found." }));
            }

            if (task.UserId != userId)
            {
                return Forbid();
            }

            return Ok(ApiResponse<TaskResponse>.Ok(
                task,
                "Task retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<TaskResponse>.Fail(
                "Unauthorized request.",
                new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving task {TaskId}.", id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<TaskResponse>.Fail(
                    "An unexpected error occurred while retrieving the task.",
                    new List<string> { "Please try again later." }));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            request.UserId = userId;

            var createdTask = await _taskItemService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdTask.Id },
                ApiResponse<TaskResponse>.Ok(
                    createdTask,
                    "Task created successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(
                "Validation error.",
                new List<string> { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(
                "Task could not be created.",
                new List<string> { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<TaskResponse>.Fail(
                "Unauthorized request.",
                new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating task.");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<TaskResponse>.Fail(
                    "An unexpected error occurred while creating the task.",
                    new List<string> { "Please try again later." }));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<TaskResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            var updatedTask = await _taskItemService.UpdateAsync(
                id,
                userId,
                request,
                cancellationToken);

            if (updatedTask is null)
            {
                return NotFound(ApiResponse<TaskResponse>.Fail(
                    "Task not found.",
                    new List<string> { $"Task with id '{id}' was not found." }));
            }

            return Ok(ApiResponse<TaskResponse>.Ok(
                updatedTask,
                "Task updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(
                "Validation error.",
                new List<string> { ex.Message }));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                ApiResponse<TaskResponse>.Fail(
                    "Forbidden request.",
                    new List<string> { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<TaskResponse>.Fail(
                "Task could not be updated.",
                new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating task {TaskId}.", id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<TaskResponse>.Fail(
                    "An unexpected error occurred while updating the task.",
                    new List<string> { "Please try again later." }));
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();

            var deleted = await _taskItemService.DeleteAsync(
                id,
                userId,
                cancellationToken);

            if (!deleted)
            {
                return NotFound(ApiResponse<bool>.Fail(
                    "Task not found.",
                    new List<string> { $"Task with id '{id}' was not found." }));
            }

            return Ok(ApiResponse<bool>.Ok(
                true,
                "Task deleted successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                ApiResponse<bool>.Fail(
                    "Forbidden request.",
                    new List<string> { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting task {TaskId}.", id);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.Fail(
                    "An unexpected error occurred while deleting the task.",
                    new List<string> { "Please try again later." }));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdValue))
        {
            throw new UnauthorizedAccessException("User identifier was not found in the token.");
        }

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("User identifier in the token is invalid.");
        }

        return userId;
    }
}