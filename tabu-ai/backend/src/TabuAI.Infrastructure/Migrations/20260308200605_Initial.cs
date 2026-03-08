using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RequiredValue = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Role = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    GamesWon = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    GoogleId = table.Column<string>(type: "text", nullable: true),
                    FacebookId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetWord = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TabuWords = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ScoreEarned = table.Column<int>(type: "integer", nullable: true),
                    RelatedEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddresseeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_AddresseeId",
                        column: x => x.AddresseeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserBadges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BadgeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBadges_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBadges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetricName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    UserPrompt = table.Column<string>(type: "text", nullable: false),
                    AiResponse = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsCorrectGuess = table.Column<bool>(type: "boolean", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    TimeSpent = table.Column<TimeSpan>(type: "interval", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSessions_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptNumber = table.Column<int>(type: "integer", nullable: false),
                    UserPrompt = table.Column<string>(type: "text", nullable: false),
                    AiGuess = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    AiFeedback = table.Column<string>(type: "text", nullable: true),
                    PromptQuality = table.Column<int>(type: "integer", nullable: true),
                    Suggestions = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameAttempts_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VersusGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Player1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Player2Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Player1GameSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Player2GameSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Player1Score = table.Column<int>(type: "integer", nullable: false),
                    Player2Score = table.Column<int>(type: "integer", nullable: false),
                    Player1Attempts = table.Column<int>(type: "integer", nullable: false),
                    Player2Attempts = table.Column<int>(type: "integer", nullable: false),
                    RoomCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersusGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VersusGames_GameSessions_Player1GameSessionId",
                        column: x => x.Player1GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VersusGames_GameSessions_Player2GameSessionId",
                        column: x => x.Player2GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VersusGames_Users_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersusGames_Users_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersusGames_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengedId = table.Column<Guid>(type: "uuid", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersusGameId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_ChallengedId",
                        column: x => x.ChallengedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_ChallengerId",
                        column: x => x.ChallengerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_VersusGames_VersusGameId",
                        column: x => x.VersusGameId,
                        principalTable: "VersusGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Challenges_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "IsActive", "Name", "RequiredValue", "Type" },
                values: new object[,]
                {
                    { new Guid("a5b0c1d2-5678-9012-3412-34567890abcd"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "10 mükemmel prompt yazdın!", "/badges/perfect-prompts.svg", true, "Prompt Ustası", 10, 3 },
                    { new Guid("b6c1d2e3-6789-0123-4523-4567890abcde"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "50 oyunda hiç tabu kelime kullanmadın!", "/badges/tabu-avoidance.svg", true, "Tabu Kaçkını", 50, 5 },
                    { new Guid("f4a9b0c1-4567-8901-2301-234567890abc"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "İlk oyununu tamamladın!", "/badges/first-game.svg", true, "İlk Adım", 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "FacebookId", "GamesPlayed", "GamesWon", "GoogleId", "IsActive", "LastLoginAt", "Level", "PasswordHash", "Role", "TotalScore", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin User", "admin@tabuai.com", null, 0, 0, null, true, null, 1, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgwd966VQRfAk5U5Z6.t.15v8vS6", 2, 0, "admin" });

            migrationBuilder.InsertData(
                table: "Words",
                columns: new[] { "Id", "Category", "CreatedAt", "Difficulty", "IsActive", "TabuWords", "TargetWord", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"ray\",\"vagon\",\"istasyon\",\"lokomotif\",\"bilet\"]", "Tren", null },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"pedal\",\"tekerlek\",\"zincir\",\"gidon\",\"s\\u00FCrmek\"]", "Bisiklet", null },
                    { new Guid("a3333333-3333-3333-3333-333333333333"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"aramak\",\"konu\\u015Fmak\",\"numara\",\"mobil\",\"ekran\"]", "Telefon", null },
                    { new Guid("a4444444-4444-4444-4444-444444444444"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"web\",\"a\\u011F\",\"ba\\u011Flant\\u0131\",\"site\",\"\\u00E7evrimi\\u00E7i\"]", "İnternet", null },
                    { new Guid("a5555555-5555-5555-5555-555555555555"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"\\u0131\\u015F\\u0131k\",\"s\\u0131cak\",\"y\\u0131ld\\u0131z\",\"g\\u00FCnd\\u00FCz\",\"enerji\"]", "Güneş", null },
                    { new Guid("a6666666-6666-6666-6666-666666666666"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"gen\",\"h\\u00FCcre\",\"kal\\u0131t\\u0131m\",\"\\u00E7ift sarmal\",\"biyoloji\"]", "DNA", null },
                    { new Guid("a7777777-7777-7777-7777-777777777777"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"boya\",\"tablo\",\"f\\u0131r\\u00E7a\",\"tuval\",\"\\u00E7izmek\"]", "Resim", null },
                    { new Guid("a8888888-8888-8888-8888-888888888888"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "[\"Dal\\u00ED\",\"r\\u00FCya\",\"bilin\\u00E7alt\\u0131\",\"ger\\u00E7ek\\u00FCst\\u00FC\",\"sanat ak\\u0131m\\u0131\"]", "Sürrealizm", null },
                    { new Guid("a9999999-9999-9999-9999-999999999999"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"hamur\",\"peynir\",\"dilim\",\"\\u0130talyan\",\"f\\u0131r\\u0131n\"]", "Pizza", null },
                    { new Guid("b1111111-1111-1111-1111-111111111111"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"tatl\\u0131\",\"yufka\",\"\\u015Ferbet\",\"f\\u0131st\\u0131k\",\"Gaziantep\"]", "Baklava", null },
                    { new Guid("b2222222-2222-2222-2222-222222222222"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"top\",\"gol\",\"kaleci\",\"stadyum\",\"ma\\u00E7\"]", "Futbol", null },
                    { new Guid("b3333333-3333-3333-3333-333333333333"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"halka\",\"madalya\",\"spor\",\"me\\u015Fale\",\"yar\\u0131\\u015Fma\"]", "Olimpiyat", null },
                    { new Guid("b4444444-4444-4444-4444-444444444444"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"M\\u0131s\\u0131r\",\"firavun\",\"ta\\u015F\",\"\\u00FC\\u00E7gen\",\"antik\"]", "Piramit", null },
                    { new Guid("b5555555-5555-5555-5555-555555555555"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"lav\",\"patlama\",\"da\\u011F\",\"magma\",\"k\\u00FCl\"]", "Yanardağ", null },
                    { new Guid("b6666666-6666-6666-6666-666666666666"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"tel\",\"akort\",\"\\u00E7almak\",\"m\\u00FCzik\",\"pena\"]", "Gitar", null },
                    { new Guid("b7777777-7777-7777-7777-777777777777"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"\\u015Fark\\u0131\",\"sahne\",\"soprano\",\"orkestra\",\"tiyatro\"]", "Opera", null },
                    { new Guid("c1000001-0001-0001-0001-000000000001"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"et\",\"\\u015Fi\\u015F\",\"\\u0131zgara\",\"Adana\",\"mangal\"]", "Kebap", null },
                    { new Guid("c1000001-0001-0001-0001-000000000002"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"Japon\",\"pirin\\u00E7\",\"bal\\u0131k\",\"soya sosu\",\"\\u00E7i\\u011F\"]", "Sushi", null },
                    { new Guid("c1000001-0001-0001-0001-000000000003"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"so\\u011Fuk\",\"tatl\\u0131\",\"k\\u00FClah\",\"s\\u00FCt\",\"vanilya\"]", "Dondurma", null },
                    { new Guid("c1000001-0001-0001-0001-000000000004"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"demlik\",\"bardak\",\"s\\u0131cak\",\"\\u015Feker\",\"\\u00E7aydanl\\u0131k\"]", "Çay", null },
                    { new Guid("c1000001-0001-0001-0001-000000000005"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"fincan\",\"kafein\",\"T\\u00FCrk\",\"espresso\",\"\\u00E7ekirdek\"]", "Kahve", null },
                    { new Guid("c1000001-0001-0001-0001-000000000006"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"ince\",\"hamur\",\"k\\u0131yma\",\"limon\",\"rulo\"]", "Lahmacun", null },
                    { new Guid("c1000001-0001-0001-0001-000000000007"), "Yemek", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"hamur\",\"k\\u0131yma\",\"yo\\u011Furt\",\"Kayseri\",\"k\\u00FC\\u00E7\\u00FCk\"]", "Mantı", null },
                    { new Guid("c2000001-0001-0001-0001-000000000001"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"makine\",\"otomat\",\"metal\",\"programlama\",\"android\"]", "Robot", null },
                    { new Guid("c2000001-0001-0001-0001-000000000002"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"u\\u00E7mak\",\"kumanda\",\"kamera\",\"pervane\",\"insans\\u0131z\"]", "Drone", null },
                    { new Guid("c2000001-0001-0001-0001-000000000003"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"kablosuz\",\"ba\\u011Flant\\u0131\",\"kulakl\\u0131k\",\"e\\u015Fle\\u015Ftirme\",\"sinyal\"]", "Bluetooth", null },
                    { new Guid("c2000001-0001-0001-0001-000000000004"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"g\\u00FCvenlik\",\"giri\\u015F\",\"parola\",\"karakter\",\"gizli\"]", "Şifre", null },
                    { new Guid("c2000001-0001-0001-0001-000000000005"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"payla\\u015F\\u0131m\",\"be\\u011Feni\",\"takip\",\"Instagram\",\"g\\u00F6nderi\"]", "Sosyal Medya", null },
                    { new Guid("c2a7b8c9-2345-6789-01cd-ef1234567890"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"ekran\",\"klavye\",\"fare\",\"i\\u015Flemci\",\"teknoloji\"]", "Bilgisayar", null },
                    { new Guid("c3000001-0001-0001-0001-000000000001"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"pota\",\"top\",\"sma\\u00E7\",\"NBA\",\"say\\u0131\"]", "Basketbol", null },
                    { new Guid("c3000001-0001-0001-0001-000000000002"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"raket\",\"top\",\"kort\",\"servis\",\"set\"]", "Tenis", null },
                    { new Guid("c3000001-0001-0001-0001-000000000003"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"havuz\",\"su\",\"kula\\u00E7\",\"mayo\",\"kulvar\"]", "Yüzme", null },
                    { new Guid("c3000001-0001-0001-0001-000000000004"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"ko\\u015Fu\",\"42 km\",\"dayan\\u0131kl\\u0131l\\u0131k\",\"yar\\u0131\\u015F\",\"parkur\"]", "Maraton", null },
                    { new Guid("c3000001-0001-0001-0001-000000000005"), "Spor", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"file\",\"sma\\u00E7\",\"pas\",\"top\",\"set\"]", "Voleybol", null },
                    { new Guid("c4000001-0001-0001-0001-000000000001"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"\\u00E7ekirdek\",\"elektron\",\"proton\",\"k\\u00FC\\u00E7\\u00FCk\",\"element\"]", "Atom", null },
                    { new Guid("c4000001-0001-0001-0001-000000000002"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"g\\u00F6zlem\",\"uzay\",\"mercek\",\"y\\u0131ld\\u0131z\",\"b\\u00FCy\\u00FCtmek\"]", "Teleskop", null },
                    { new Guid("c4000001-0001-0001-0001-000000000003"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"fay\",\"sars\\u0131nt\\u0131\",\"b\\u00FCy\\u00FCkl\\u00FCk\",\"Richter\",\"y\\u0131k\\u0131m\"]", "Deprem", null },
                    { new Guid("c4000001-0001-0001-0001-000000000004"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"uzay\",\"\\u00E7ekim\",\"\\u0131\\u015F\\u0131k\",\"y\\u0131ld\\u0131z\",\"galaksi\"]", "Karadelik", null },
                    { new Guid("c4000001-0001-0001-0001-000000000005"), "Bilim", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"Darwin\",\"do\\u011Fal se\\u00E7ilim\",\"t\\u00FCr\",\"adaptasyon\",\"mutasyon\"]", "Evrim", null },
                    { new Guid("c5000001-0001-0001-0001-000000000001"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"renk\",\"ya\\u011Fmur\",\"g\\u00FCne\\u015F\",\"yedi\",\"g\\u00F6ky\\u00FCz\\u00FC\"]", "Gökkuşağı", null },
                    { new Guid("c5000001-0001-0001-0001-000000000002"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"deniz\",\"su\",\"derin\",\"dalga\",\"Pasifik\"]", "Okyanus", null },
                    { new Guid("c5000001-0001-0001-0001-000000000003"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"a\\u011Fa\\u00E7\",\"ye\\u015Fil\",\"do\\u011Fa\",\"hayvan\",\"yaprak\"]", "Orman", null },
                    { new Guid("c5000001-0001-0001-0001-000000000004"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"su\",\"d\\u00FC\\u015Fmek\",\"kayal\\u0131k\",\"do\\u011Fa\",\"akmak\"]", "Şelale", null },
                    { new Guid("c5000001-0001-0001-0001-000000000005"), "Doğa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"buz\",\"so\\u011Fuk\",\"penguen\",\"kuzey\",\"g\\u00FCney\"]", "Kutup", null },
                    { new Guid("c6000001-0001-0001-0001-000000000001"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"padi\\u015Fah\",\"imparatorluk\",\"\\u0130stanbul\",\"fetih\",\"sultan\"]", "Osmanlı", null },
                    { new Guid("c6000001-0001-0001-0001-000000000002"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"Cumhuriyet\",\"kurucu\",\"lider\",\"Ankara\",\"devrim\"]", "Atatürk", null },
                    { new Guid("c6000001-0001-0001-0001-000000000003"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"nesli t\\u00FCkenmi\\u015F\",\"dev\",\"s\\u00FCr\\u00FCngen\",\"fosil\",\"Jura\"]", "Dinozor", null },
                    { new Guid("c6000001-0001-0001-0001-000000000004"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"Roma\",\"arena\",\"d\\u00F6v\\u00FC\\u015F\",\"k\\u0131l\\u0131\\u00E7\",\"k\\u00F6le\"]", "Gladyatör", null },
                    { new Guid("c6000001-0001-0001-0001-000000000005"), "Tarih", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"at\",\"sava\\u015F\",\"Yunan\",\"hile\",\"antik\"]", "Truva", null },
                    { new Guid("c7000001-0001-0001-0001-000000000001"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"mermer\",\"yontmak\",\"sanat\\u00E7\\u0131\",\"m\\u00FCze\",\"\\u00FC\\u00E7 boyutlu\"]", "Heykel", null },
                    { new Guid("c7000001-0001-0001-0001-000000000002"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"dans\",\"parmak ucu\",\"sahne\",\"ku\\u011Fu\",\"tutu\"]", "Bale", null },
                    { new Guid("c7000001-0001-0001-0001-000000000003"), "Sanat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"su\",\"boya\",\"desen\",\"geleneksel\",\"ka\\u011F\\u0131t\"]", "Ebru", null },
                    { new Guid("c8000001-0001-0001-0001-000000000001"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"tu\\u015F\",\"siyah beyaz\",\"\\u00E7almak\",\"kuyruklu\",\"nota\"]", "Piyano", null },
                    { new Guid("c8000001-0001-0001-0001-000000000002"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"vurmak\",\"ritim\",\"baget\",\"ses\",\"tempo\"]", "Davul", null },
                    { new Guid("c8000001-0001-0001-0001-000000000003"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"s\\u00F6z\",\"ritim\",\"hip-hop\",\"beat\",\"kafiye\"]", "Rap", null },
                    { new Guid("c9000001-0001-0001-0001-000000000001"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"yeralt\\u0131\",\"istasyon\",\"ray\",\"\\u015Fehir\",\"ula\\u015F\\u0131m\"]", "Metro", null },
                    { new Guid("c9000001-0001-0001-0001-000000000002"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"deniz\",\"liman\",\"kaptan\",\"yolcu\",\"g\\u00FCverte\"]", "Gemi", null },
                    { new Guid("c9000001-0001-0001-0001-000000000003"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"uzay\",\"f\\u0131rlatmak\",\"NASA\",\"yak\\u0131t\",\"h\\u0131z\"]", "Roket", null },
                    { new Guid("d1000001-0001-0001-0001-000000000001"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"bo\\u011Faz\",\"k\\u00F6pr\\u00FC\",\"iki k\\u0131ta\",\"cami\",\"b\\u00FCy\\u00FCk\\u015Fehir\"]", "İstanbul", null },
                    { new Guid("d1000001-0001-0001-0001-000000000002"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"peri bacas\\u0131\",\"balon\",\"Nev\\u015Fehir\",\"yeralt\\u0131\",\"kayal\\u0131k\"]", "Kapadokya", null },
                    { new Guid("d1000001-0001-0001-0001-000000000003"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"buz\",\"kutup\",\"so\\u011Fuk\",\"penguen\",\"k\\u0131ta\"]", "Antarktika", null },
                    { new Guid("d1000001-0001-0001-0001-000000000004"), "Coğrafya", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"travertenler\",\"beyaz\",\"termal\",\"Denizli\",\"UNESCO\"]", "Pamukkale", null },
                    { new Guid("d1b6a7b8-1234-5678-90ab-cdef12345678"), "Ulaşım", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"hava\",\"kanat\",\"yolcu\",\"pilot\",\"u\\u00E7mak\"]", "Uçak", null },
                    { new Guid("d2000001-0001-0001-0001-000000000001"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"hastane\",\"tedavi\",\"ila\\u00E7\",\"muayene\",\"stetoskop\"]", "Doktor", null },
                    { new Guid("d2000001-0001-0001-0001-000000000002"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"uzay\",\"roket\",\"NASA\",\"a\\u011F\\u0131rl\\u0131ks\\u0131z\",\"kask\"]", "Astronot", null },
                    { new Guid("d2000001-0001-0001-0001-000000000003"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"bina\",\"\\u00E7izim\",\"proje\",\"tasar\\u0131m\",\"yap\\u0131\"]", "Mimar", null },
                    { new Guid("d2000001-0001-0001-0001-000000000004"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"mutfak\",\"yemek\",\"pi\\u015Firmek\",\"restoran\",\"tarif\"]", "Aşçı", null },
                    { new Guid("d2000001-0001-0001-0001-000000000005"), "Meslekler", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"ipucu\",\"soru\\u015Fturma\",\"cinayet\",\"b\\u00FCy\\u00FCte\\u00E7\",\"su\\u00E7\"]", "Dedektif", null },
                    { new Guid("d3000001-0001-0001-0001-000000000001"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"deniz\",\"zeki\",\"atlama\",\"memeli\",\"gri\"]", "Yunus", null },
                    { new Guid("d3000001-0001-0001-0001-000000000002"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"ku\\u015F\",\"y\\u0131rt\\u0131c\\u0131\",\"kanat\",\"g\\u00F6ky\\u00FCz\\u00FC\",\"pen\\u00E7e\"]", "Kartal", null },
                    { new Guid("d3000001-0001-0001-0001-000000000003"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"renk de\\u011Fi\\u015Ftirmek\",\"s\\u00FCr\\u00FCngen\",\"dil\",\"kamufle\",\"g\\u00F6z\"]", "Bukalemun", null },
                    { new Guid("d3000001-0001-0001-0001-000000000004"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"hortum\",\"b\\u00FCy\\u00FCk\",\"fildi\\u015Fi\",\"Afrika\",\"haf\\u0131za\"]", "Fil", null },
                    { new Guid("d3000001-0001-0001-0001-000000000005"), "Hayvanlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"sekiz\",\"kol\",\"deniz\",\"m\\u00FCrekkep\",\"y\\u00FCzmek\"]", "Ahtapot", null },
                    { new Guid("d4000001-0001-0001-0001-000000000001"), "Eğlence", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"palya\\u00E7o\",\"akrobat\",\"g\\u00F6steri\",\"\\u00E7ad\\u0131r\",\"hayvan\"]", "Sirk", null },
                    { new Guid("d4000001-0001-0001-0001-000000000002"), "Eğlence", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"\\u015Fark\\u0131\",\"mikrofon\",\"ekran\",\"s\\u00F6z\",\"e\\u011Flence\"]", "Karaoke", null },
                    { new Guid("d4000001-0001-0001-0001-000000000003"), "Eğlence", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"d\\u00F6nme dolap\",\"h\\u0131z treni\",\"e\\u011Flence\",\"bilet\",\"atl\\u0131kar\\u0131nca\"]", "Lunapark", null },
                    { new Guid("d5000001-0001-0001-0001-000000000001"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"ge\\u00E7mi\\u015F\",\"\\u00F6zlem\",\"an\\u0131\",\"eski\",\"hat\\u0131ra\"]", "Nostalji", null },
                    { new Guid("d5000001-0001-0001-0001-000000000002"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"oy\",\"halk\",\"se\\u00E7im\",\"\\u00F6zg\\u00FCrl\\u00FCk\",\"y\\u00F6netim\"]", "Demokrasi", null },
                    { new Guid("d5000001-0001-0001-0001-000000000003"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"anlama\",\"duygu\",\"kar\\u015F\\u0131 taraf\",\"hissetmek\",\"ba\\u015Fkas\\u0131\"]", "Empati", null },
                    { new Guid("d5000001-0001-0001-0001-000000000004"), "Kavramlar", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, true, "[\"tersine\",\"alay\",\"anlam\",\"s\\u00F6z\",\"beklenti\"]", "Ironi", null },
                    { new Guid("d6000001-0001-0001-0001-000000000001"), "Sinema", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"\\u00F6d\\u00FCl\",\"Hollywood\",\"film\",\"heykelcik\",\"t\\u00F6ren\"]", "Oscar", null },
                    { new Guid("d6000001-0001-0001-0001-000000000002"), "Sinema", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"b\\u00F6l\\u00FCm\",\"sezon\",\"televizyon\",\"izlemek\",\"oyuncu\"]", "Dizi", null },
                    { new Guid("d6000001-0001-0001-0001-000000000003"), "Sinema", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"\\u00E7izgi film\",\"karakter\",\"Pixar\",\"canland\\u0131rma\",\"Disney\"]", "Animasyon", null },
                    { new Guid("d7000001-0001-0001-0001-000000000001"), "Edebiyat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"kitap\",\"yazar\",\"sayfa\",\"okumak\",\"hikaye\"]", "Roman", null },
                    { new Guid("d7000001-0001-0001-0001-000000000002"), "Edebiyat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, true, "[\"m\\u0131sra\",\"kafiye\",\"dize\",\"\\u015Fair\",\"\\u00F6l\\u00E7\\u00FC\"]", "Şiir", null },
                    { new Guid("d7000001-0001-0001-0001-000000000003"), "Edebiyat", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, true, "[\"\\u00E7ocuk\",\"prenses\",\"bir varm\\u0131\\u015F\",\"hayal\",\"ejderha\"]", "Masal", null },
                    { new Guid("e3a8b9c0-3456-7890-12ef-1234567890ab"), "Teknoloji", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"AI\",\"makine\",\"\\u00F6\\u011Frenme\",\"algoritma\",\"robot\"]", "Yapay Zeka", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId_CreatedAt",
                table: "ActivityLogs",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengedId",
                table: "Challenges",
                column: "ChallengedId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_ChallengerId",
                table: "Challenges",
                column: "ChallengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_VersusGameId",
                table: "Challenges",
                column: "VersusGameId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_WordId",
                table: "Challenges",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_AddresseeId",
                table: "Friendships",
                column: "AddresseeId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_RequesterId_AddresseeId",
                table: "Friendships",
                columns: new[] { "RequesterId", "AddresseeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameAttempts_GameSessionId",
                table: "GameAttempts",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserId",
                table: "GameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_WordId",
                table: "GameSessions",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_UserId_BadgeId",
                table: "UserBadges",
                columns: new[] { "UserId", "BadgeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FacebookId",
                table: "Users",
                column: "FacebookId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStatistics_UserId",
                table: "UserStatistics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VersusGames_Player1GameSessionId",
                table: "VersusGames",
                column: "Player1GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VersusGames_Player1Id",
                table: "VersusGames",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_VersusGames_Player2GameSessionId",
                table: "VersusGames",
                column: "Player2GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VersusGames_Player2Id",
                table: "VersusGames",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_VersusGames_RoomCode",
                table: "VersusGames",
                column: "RoomCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersusGames_WordId",
                table: "VersusGames",
                column: "WordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "GameAttempts");

            migrationBuilder.DropTable(
                name: "UserBadges");

            migrationBuilder.DropTable(
                name: "UserStatistics");

            migrationBuilder.DropTable(
                name: "VersusGames");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
