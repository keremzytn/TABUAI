using Microsoft.EntityFrameworkCore;
using TabuAI.Domain.Entities;
using TabuAI.Domain.Enums;
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
    public DbSet<GameAttempt> GameAttempts { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<UserBadge> UserBadges { get; set; }
    public DbSet<UserStatistic> UserStatistics { get; set; }

    public DbSet<Friendship> Friendships { get; set; }

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
            entity.Property(e => e.Role).IsRequired().HasDefaultValue(UserRole.User);
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
                )
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        });

        // GameSession Configuration
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserPrompt).IsRequired();
            entity.Property(e => e.AiResponse).HasMaxLength(500);
            
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

        // GameAttempt Configuration
        modelBuilder.Entity<GameAttempt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserPrompt).IsRequired();
            entity.Property(e => e.AiGuess).IsRequired().HasMaxLength(100);
            
            // JSON column for Suggestions
            entity.Property(e => e.Suggestions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!) ?? new List<string>()
                )
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

            entity.HasOne(e => e.GameSession)
                .WithMany(e => e.Attempts)
                .HasForeignKey(e => e.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
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

        // Friendship Configuration
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Requester)
                .WithMany(e => e.SentFriendRequests)
                .HasForeignKey(e => e.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Addressee)
                .WithMany(e => e.ReceivedFriendRequests)
                .HasForeignKey(e => e.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);

            // A user can only send one request to another user
            entity.HasIndex(e => new { e.RequesterId, e.AddresseeId }).IsUnique();
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Admin User
        var adminUser = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Username = "admin",
            Email = "admin@tabuai.com",
            DisplayName = "Admin User",
            Role = UserRole.Admin,
            PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgwd966VQRfAk5U5Z6.t.15v8vS6", // admin123
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsActive = true
        };

        modelBuilder.Entity<User>().HasData(adminUser);

        // Seed Words
        var words = new List<Word>
        {
            // Ulaşım
            new()
            {
                Id = Guid.Parse("d1b6a7b8-1234-5678-90ab-cdef12345678"),
                TargetWord = "Uçak",
                TabuWords = new List<string> { "hava", "kanat", "yolcu", "pilot", "uçmak" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                TargetWord = "Tren",
                TabuWords = new List<string> { "ray", "vagon", "istasyon", "lokomotif", "bilet" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                TargetWord = "Bisiklet",
                TabuWords = new List<string> { "pedal", "tekerlek", "zincir", "gidon", "sürmek" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Teknoloji
            new()
            {
                Id = Guid.Parse("c2a7b8c9-2345-6789-01cd-ef1234567890"),
                TargetWord = "Bilgisayar",
                TabuWords = new List<string> { "ekran", "klavye", "fare", "işlemci", "teknoloji" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("e3a8b9c0-3456-7890-12ef-1234567890ab"),
                TargetWord = "Yapay Zeka",
                TabuWords = new List<string> { "AI", "makine", "öğrenme", "algoritma", "robot" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                TargetWord = "Telefon",
                TabuWords = new List<string> { "aramak", "konuşmak", "numara", "mobil", "ekran" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                TargetWord = "İnternet",
                TabuWords = new List<string> { "web", "ağ", "bağlantı", "site", "çevrimiçi" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Bilim
            new()
            {
                Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                TargetWord = "Güneş",
                TabuWords = new List<string> { "ışık", "sıcak", "yıldız", "gündüz", "enerji" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                TargetWord = "DNA",
                TabuWords = new List<string> { "gen", "hücre", "kalıtım", "çift sarmal", "biyoloji" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Sanat
            new()
            {
                Id = Guid.Parse("a7777777-7777-7777-7777-777777777777"),
                TargetWord = "Resim",
                TabuWords = new List<string> { "boya", "tablo", "fırça", "tuval", "çizmek" },
                Category = "Sanat",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a8888888-8888-8888-8888-888888888888"),
                TargetWord = "Sürrealizm",
                TabuWords = new List<string> { "Dalí", "rüya", "bilinçaltı", "gerçeküstü", "sanat akımı" },
                Category = "Sanat",
                Difficulty = DifficultyLevel.Expert,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Yemek
            new()
            {
                Id = Guid.Parse("a9999999-9999-9999-9999-999999999999"),
                TargetWord = "Pizza",
                TabuWords = new List<string> { "hamur", "peynir", "dilim", "İtalyan", "fırın" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                TargetWord = "Baklava",
                TabuWords = new List<string> { "tatlı", "yufka", "şerbet", "fıstık", "Gaziantep" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Spor
            new()
            {
                Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                TargetWord = "Futbol",
                TabuWords = new List<string> { "top", "gol", "kaleci", "stadyum", "maç" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"),
                TargetWord = "Olimpiyat",
                TabuWords = new List<string> { "halka", "madalya", "spor", "meşale", "yarışma" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Tarih
            new()
            {
                Id = Guid.Parse("b4444444-4444-4444-4444-444444444444"),
                TargetWord = "Piramit",
                TabuWords = new List<string> { "Mısır", "firavun", "taş", "üçgen", "antik" },
                Category = "Tarih",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Doğa
            new()
            {
                Id = Guid.Parse("b5555555-5555-5555-5555-555555555555"),
                TargetWord = "Yanardağ",
                TabuWords = new List<string> { "lav", "patlama", "dağ", "magma", "kül" },
                Category = "Doğa",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Müzik
            new()
            {
                Id = Guid.Parse("b6666666-6666-6666-6666-666666666666"),
                TargetWord = "Gitar",
                TabuWords = new List<string> { "tel", "akort", "çalmak", "müzik", "pena" },
                Category = "Müzik",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("b7777777-7777-7777-7777-777777777777"),
                TargetWord = "Opera",
                TabuWords = new List<string> { "şarkı", "sahne", "soprano", "orkestra", "tiyatro" },
                Category = "Müzik",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<Word>().HasData(words);

        // Seed Badges
        var badges = new List<Badge>
        {
            new()
            {
                Id = Guid.Parse("f4a9b0c1-4567-8901-2301-234567890abc"),
                Name = "İlk Adım",
                Description = "İlk oyununu tamamladın!",
                IconUrl = "/badges/first-game.svg",
                Type = BadgeType.GamesWon,
                RequiredValue = 1,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("a5b0c1d2-5678-9012-3412-34567890abcd"),
                Name = "Prompt Ustası",
                Description = "10 mükemmel prompt yazdın!",
                IconUrl = "/badges/perfect-prompts.svg",
                Type = BadgeType.PerfectPrompts,
                RequiredValue = 10,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("b6c1d2e3-6789-0123-4523-4567890abcde"),
                Name = "Tabu Kaçkını",
                Description = "50 oyunda hiç tabu kelime kullanmadın!",
                IconUrl = "/badges/tabu-avoidance.svg",
                Type = BadgeType.TabuAvoidance,
                RequiredValue = 50,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<Badge>().HasData(badges);
    }
}