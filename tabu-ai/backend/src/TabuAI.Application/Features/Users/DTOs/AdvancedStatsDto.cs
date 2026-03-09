namespace TabuAI.Application.Features.Users.DTOs;

public class PromptAnalysisChartDto
{
    public List<PromptAnalysisDataPoint> DataPoints { get; set; } = new();
}

public class PromptAnalysisDataPoint
{
    public DateTime Date { get; set; }
    public double AveragePromptLength { get; set; }
    public double SuccessRate { get; set; }
    public int UniqueWordsUsed { get; set; }
    public int GamesPlayed { get; set; }
    public double AverageScore { get; set; }
}

public class StyleAnalysisDto
{
    public string PlayerTitle { get; set; } = string.Empty;
    public string TitleEmoji { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<StyleTraitDto> Traits { get; set; } = new();
    public PromptStyleDto PromptStyle { get; set; } = new();
    public List<CategoryPerformanceDto> TopCategories { get; set; } = new();
}

public class StyleTraitDto
{
    public string Name { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class PromptStyleDto
{
    public double AverageLength { get; set; }
    public double AdjectiveRatio { get; set; }
    public double VerbRatio { get; set; }
    public double UniqueWordRatio { get; set; }
    public string DominantStyle { get; set; } = string.Empty;
}

public class CategoryPerformanceDto
{
    public string Category { get; set; } = string.Empty;
    public int GamesPlayed { get; set; }
    public double SuccessRate { get; set; }
    public double AverageScore { get; set; }
}

public class BadgeGalleryDto
{
    public List<BadgeShowcaseDto> EarnedBadges { get; set; } = new();
    public List<BadgeShowcaseDto> LockedBadges { get; set; } = new();
    public List<Guid> ShowcaseBadgeIds { get; set; } = new();
    public int TotalBadges { get; set; }
    public int EarnedCount { get; set; }
    public double CompletionPercentage { get; set; }
}

public class BadgeShowcaseDto
{
    public Guid BadgeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string BadgeType { get; set; } = string.Empty;
    public int RequiredValue { get; set; }
    public int CurrentProgress { get; set; }
    public double ProgressPercentage { get; set; }
    public DateTime? EarnedAt { get; set; }
    public bool IsEarned { get; set; }
    public string Rarity { get; set; } = "Common";
}

public class UpdateShowcaseRequest
{
    public List<Guid> BadgeIds { get; set; } = new();
}
