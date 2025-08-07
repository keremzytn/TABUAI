using Microsoft.EntityFrameworkCore;
using TabuAI.Domain.Entities;
using System.Text.Json;

namespace TabuAI.Infrastructure.Data;

public class TabuAIDbContext : DbContext
{
    public TabuAIDbContext(DbContextOptions<TabuAIDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<UserBadge> UserBadges { get; set; }
    public DbSet<UserStatistic> UserStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(254);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Word Configuration
        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TargetWord).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            
            // JSON column for TabuWords
            entity.Property(e => e.TabuWords)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
                );
        });

        // GameSession Configuration
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserPrompt).IsRequired();
            entity.Property(e => e.AiResponse).HasMaxLength(500);
            entity.Property(e => e.AiFeedback);
            
            // JSON column for Suggestions
            entity.Property(e => e.Suggestions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
                );

            // Foreign Keys
            entity.HasOne(e => e.User)
                .WithMany(e => e.GameSessions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Word)
                .WithMany(e => e.GameSessions)
                .HasForeignKey(e => e.WordId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Badge Configuration
        modelBuilder.Entity<Badge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
        });

        // UserBadge Configuration
        modelBuilder.Entity<UserBadge>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.UserBadges)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Badge)
                .WithMany(e => e.UserBadges)
                .HasForeignKey(e => e.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: user can earn each badge only once
            entity.HasIndex(e => new { e.UserId, e.BadgeId }).IsUnique();
        });

        // UserStatistic Configuration
        modelBuilder.Entity<UserStatistic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MetricName).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.UserStatistics)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Words
        var words = new List<Word>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TargetWord = "Uçak",
                TabuWords = new List<string> { "hava", "kanat", "yolcu", "pilot", "uçmak" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                TargetWord = "Bilgisayar",
                TabuWords = new List<string> { "ekran", "klavye", "fare", "işlemci", "teknoloji" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                TargetWord = "Yapay Zeka",
                TabuWords = new List<string> { "AI", "makine", "öğrenme", "algoritma", "robot" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<Word>().HasData(words);

        // Seed Badges
        var badges = new List<Badge>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "İlk Adım",
                Description = "İlk oyununu tamamladın!",
                IconUrl = "/badges/first-game.svg",
                Type = BadgeType.GamesWon,
                RequiredValue = 1,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Prompt Ustası",
                Description = "10 mükemmel prompt yazdın!",
                IconUrl = "/badges/perfect-prompts.svg",
                Type = BadgeType.PerfectPrompts,
                RequiredValue = 10,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tabu Kaçkını",
                Description = "50 oyunda hiç tabu kelime kullanmadın!",
                IconUrl = "/badges/tabu-avoidance.svg",
                Type = BadgeType.TabuAvoidance,
                RequiredValue = 50,
                CreatedAt = DateTime.UtcNow
            }
        };

        modelBuilder.Entity<Badge>().HasData(badges);
    }
}