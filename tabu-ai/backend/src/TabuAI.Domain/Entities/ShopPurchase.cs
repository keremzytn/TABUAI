namespace TabuAI.Domain.Entities;

public class ShopPurchase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    
    public User User { get; set; } = null!;
}
