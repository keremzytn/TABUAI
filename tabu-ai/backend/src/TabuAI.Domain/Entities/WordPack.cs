namespace TabuAI.Domain.Entities;

public class WordPack
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "tr";
    public Guid CreatedByUserId { get; set; }
    public bool IsPublic { get; set; } = true;
    public bool IsApproved { get; set; } = false;
    public int PlayCount { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;
    public ICollection<Word> Words { get; set; } = new List<Word>();
}
