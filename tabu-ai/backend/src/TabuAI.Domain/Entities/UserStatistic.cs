namespace TabuAI.Domain.Entities;

public class UserStatistic
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTime RecordedAt { get; set; }
    public StatisticType Type { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}

public enum StatisticType
{
    // Performance Metrics
    AveragePromptLength = 1,
    AverageCompletionTime = 2,
    SuccessRate = 3,
    
    // Engagement Metrics
    DailyPlayCount = 4,
    WeeklyPlayCount = 5,
    MonthlyPlayCount = 6,
    
    // Quality Metrics
    AveragePromptQuality = 7,
    TabuViolationRate = 8,
    
    // Progress Metrics
    CurrentStreak = 9,
    BestStreak = 10,
    TotalScore = 11,
    
    // Category Performance
    CategorySuccessRate = 12,
    DifficultySuccessRate = 13
}