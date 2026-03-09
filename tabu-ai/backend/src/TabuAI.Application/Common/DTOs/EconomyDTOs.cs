namespace TabuAI.Application.Common.DTOs;

public class CoinBalanceDto
{
    public int Balance { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public double StreakMultiplier { get; set; }
    public bool HasStreakShield { get; set; }
    public int DoubleCoinGamesLeft { get; set; }
    public List<CoinTransactionDto> RecentTransactions { get; set; } = new();
}

public class CoinTransactionDto
{
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PurchaseHintRequest
{
    public string GameSessionId { get; set; } = string.Empty;
}

public class PurchaseHintResult
{
    public bool Success { get; set; }
    public string? Hint { get; set; }
    public int RemainingCoins { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ShopItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
    public bool IsOwned { get; set; }
}

public class PurchaseItemRequest
{
    public string ItemId { get; set; } = string.Empty;
}

public class PurchaseResult
{
    public bool Success { get; set; }
    public int RemainingCoins { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
}

public class EquipItemRequest
{
    public string Type { get; set; } = string.Empty;
    public string? ItemId { get; set; }
}
