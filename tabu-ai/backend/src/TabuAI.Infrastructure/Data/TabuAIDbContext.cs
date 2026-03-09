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
    public DbSet<VersusGame> VersusGames { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<WordPack> WordPacks { get; set; }
    public DbSet<DailyChallenge> DailyChallenges { get; set; }
    public DbSet<DailyChallengeEntry> DailyChallengeEntries { get; set; }

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
            entity.Property(e => e.Level).IsRequired().HasDefaultValue(PlayerLevel.Rookie).HasSentinel((PlayerLevel)0);
            entity.Property(e => e.Role).IsRequired().HasDefaultValue(UserRole.User).HasSentinel((UserRole)0);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.GoogleId).IsUnique();
            entity.HasIndex(e => e.FacebookId).IsUnique();
        });

        // Word Configuration
        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TargetWord).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            
            entity.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("tr");
            entity.Property(e => e.WordPackId).IsRequired(false);
            entity.HasOne(e => e.WordPack)
                .WithMany(e => e.Words)
                .HasForeignKey(e => e.WordPackId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

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

        // VersusGame Configuration
        modelBuilder.Entity<VersusGame>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoomCode).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.RoomCode).IsUnique();

            entity.HasOne(e => e.Word)
                .WithMany()
                .HasForeignKey(e => e.WordId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Player1)
                .WithMany()
                .HasForeignKey(e => e.Player1Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Player2)
                .WithMany()
                .HasForeignKey(e => e.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Player1GameSession)
                .WithMany()
                .HasForeignKey(e => e.Player1GameSessionId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Player2GameSession)
                .WithMany()
                .HasForeignKey(e => e.Player2GameSessionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Challenge Configuration
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).HasMaxLength(200);

            entity.HasOne(e => e.Challenger)
                .WithMany()
                .HasForeignKey(e => e.ChallengerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Challenged)
                .WithMany()
                .HasForeignKey(e => e.ChallengedId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Word)
                .WithMany()
                .HasForeignKey(e => e.WordId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.VersusGame)
                .WithMany()
                .HasForeignKey(e => e.VersusGameId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ActivityLog Configuration
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.User)
                .WithMany(e => e.ActivityLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.CreatedAt });
        });

        // WordPack Configuration
        modelBuilder.Entity<WordPack>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("tr");
            entity.HasIndex(e => e.Language);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DailyChallenge Configuration
        modelBuilder.Entity<DailyChallenge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Language).HasMaxLength(10).HasDefaultValue("tr");
            entity.HasIndex(e => new { e.ChallengeDate, e.Language }).IsUnique();

            entity.HasOne(e => e.Word)
                .WithMany()
                .HasForeignKey(e => e.WordId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DailyChallengeEntry Configuration
        modelBuilder.Entity<DailyChallengeEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DailyChallengeId, e.UserId }).IsUnique();

            entity.HasOne(e => e.DailyChallenge)
                .WithMany()
                .HasForeignKey(e => e.DailyChallengeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.GameSession)
                .WithMany()
                .HasForeignKey(e => e.GameSessionId)
                .OnDelete(DeleteBehavior.Restrict);
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
            },
            // Yemek - Ek
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000001"),
                TargetWord = "Kebap",
                TabuWords = new List<string> { "et", "şiş", "ızgara", "Adana", "mangal" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000002"),
                TargetWord = "Sushi",
                TabuWords = new List<string> { "Japon", "pirinç", "balık", "soya sosu", "çiğ" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000003"),
                TargetWord = "Dondurma",
                TabuWords = new List<string> { "soğuk", "tatlı", "külah", "süt", "vanilya" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000004"),
                TargetWord = "Çay",
                TabuWords = new List<string> { "demlik", "bardak", "sıcak", "şeker", "çaydanlık" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000005"),
                TargetWord = "Kahve",
                TabuWords = new List<string> { "fincan", "kafein", "Türk", "espresso", "çekirdek" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000006"),
                TargetWord = "Lahmacun",
                TabuWords = new List<string> { "ince", "hamur", "kıyma", "limon", "rulo" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c1000001-0001-0001-0001-000000000007"),
                TargetWord = "Mantı",
                TabuWords = new List<string> { "hamur", "kıyma", "yoğurt", "Kayseri", "küçük" },
                Category = "Yemek",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Teknoloji - Ek
            new()
            {
                Id = Guid.Parse("c2000001-0001-0001-0001-000000000001"),
                TargetWord = "Robot",
                TabuWords = new List<string> { "makine", "otomat", "metal", "programlama", "android" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c2000001-0001-0001-0001-000000000002"),
                TargetWord = "Drone",
                TabuWords = new List<string> { "uçmak", "kumanda", "kamera", "pervane", "insansız" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c2000001-0001-0001-0001-000000000003"),
                TargetWord = "Bluetooth",
                TabuWords = new List<string> { "kablosuz", "bağlantı", "kulaklık", "eşleştirme", "sinyal" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c2000001-0001-0001-0001-000000000004"),
                TargetWord = "Şifre",
                TabuWords = new List<string> { "güvenlik", "giriş", "parola", "karakter", "gizli" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c2000001-0001-0001-0001-000000000005"),
                TargetWord = "Sosyal Medya",
                TabuWords = new List<string> { "paylaşım", "beğeni", "takip", "Instagram", "gönderi" },
                Category = "Teknoloji",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Spor - Ek
            new()
            {
                Id = Guid.Parse("c3000001-0001-0001-0001-000000000001"),
                TargetWord = "Basketbol",
                TabuWords = new List<string> { "pota", "top", "smaç", "NBA", "sayı" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c3000001-0001-0001-0001-000000000002"),
                TargetWord = "Tenis",
                TabuWords = new List<string> { "raket", "top", "kort", "servis", "set" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c3000001-0001-0001-0001-000000000003"),
                TargetWord = "Yüzme",
                TabuWords = new List<string> { "havuz", "su", "kulaç", "mayo", "kulvar" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c3000001-0001-0001-0001-000000000004"),
                TargetWord = "Maraton",
                TabuWords = new List<string> { "koşu", "42 km", "dayanıklılık", "yarış", "parkur" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c3000001-0001-0001-0001-000000000005"),
                TargetWord = "Voleybol",
                TabuWords = new List<string> { "file", "smaç", "pas", "top", "set" },
                Category = "Spor",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Bilim - Ek
            new()
            {
                Id = Guid.Parse("c4000001-0001-0001-0001-000000000001"),
                TargetWord = "Atom",
                TabuWords = new List<string> { "çekirdek", "elektron", "proton", "küçük", "element" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c4000001-0001-0001-0001-000000000002"),
                TargetWord = "Teleskop",
                TabuWords = new List<string> { "gözlem", "uzay", "mercek", "yıldız", "büyütmek" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c4000001-0001-0001-0001-000000000003"),
                TargetWord = "Deprem",
                TabuWords = new List<string> { "fay", "sarsıntı", "büyüklük", "Richter", "yıkım" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c4000001-0001-0001-0001-000000000004"),
                TargetWord = "Karadelik",
                TabuWords = new List<string> { "uzay", "çekim", "ışık", "yıldız", "galaksi" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c4000001-0001-0001-0001-000000000005"),
                TargetWord = "Evrim",
                TabuWords = new List<string> { "Darwin", "doğal seçilim", "tür", "adaptasyon", "mutasyon" },
                Category = "Bilim",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Doğa - Ek
            new()
            {
                Id = Guid.Parse("c5000001-0001-0001-0001-000000000001"),
                TargetWord = "Gökkuşağı",
                TabuWords = new List<string> { "renk", "yağmur", "güneş", "yedi", "gökyüzü" },
                Category = "Doğa",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c5000001-0001-0001-0001-000000000002"),
                TargetWord = "Okyanus",
                TabuWords = new List<string> { "deniz", "su", "derin", "dalga", "Pasifik" },
                Category = "Doğa",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c5000001-0001-0001-0001-000000000003"),
                TargetWord = "Orman",
                TabuWords = new List<string> { "ağaç", "yeşil", "doğa", "hayvan", "yaprak" },
                Category = "Doğa",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c5000001-0001-0001-0001-000000000004"),
                TargetWord = "Şelale",
                TabuWords = new List<string> { "su", "düşmek", "kayalık", "doğa", "akmak" },
                Category = "Doğa",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c5000001-0001-0001-0001-000000000005"),
                TargetWord = "Kutup",
                TabuWords = new List<string> { "buz", "soğuk", "penguen", "kuzey", "güney" },
                Category = "Doğa",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Tarih - Ek
            new()
            {
                Id = Guid.Parse("c6000001-0001-0001-0001-000000000001"),
                TargetWord = "Osmanlı",
                TabuWords = new List<string> { "padişah", "imparatorluk", "İstanbul", "fetih", "sultan" },
                Category = "Tarih",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c6000001-0001-0001-0001-000000000002"),
                TargetWord = "Atatürk",
                TabuWords = new List<string> { "Cumhuriyet", "kurucu", "lider", "Ankara", "devrim" },
                Category = "Tarih",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c6000001-0001-0001-0001-000000000003"),
                TargetWord = "Dinozor",
                TabuWords = new List<string> { "nesli tükenmiş", "dev", "sürüngen", "fosil", "Jura" },
                Category = "Tarih",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c6000001-0001-0001-0001-000000000004"),
                TargetWord = "Gladyatör",
                TabuWords = new List<string> { "Roma", "arena", "dövüş", "kılıç", "köle" },
                Category = "Tarih",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c6000001-0001-0001-0001-000000000005"),
                TargetWord = "Truva",
                TabuWords = new List<string> { "at", "savaş", "Yunan", "hile", "antik" },
                Category = "Tarih",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Sanat - Ek
            new()
            {
                Id = Guid.Parse("c7000001-0001-0001-0001-000000000001"),
                TargetWord = "Heykel",
                TabuWords = new List<string> { "mermer", "yontmak", "sanatçı", "müze", "üç boyutlu" },
                Category = "Sanat",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c7000001-0001-0001-0001-000000000002"),
                TargetWord = "Bale",
                TabuWords = new List<string> { "dans", "parmak ucu", "sahne", "kuğu", "tutu" },
                Category = "Sanat",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c7000001-0001-0001-0001-000000000003"),
                TargetWord = "Ebru",
                TabuWords = new List<string> { "su", "boya", "desen", "geleneksel", "kağıt" },
                Category = "Sanat",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Müzik - Ek
            new()
            {
                Id = Guid.Parse("c8000001-0001-0001-0001-000000000001"),
                TargetWord = "Piyano",
                TabuWords = new List<string> { "tuş", "siyah beyaz", "çalmak", "kuyruklu", "nota" },
                Category = "Müzik",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c8000001-0001-0001-0001-000000000002"),
                TargetWord = "Davul",
                TabuWords = new List<string> { "vurmak", "ritim", "baget", "ses", "tempo" },
                Category = "Müzik",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c8000001-0001-0001-0001-000000000003"),
                TargetWord = "Rap",
                TabuWords = new List<string> { "söz", "ritim", "hip-hop", "beat", "kafiye" },
                Category = "Müzik",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Ulaşım - Ek
            new()
            {
                Id = Guid.Parse("c9000001-0001-0001-0001-000000000001"),
                TargetWord = "Metro",
                TabuWords = new List<string> { "yeraltı", "istasyon", "ray", "şehir", "ulaşım" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c9000001-0001-0001-0001-000000000002"),
                TargetWord = "Gemi",
                TabuWords = new List<string> { "deniz", "liman", "kaptan", "yolcu", "güverte" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("c9000001-0001-0001-0001-000000000003"),
                TargetWord = "Roket",
                TabuWords = new List<string> { "uzay", "fırlatmak", "NASA", "yakıt", "hız" },
                Category = "Ulaşım",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Coğrafya (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d1000001-0001-0001-0001-000000000001"),
                TargetWord = "İstanbul",
                TabuWords = new List<string> { "boğaz", "köprü", "iki kıta", "cami", "büyükşehir" },
                Category = "Coğrafya",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d1000001-0001-0001-0001-000000000002"),
                TargetWord = "Kapadokya",
                TabuWords = new List<string> { "peri bacası", "balon", "Nevşehir", "yeraltı", "kayalık" },
                Category = "Coğrafya",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d1000001-0001-0001-0001-000000000003"),
                TargetWord = "Antarktika",
                TabuWords = new List<string> { "buz", "kutup", "soğuk", "penguen", "kıta" },
                Category = "Coğrafya",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d1000001-0001-0001-0001-000000000004"),
                TargetWord = "Pamukkale",
                TabuWords = new List<string> { "travertenler", "beyaz", "termal", "Denizli", "UNESCO" },
                Category = "Coğrafya",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Meslekler (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d2000001-0001-0001-0001-000000000001"),
                TargetWord = "Doktor",
                TabuWords = new List<string> { "hastane", "tedavi", "ilaç", "muayene", "stetoskop" },
                Category = "Meslekler",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d2000001-0001-0001-0001-000000000002"),
                TargetWord = "Astronot",
                TabuWords = new List<string> { "uzay", "roket", "NASA", "ağırlıksız", "kask" },
                Category = "Meslekler",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d2000001-0001-0001-0001-000000000003"),
                TargetWord = "Mimar",
                TabuWords = new List<string> { "bina", "çizim", "proje", "tasarım", "yapı" },
                Category = "Meslekler",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d2000001-0001-0001-0001-000000000004"),
                TargetWord = "Aşçı",
                TabuWords = new List<string> { "mutfak", "yemek", "pişirmek", "restoran", "tarif" },
                Category = "Meslekler",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d2000001-0001-0001-0001-000000000005"),
                TargetWord = "Dedektif",
                TabuWords = new List<string> { "ipucu", "soruşturma", "cinayet", "büyüteç", "suç" },
                Category = "Meslekler",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Hayvanlar (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d3000001-0001-0001-0001-000000000001"),
                TargetWord = "Yunus",
                TabuWords = new List<string> { "deniz", "zeki", "atlama", "memeli", "gri" },
                Category = "Hayvanlar",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d3000001-0001-0001-0001-000000000002"),
                TargetWord = "Kartal",
                TabuWords = new List<string> { "kuş", "yırtıcı", "kanat", "gökyüzü", "pençe" },
                Category = "Hayvanlar",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d3000001-0001-0001-0001-000000000003"),
                TargetWord = "Bukalemun",
                TabuWords = new List<string> { "renk değiştirmek", "sürüngen", "dil", "kamufle", "göz" },
                Category = "Hayvanlar",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d3000001-0001-0001-0001-000000000004"),
                TargetWord = "Fil",
                TabuWords = new List<string> { "hortum", "büyük", "fildişi", "Afrika", "hafıza" },
                Category = "Hayvanlar",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d3000001-0001-0001-0001-000000000005"),
                TargetWord = "Ahtapot",
                TabuWords = new List<string> { "sekiz", "kol", "deniz", "mürekkep", "yüzmek" },
                Category = "Hayvanlar",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Eğlence (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d4000001-0001-0001-0001-000000000001"),
                TargetWord = "Sirk",
                TabuWords = new List<string> { "palyaço", "akrobat", "gösteri", "çadır", "hayvan" },
                Category = "Eğlence",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d4000001-0001-0001-0001-000000000002"),
                TargetWord = "Karaoke",
                TabuWords = new List<string> { "şarkı", "mikrofon", "ekran", "söz", "eğlence" },
                Category = "Eğlence",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d4000001-0001-0001-0001-000000000003"),
                TargetWord = "Lunapark",
                TabuWords = new List<string> { "dönme dolap", "hız treni", "eğlence", "bilet", "atlıkarınca" },
                Category = "Eğlence",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Kavramlar (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d5000001-0001-0001-0001-000000000001"),
                TargetWord = "Nostalji",
                TabuWords = new List<string> { "geçmiş", "özlem", "anı", "eski", "hatıra" },
                Category = "Kavramlar",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d5000001-0001-0001-0001-000000000002"),
                TargetWord = "Demokrasi",
                TabuWords = new List<string> { "oy", "halk", "seçim", "özgürlük", "yönetim" },
                Category = "Kavramlar",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d5000001-0001-0001-0001-000000000003"),
                TargetWord = "Empati",
                TabuWords = new List<string> { "anlama", "duygu", "karşı taraf", "hissetmek", "başkası" },
                Category = "Kavramlar",
                Difficulty = DifficultyLevel.Hard,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d5000001-0001-0001-0001-000000000004"),
                TargetWord = "Ironi",
                TabuWords = new List<string> { "tersine", "alay", "anlam", "söz", "beklenti" },
                Category = "Kavramlar",
                Difficulty = DifficultyLevel.Expert,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Sinema/TV (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d6000001-0001-0001-0001-000000000001"),
                TargetWord = "Oscar",
                TabuWords = new List<string> { "ödül", "Hollywood", "film", "heykelcik", "tören" },
                Category = "Sinema",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d6000001-0001-0001-0001-000000000002"),
                TargetWord = "Dizi",
                TabuWords = new List<string> { "bölüm", "sezon", "televizyon", "izlemek", "oyuncu" },
                Category = "Sinema",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d6000001-0001-0001-0001-000000000003"),
                TargetWord = "Animasyon",
                TabuWords = new List<string> { "çizgi film", "karakter", "Pixar", "canlandırma", "Disney" },
                Category = "Sinema",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // Edebiyat (Yeni Kategori)
            new()
            {
                Id = Guid.Parse("d7000001-0001-0001-0001-000000000001"),
                TargetWord = "Roman",
                TabuWords = new List<string> { "kitap", "yazar", "sayfa", "okumak", "hikaye" },
                Category = "Edebiyat",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d7000001-0001-0001-0001-000000000002"),
                TargetWord = "Şiir",
                TabuWords = new List<string> { "mısra", "kafiye", "dize", "şair", "ölçü" },
                Category = "Edebiyat",
                Difficulty = DifficultyLevel.Medium,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.Parse("d7000001-0001-0001-0001-000000000003"),
                TargetWord = "Masal",
                TabuWords = new List<string> { "çocuk", "prenses", "bir varmış", "hayal", "ejderha" },
                Category = "Edebiyat",
                Difficulty = DifficultyLevel.Easy,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },

            // ========== ENGLISH WORDS ==========
            // Transportation
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000001"), TargetWord = "Airplane", TabuWords = new List<string> { "fly", "wing", "pilot", "sky", "airport" }, Category = "Transportation", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000002"), TargetWord = "Submarine", TabuWords = new List<string> { "underwater", "navy", "torpedo", "deep", "periscope" }, Category = "Transportation", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000003"), TargetWord = "Helicopter", TabuWords = new List<string> { "rotor", "blade", "hover", "fly", "rescue" }, Category = "Transportation", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Technology
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000004"), TargetWord = "Computer", TabuWords = new List<string> { "screen", "keyboard", "mouse", "internet", "processor" }, Category = "Technology", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000005"), TargetWord = "Smartphone", TabuWords = new List<string> { "call", "app", "screen", "touch", "mobile" }, Category = "Technology", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000006"), TargetWord = "Artificial Intelligence", TabuWords = new List<string> { "AI", "machine", "learning", "robot", "algorithm" }, Category = "Technology", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000007"), TargetWord = "Blockchain", TabuWords = new List<string> { "crypto", "bitcoin", "ledger", "decentralized", "mining" }, Category = "Technology", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Science
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000008"), TargetWord = "Gravity", TabuWords = new List<string> { "fall", "Newton", "force", "weight", "earth" }, Category = "Science", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000009"), TargetWord = "Photosynthesis", TabuWords = new List<string> { "plant", "sun", "chlorophyll", "oxygen", "leaf" }, Category = "Science", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000010"), TargetWord = "Volcano", TabuWords = new List<string> { "lava", "eruption", "mountain", "magma", "ash" }, Category = "Science", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000011"), TargetWord = "Black Hole", TabuWords = new List<string> { "space", "gravity", "light", "star", "singularity" }, Category = "Science", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Food
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000012"), TargetWord = "Pizza", TabuWords = new List<string> { "cheese", "dough", "slice", "Italian", "oven" }, Category = "Food", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000013"), TargetWord = "Sushi", TabuWords = new List<string> { "Japanese", "rice", "fish", "raw", "seaweed" }, Category = "Food", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000014"), TargetWord = "Chocolate", TabuWords = new List<string> { "sweet", "cocoa", "candy", "brown", "milk" }, Category = "Food", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000015"), TargetWord = "Croissant", TabuWords = new List<string> { "French", "pastry", "butter", "breakfast", "flaky" }, Category = "Food", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Sports
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000016"), TargetWord = "Basketball", TabuWords = new List<string> { "hoop", "ball", "dunk", "NBA", "court" }, Category = "Sports", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000017"), TargetWord = "Marathon", TabuWords = new List<string> { "running", "42km", "endurance", "race", "finish line" }, Category = "Sports", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000018"), TargetWord = "Olympics", TabuWords = new List<string> { "rings", "medal", "athlete", "torch", "games" }, Category = "Sports", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // History
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000019"), TargetWord = "Pyramid", TabuWords = new List<string> { "Egypt", "pharaoh", "stone", "ancient", "tomb" }, Category = "History", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000020"), TargetWord = "Renaissance", TabuWords = new List<string> { "art", "rebirth", "Italy", "Leonardo", "culture" }, Category = "History", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000021"), TargetWord = "Gladiator", TabuWords = new List<string> { "Rome", "arena", "fight", "sword", "Colosseum" }, Category = "History", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Nature
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000022"), TargetWord = "Rainbow", TabuWords = new List<string> { "color", "rain", "sun", "seven", "arc" }, Category = "Nature", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000023"), TargetWord = "Earthquake", TabuWords = new List<string> { "fault", "shake", "Richter", "tremor", "destruction" }, Category = "Nature", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000024"), TargetWord = "Aurora", TabuWords = new List<string> { "northern lights", "sky", "polar", "magnetic", "green" }, Category = "Nature", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Animals
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000025"), TargetWord = "Dolphin", TabuWords = new List<string> { "ocean", "smart", "jump", "mammal", "fin" }, Category = "Animals", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000026"), TargetWord = "Chameleon", TabuWords = new List<string> { "color change", "reptile", "tongue", "camouflage", "eyes" }, Category = "Animals", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000027"), TargetWord = "Penguin", TabuWords = new List<string> { "ice", "bird", "Antarctica", "waddle", "tuxedo" }, Category = "Animals", Difficulty = DifficultyLevel.Easy, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Concepts
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000028"), TargetWord = "Democracy", TabuWords = new List<string> { "vote", "people", "election", "freedom", "government" }, Category = "Concepts", Difficulty = DifficultyLevel.Medium, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000029"), TargetWord = "Nostalgia", TabuWords = new List<string> { "past", "memory", "longing", "old", "remember" }, Category = "Concepts", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e1000001-0001-0001-0001-000000000030"), TargetWord = "Karma", TabuWords = new List<string> { "fate", "action", "consequence", "balance", "destiny" }, Category = "Concepts", Difficulty = DifficultyLevel.Hard, Language = "en", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // ========== GERMAN WORDS (Deutsch) ==========
            // Verkehr (Transportation)
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000001"), TargetWord = "Flugzeug", TabuWords = new List<string> { "fliegen", "Flügel", "Pilot", "Himmel", "Flughafen" }, Category = "Verkehr", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000002"), TargetWord = "U-Boot", TabuWords = new List<string> { "Unterwasser", "Marine", "Torpedo", "tauchen", "Periskop" }, Category = "Verkehr", Difficulty = DifficultyLevel.Medium, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000003"), TargetWord = "Fahrrad", TabuWords = new List<string> { "Pedal", "Rad", "Kette", "Lenker", "fahren" }, Category = "Verkehr", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Technologie
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000004"), TargetWord = "Computer", TabuWords = new List<string> { "Bildschirm", "Tastatur", "Maus", "Internet", "Prozessor" }, Category = "Technologie", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000005"), TargetWord = "Künstliche Intelligenz", TabuWords = new List<string> { "KI", "Maschine", "Lernen", "Roboter", "Algorithmus" }, Category = "Technologie", Difficulty = DifficultyLevel.Hard, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000006"), TargetWord = "Passwort", TabuWords = new List<string> { "Sicherheit", "geheim", "Zugang", "Zeichen", "Login" }, Category = "Technologie", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Wissenschaft
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000007"), TargetWord = "Schwerkraft", TabuWords = new List<string> { "fallen", "Newton", "Kraft", "Gewicht", "Erde" }, Category = "Wissenschaft", Difficulty = DifficultyLevel.Medium, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000008"), TargetWord = "Erdbeben", TabuWords = new List<string> { "Erschütterung", "Richter", "Platte", "Zerstörung", "Beben" }, Category = "Wissenschaft", Difficulty = DifficultyLevel.Medium, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000009"), TargetWord = "Schwarzes Loch", TabuWords = new List<string> { "Weltraum", "Gravitation", "Licht", "Stern", "Singularität" }, Category = "Wissenschaft", Difficulty = DifficultyLevel.Hard, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Essen
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000010"), TargetWord = "Brezel", TabuWords = new List<string> { "Teig", "Salz", "Bayern", "Bäckerei", "Knoten" }, Category = "Essen", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000011"), TargetWord = "Sauerkraut", TabuWords = new List<string> { "Kohl", "sauer", "fermentiert", "Beilage", "deutsch" }, Category = "Essen", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000012"), TargetWord = "Schwarzwälder Kirschtorte", TabuWords = new List<string> { "Schokolade", "Kirsche", "Sahne", "Kuchen", "Torte" }, Category = "Essen", Difficulty = DifficultyLevel.Hard, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Sport
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000013"), TargetWord = "Fußball", TabuWords = new List<string> { "Ball", "Tor", "Mannschaft", "Stadion", "Bundesliga" }, Category = "Sport", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000014"), TargetWord = "Skifahren", TabuWords = new List<string> { "Schnee", "Berg", "Piste", "Winter", "Lift" }, Category = "Sport", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Geschichte
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000015"), TargetWord = "Berliner Mauer", TabuWords = new List<string> { "Teilung", "Ost", "West", "Grenze", "1989" }, Category = "Geschichte", Difficulty = DifficultyLevel.Medium, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000016"), TargetWord = "Ritter", TabuWords = new List<string> { "Rüstung", "Schwert", "Burg", "Mittelalter", "Pferd" }, Category = "Geschichte", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Natur
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000017"), TargetWord = "Regenbogen", TabuWords = new List<string> { "Farbe", "Regen", "Sonne", "sieben", "Bogen" }, Category = "Natur", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000018"), TargetWord = "Wasserfall", TabuWords = new List<string> { "Wasser", "fallen", "Felsen", "Natur", "fließen" }, Category = "Natur", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000019"), TargetWord = "Nordlicht", TabuWords = new List<string> { "Aurora", "Himmel", "Polar", "Magnetfeld", "grün" }, Category = "Natur", Difficulty = DifficultyLevel.Hard, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Tiere
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000020"), TargetWord = "Schmetterling", TabuWords = new List<string> { "Flügel", "Raupe", "bunt", "fliegen", "Nektar" }, Category = "Tiere", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000021"), TargetWord = "Eichhörnchen", TabuWords = new List<string> { "Nuss", "Baum", "klettern", "Schwanz", "Eiche" }, Category = "Tiere", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Kultur
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000022"), TargetWord = "Oktoberfest", TabuWords = new List<string> { "München", "Bier", "Fest", "Tracht", "Zelt" }, Category = "Kultur", Difficulty = DifficultyLevel.Easy, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000023"), TargetWord = "Kuckucksuhr", TabuWords = new List<string> { "Schwarzwald", "Uhr", "Vogel", "Holz", "Zeit" }, Category = "Kultur", Difficulty = DifficultyLevel.Medium, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Konzepte
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000024"), TargetWord = "Fernweh", TabuWords = new List<string> { "Reise", "Sehnsucht", "fern", "Heimweh", "Welt" }, Category = "Konzepte", Difficulty = DifficultyLevel.Hard, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e2000001-0001-0001-0001-000000000025"), TargetWord = "Zeitgeist", TabuWords = new List<string> { "Epoche", "Geist", "Gesellschaft", "Trend", "Stimmung" }, Category = "Konzepte", Difficulty = DifficultyLevel.Expert, Language = "de", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // ========== FRENCH WORDS (Français) ==========
            // Transport
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000001"), TargetWord = "Avion", TabuWords = new List<string> { "voler", "aile", "pilote", "ciel", "aéroport" }, Category = "Transport", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000002"), TargetWord = "Métro", TabuWords = new List<string> { "souterrain", "station", "ticket", "Paris", "ligne" }, Category = "Transport", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000003"), TargetWord = "Vélo", TabuWords = new List<string> { "pédale", "roue", "chaîne", "guidon", "rouler" }, Category = "Transport", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Technologie
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000004"), TargetWord = "Ordinateur", TabuWords = new List<string> { "écran", "clavier", "souris", "internet", "processeur" }, Category = "Technologie", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000005"), TargetWord = "Intelligence Artificielle", TabuWords = new List<string> { "IA", "machine", "apprentissage", "robot", "algorithme" }, Category = "Technologie", Difficulty = DifficultyLevel.Hard, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000006"), TargetWord = "Mot de passe", TabuWords = new List<string> { "sécurité", "secret", "accès", "caractère", "connexion" }, Category = "Technologie", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Science
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000007"), TargetWord = "Gravité", TabuWords = new List<string> { "tomber", "Newton", "force", "poids", "terre" }, Category = "Science", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000008"), TargetWord = "Trou noir", TabuWords = new List<string> { "espace", "gravité", "lumière", "étoile", "galaxie" }, Category = "Science", Difficulty = DifficultyLevel.Hard, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000009"), TargetWord = "Volcan", TabuWords = new List<string> { "lave", "éruption", "montagne", "magma", "cendre" }, Category = "Science", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Gastronomie
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000010"), TargetWord = "Croissant", TabuWords = new List<string> { "beurre", "pâte", "petit-déjeuner", "boulangerie", "feuilleté" }, Category = "Gastronomie", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000011"), TargetWord = "Crêpe", TabuWords = new List<string> { "pâte", "fine", "Bretagne", "sucre", "poêle" }, Category = "Gastronomie", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000012"), TargetWord = "Ratatouille", TabuWords = new List<string> { "légume", "Provence", "film", "aubergine", "courgette" }, Category = "Gastronomie", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000013"), TargetWord = "Baguette", TabuWords = new List<string> { "pain", "long", "croûte", "boulanger", "farine" }, Category = "Gastronomie", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000014"), TargetWord = "Fromage", TabuWords = new List<string> { "lait", "Camembert", "Brie", "vache", "affiner" }, Category = "Gastronomie", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Sport
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000015"), TargetWord = "Football", TabuWords = new List<string> { "ballon", "but", "équipe", "stade", "match" }, Category = "Sport", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000016"), TargetWord = "Tour de France", TabuWords = new List<string> { "vélo", "cyclisme", "étape", "maillot jaune", "course" }, Category = "Sport", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000017"), TargetWord = "Escrime", TabuWords = new List<string> { "épée", "touche", "masque", "piste", "duel" }, Category = "Sport", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Histoire
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000018"), TargetWord = "Révolution", TabuWords = new List<string> { "1789", "Bastille", "liberté", "peuple", "guillotine" }, Category = "Histoire", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000019"), TargetWord = "Napoléon", TabuWords = new List<string> { "empereur", "guerre", "Waterloo", "Corse", "conquête" }, Category = "Histoire", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000020"), TargetWord = "Versailles", TabuWords = new List<string> { "château", "roi", "jardin", "Louis", "palace" }, Category = "Histoire", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Nature
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000021"), TargetWord = "Arc-en-ciel", TabuWords = new List<string> { "couleur", "pluie", "soleil", "sept", "ciel" }, Category = "Nature", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000022"), TargetWord = "Cascade", TabuWords = new List<string> { "eau", "tomber", "rocher", "rivière", "nature" }, Category = "Nature", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Animaux
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000023"), TargetWord = "Papillon", TabuWords = new List<string> { "aile", "chenille", "coloré", "voler", "nectar" }, Category = "Animaux", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000024"), TargetWord = "Dauphin", TabuWords = new List<string> { "mer", "intelligent", "sauter", "mammifère", "nager" }, Category = "Animaux", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000025"), TargetWord = "Caméléon", TabuWords = new List<string> { "couleur", "reptile", "langue", "camouflage", "yeux" }, Category = "Animaux", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Culture
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000026"), TargetWord = "Tour Eiffel", TabuWords = new List<string> { "Paris", "fer", "tour", "monument", "Gustave" }, Category = "Culture", Difficulty = DifficultyLevel.Easy, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000027"), TargetWord = "Impressionnisme", TabuWords = new List<string> { "Monet", "peinture", "lumière", "art", "mouvement" }, Category = "Culture", Difficulty = DifficultyLevel.Hard, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Concepts
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000028"), TargetWord = "Démocratie", TabuWords = new List<string> { "vote", "peuple", "élection", "liberté", "gouvernement" }, Category = "Concepts", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000029"), TargetWord = "Joie de vivre", TabuWords = new List<string> { "bonheur", "vie", "plaisir", "enthousiasme", "vivre" }, Category = "Concepts", Difficulty = DifficultyLevel.Hard, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new() { Id = Guid.Parse("e3000001-0001-0001-0001-000000000030"), TargetWord = "Liberté", TabuWords = new List<string> { "libre", "droit", "prison", "indépendance", "révolution" }, Category = "Concepts", Difficulty = DifficultyLevel.Medium, Language = "fr", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
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