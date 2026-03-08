using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TabuAI.Application.Common.DTOs;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Enums;
using TabuAI.Domain.Interfaces;

namespace TabuAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(IUnitOfWork unitOfWork, ILogger<ActivityController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get activity feed for a user's friends
    /// </summary>
    [HttpGet("feed/{userId}")]
    [ProducesResponseType(typeof(List<ActivityLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActivityLogDto>>> GetFeed(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            // Get user's friends
            var friendships = await _unitOfWork.Friendships.FindAsync(
                f => (f.RequesterId == userId || f.AddresseeId == userId) && f.Status == FriendshipStatus.Accepted);

            var friendIds = friendships
                .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
                .ToList();

            // Include the user's own activities
            friendIds.Add(userId);

            // Get activities from friends
            var activities = await _unitOfWork.ActivityLogs.FindAsync(a => friendIds.Contains(a.UserId));

            var sortedActivities = activities
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new List<ActivityLogDto>();
            foreach (var activity in sortedActivities)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(activity.UserId);
                result.Add(new ActivityLogDto
                {
                    Id = activity.Id,
                    UserId = activity.UserId,
                    UserDisplayName = user?.DisplayName ?? user?.Username ?? "Bilinmeyen",
                    Type = activity.Type.ToString(),
                    Description = activity.Description,
                    ScoreEarned = activity.ScoreEarned,
                    CreatedAt = activity.CreatedAt
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting activity feed");
            return StatusCode(500, new { message = "Aktivite akışı alınamadı" });
        }
    }

    /// <summary>
    /// Get a specific user's activity history
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<ActivityLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActivityLogDto>>> GetUserActivity(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return NotFound(new { message = "Kullanıcı bulunamadı" });

            var activities = await _unitOfWork.ActivityLogs.FindAsync(a => a.UserId == userId);

            var sortedActivities = activities
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = sortedActivities.Select(a => new ActivityLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserDisplayName = user.DisplayName ?? user.Username,
                Type = a.Type.ToString(),
                Description = a.Description,
                ScoreEarned = a.ScoreEarned,
                CreatedAt = a.CreatedAt
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user activity");
            return StatusCode(500, new { message = "Kullanıcı aktiviteleri alınamadı" });
        }
    }
}
