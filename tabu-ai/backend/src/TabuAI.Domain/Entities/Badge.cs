namespace TabuAI.Domain.Entities;

public class Badge
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public BadgeType Type { get; set; }
    public int RequiredValue { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}

public class UserBadge
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BadgeId { get; set; }
    public DateTime EarnedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Badge Badge { get; set; } = null!;
}

public enum BadgeType
{
    GamesWon = 1,           // Oyun kazanma sayısı
    ConsecutiveWins = 2,    // Ardışık kazanımlar
    PerfectPrompts = 3,     // Mükemmel promptlar
    FastCompleter = 4,      // Hızlı tamamlama
    TabuAvoidance = 5,      // Tabu kelimelerden kaçınma
    Streak = 6,             // Seri başarı
    CategoryMaster = 7,     // Kategori uzmanı
    LevelUp = 8,           // Seviye atlama
    HighScore = 9,         // Yüksek skor
    Dedication = 10        // Özveri (günlük giriş)
}