namespace TabuAI.Application.Features.Friends.DTOs;

public class FriendDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Level { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public DateTime FriendSince { get; set; }
}

public class FriendRequestDto
{
    public Guid RequestId { get; set; }
    public Guid FromUserId { get; set; }
    public string FromUsername { get; set; } = string.Empty;
    public string? FromDisplayName { get; set; }
    public DateTime SentAt { get; set; }
}

public class UserSearchResultDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Level { get; set; } = string.Empty;
    public string? FriendshipStatus { get; set; } // null = no relationship, "Pending", "Accepted"
}
