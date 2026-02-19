using TabuAI.Domain.Entities;

namespace TabuAI.Application.Features.Users.DTOs;

public class UserStatisticDto
{
    public string MetricName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public StatisticType Type { get; set; }
    public string FormattedValue { get; set; } = string.Empty;
}
