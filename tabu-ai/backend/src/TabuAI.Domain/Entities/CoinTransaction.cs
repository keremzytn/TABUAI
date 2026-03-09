namespace TabuAI.Domain.Entities;

public class CoinTransaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Amount { get; set; }
    public CoinTransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; } = null!;
}

public enum CoinTransactionType
{
    GameWin = 1,
    StreakBonus = 2,
    DailyChallengeBonus = 3,
    HintPurchase = 4,
    AvatarPurchase = 5,
    BadgeShowcase = 6,
    LevelUpBonus = 7,
    ShopPurchase = 8,
    StreakMilestoneReward = 9,
    CardDesignPurchase = 10,
    StreakShieldUsed = 11
}
