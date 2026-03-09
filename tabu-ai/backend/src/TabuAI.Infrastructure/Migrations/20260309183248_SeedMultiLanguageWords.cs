using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMultiLanguageWords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Words",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Words",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "tr");

            migrationBuilder.AddColumn<Guid>(
                name: "WordPackId",
                table: "Words",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DailyChallenges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChallengeDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WordId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "tr"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyChallenges_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WordPacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "tr"),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false),
                    LikeCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordPacks_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyChallengeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyChallengeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    AttemptsUsed = table.Column<int>(type: "integer", nullable: false),
                    TimeTaken = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyChallengeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyChallengeEntries_DailyChallenges_DailyChallengeId",
                        column: x => x.DailyChallengeId,
                        principalTable: "DailyChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DailyChallengeEntries_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DailyChallengeEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a5555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a7777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a8888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a9999999-9999-9999-9999-999999999999"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b3333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b4444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b5555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b6666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b7777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000006"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c1000001-0001-0001-0001-000000000007"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c2a7b8c9-2345-6789-01cd-ef1234567890"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c3000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c4000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c5000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c6000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c7000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c7000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c7000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c8000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c8000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c8000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c9000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c9000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("c9000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d1b6a7b8-1234-5678-90ab-cdef12345678"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d2000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d3000001-0001-0001-0001-000000000005"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d4000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d4000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d4000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d5000001-0001-0001-0001-000000000004"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d6000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d6000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d6000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d7000001-0001-0001-0001-000000000001"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d7000001-0001-0001-0001-000000000002"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("d7000001-0001-0001-0001-000000000003"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3a8b9c0-3456-7890-12ef-1234567890ab"),
                columns: new[] { "CreatedByUserId", "Language", "WordPackId" },
                values: new object[] { null, "tr", null });

            migrationBuilder.InsertData(
                table: "Words",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedByUserId", "Difficulty", "IsActive", "Language", "TabuWords", "TargetWord", "UpdatedAt", "WordPackId" },
                values: new object[,]
                {
                    { new Guid("e1000001-0001-0001-0001-000000000001"), "Transportation", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"fly\",\"wing\",\"pilot\",\"sky\",\"airport\"]", "Airplane", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000002"), "Transportation", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"underwater\",\"navy\",\"torpedo\",\"deep\",\"periscope\"]", "Submarine", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000003"), "Transportation", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"rotor\",\"blade\",\"hover\",\"fly\",\"rescue\"]", "Helicopter", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000004"), "Technology", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"screen\",\"keyboard\",\"mouse\",\"internet\",\"processor\"]", "Computer", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000005"), "Technology", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"call\",\"app\",\"screen\",\"touch\",\"mobile\"]", "Smartphone", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000006"), "Technology", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"AI\",\"machine\",\"learning\",\"robot\",\"algorithm\"]", "Artificial Intelligence", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000007"), "Technology", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"crypto\",\"bitcoin\",\"ledger\",\"decentralized\",\"mining\"]", "Blockchain", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000008"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"fall\",\"Newton\",\"force\",\"weight\",\"earth\"]", "Gravity", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000009"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"plant\",\"sun\",\"chlorophyll\",\"oxygen\",\"leaf\"]", "Photosynthesis", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000010"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"lava\",\"eruption\",\"mountain\",\"magma\",\"ash\"]", "Volcano", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000011"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"space\",\"gravity\",\"light\",\"star\",\"singularity\"]", "Black Hole", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000012"), "Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"cheese\",\"dough\",\"slice\",\"Italian\",\"oven\"]", "Pizza", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000013"), "Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"Japanese\",\"rice\",\"fish\",\"raw\",\"seaweed\"]", "Sushi", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000014"), "Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"sweet\",\"cocoa\",\"candy\",\"brown\",\"milk\"]", "Chocolate", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000015"), "Food", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"French\",\"pastry\",\"butter\",\"breakfast\",\"flaky\"]", "Croissant", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000016"), "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"hoop\",\"ball\",\"dunk\",\"NBA\",\"court\"]", "Basketball", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000017"), "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"running\",\"42km\",\"endurance\",\"race\",\"finish line\"]", "Marathon", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000018"), "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"rings\",\"medal\",\"athlete\",\"torch\",\"games\"]", "Olympics", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000019"), "History", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"Egypt\",\"pharaoh\",\"stone\",\"ancient\",\"tomb\"]", "Pyramid", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000020"), "History", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"art\",\"rebirth\",\"Italy\",\"Leonardo\",\"culture\"]", "Renaissance", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000021"), "History", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"Rome\",\"arena\",\"fight\",\"sword\",\"Colosseum\"]", "Gladiator", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000022"), "Nature", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"color\",\"rain\",\"sun\",\"seven\",\"arc\"]", "Rainbow", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000023"), "Nature", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"fault\",\"shake\",\"Richter\",\"tremor\",\"destruction\"]", "Earthquake", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000024"), "Nature", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"northern lights\",\"sky\",\"polar\",\"magnetic\",\"green\"]", "Aurora", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000025"), "Animals", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"ocean\",\"smart\",\"jump\",\"mammal\",\"fin\"]", "Dolphin", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000026"), "Animals", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"color change\",\"reptile\",\"tongue\",\"camouflage\",\"eyes\"]", "Chameleon", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000027"), "Animals", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "en", "[\"ice\",\"bird\",\"Antarctica\",\"waddle\",\"tuxedo\"]", "Penguin", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000028"), "Concepts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "en", "[\"vote\",\"people\",\"election\",\"freedom\",\"government\"]", "Democracy", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000029"), "Concepts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"past\",\"memory\",\"longing\",\"old\",\"remember\"]", "Nostalgia", null, null },
                    { new Guid("e1000001-0001-0001-0001-000000000030"), "Concepts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "en", "[\"fate\",\"action\",\"consequence\",\"balance\",\"destiny\"]", "Karma", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000001"), "Verkehr", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"fliegen\",\"Fl\\u00FCgel\",\"Pilot\",\"Himmel\",\"Flughafen\"]", "Flugzeug", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000002"), "Verkehr", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "de", "[\"Unterwasser\",\"Marine\",\"Torpedo\",\"tauchen\",\"Periskop\"]", "U-Boot", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000003"), "Verkehr", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Pedal\",\"Rad\",\"Kette\",\"Lenker\",\"fahren\"]", "Fahrrad", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000004"), "Technologie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Bildschirm\",\"Tastatur\",\"Maus\",\"Internet\",\"Prozessor\"]", "Computer", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000005"), "Technologie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "de", "[\"KI\",\"Maschine\",\"Lernen\",\"Roboter\",\"Algorithmus\"]", "Künstliche Intelligenz", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000006"), "Technologie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Sicherheit\",\"geheim\",\"Zugang\",\"Zeichen\",\"Login\"]", "Passwort", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000007"), "Wissenschaft", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "de", "[\"fallen\",\"Newton\",\"Kraft\",\"Gewicht\",\"Erde\"]", "Schwerkraft", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000008"), "Wissenschaft", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "de", "[\"Ersch\\u00FCtterung\",\"Richter\",\"Platte\",\"Zerst\\u00F6rung\",\"Beben\"]", "Erdbeben", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000009"), "Wissenschaft", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "de", "[\"Weltraum\",\"Gravitation\",\"Licht\",\"Stern\",\"Singularit\\u00E4t\"]", "Schwarzes Loch", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000010"), "Essen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Teig\",\"Salz\",\"Bayern\",\"B\\u00E4ckerei\",\"Knoten\"]", "Brezel", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000011"), "Essen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Kohl\",\"sauer\",\"fermentiert\",\"Beilage\",\"deutsch\"]", "Sauerkraut", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000012"), "Essen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "de", "[\"Schokolade\",\"Kirsche\",\"Sahne\",\"Kuchen\",\"Torte\"]", "Schwarzwälder Kirschtorte", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000013"), "Sport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Ball\",\"Tor\",\"Mannschaft\",\"Stadion\",\"Bundesliga\"]", "Fußball", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000014"), "Sport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Schnee\",\"Berg\",\"Piste\",\"Winter\",\"Lift\"]", "Skifahren", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000015"), "Geschichte", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "de", "[\"Teilung\",\"Ost\",\"West\",\"Grenze\",\"1989\"]", "Berliner Mauer", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000016"), "Geschichte", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"R\\u00FCstung\",\"Schwert\",\"Burg\",\"Mittelalter\",\"Pferd\"]", "Ritter", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000017"), "Natur", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Farbe\",\"Regen\",\"Sonne\",\"sieben\",\"Bogen\"]", "Regenbogen", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000018"), "Natur", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Wasser\",\"fallen\",\"Felsen\",\"Natur\",\"flie\\u00DFen\"]", "Wasserfall", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000019"), "Natur", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "de", "[\"Aurora\",\"Himmel\",\"Polar\",\"Magnetfeld\",\"gr\\u00FCn\"]", "Nordlicht", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000020"), "Tiere", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Fl\\u00FCgel\",\"Raupe\",\"bunt\",\"fliegen\",\"Nektar\"]", "Schmetterling", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000021"), "Tiere", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"Nuss\",\"Baum\",\"klettern\",\"Schwanz\",\"Eiche\"]", "Eichhörnchen", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000022"), "Kultur", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "de", "[\"M\\u00FCnchen\",\"Bier\",\"Fest\",\"Tracht\",\"Zelt\"]", "Oktoberfest", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000023"), "Kultur", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "de", "[\"Schwarzwald\",\"Uhr\",\"Vogel\",\"Holz\",\"Zeit\"]", "Kuckucksuhr", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000024"), "Konzepte", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "de", "[\"Reise\",\"Sehnsucht\",\"fern\",\"Heimweh\",\"Welt\"]", "Fernweh", null, null },
                    { new Guid("e2000001-0001-0001-0001-000000000025"), "Konzepte", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "de", "[\"Epoche\",\"Geist\",\"Gesellschaft\",\"Trend\",\"Stimmung\"]", "Zeitgeist", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000001"), "Transport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"voler\",\"aile\",\"pilote\",\"ciel\",\"a\\u00E9roport\"]", "Avion", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000002"), "Transport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"souterrain\",\"station\",\"ticket\",\"Paris\",\"ligne\"]", "Métro", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000003"), "Transport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"p\\u00E9dale\",\"roue\",\"cha\\u00EEne\",\"guidon\",\"rouler\"]", "Vélo", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000004"), "Technologie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"\\u00E9cran\",\"clavier\",\"souris\",\"internet\",\"processeur\"]", "Ordinateur", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000005"), "Technologie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "fr", "[\"IA\",\"machine\",\"apprentissage\",\"robot\",\"algorithme\"]", "Intelligence Artificielle", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000006"), "Technologie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"s\\u00E9curit\\u00E9\",\"secret\",\"acc\\u00E8s\",\"caract\\u00E8re\",\"connexion\"]", "Mot de passe", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000007"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"tomber\",\"Newton\",\"force\",\"poids\",\"terre\"]", "Gravité", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000008"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "fr", "[\"espace\",\"gravit\\u00E9\",\"lumi\\u00E8re\",\"\\u00E9toile\",\"galaxie\"]", "Trou noir", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000009"), "Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"lave\",\"\\u00E9ruption\",\"montagne\",\"magma\",\"cendre\"]", "Volcan", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000010"), "Gastronomie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"beurre\",\"p\\u00E2te\",\"petit-d\\u00E9jeuner\",\"boulangerie\",\"feuillet\\u00E9\"]", "Croissant", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000011"), "Gastronomie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"p\\u00E2te\",\"fine\",\"Bretagne\",\"sucre\",\"po\\u00EAle\"]", "Crêpe", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000012"), "Gastronomie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"l\\u00E9gume\",\"Provence\",\"film\",\"aubergine\",\"courgette\"]", "Ratatouille", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000013"), "Gastronomie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"pain\",\"long\",\"cro\\u00FBte\",\"boulanger\",\"farine\"]", "Baguette", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000014"), "Gastronomie", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"lait\",\"Camembert\",\"Brie\",\"vache\",\"affiner\"]", "Fromage", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000015"), "Sport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"ballon\",\"but\",\"\\u00E9quipe\",\"stade\",\"match\"]", "Football", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000016"), "Sport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"v\\u00E9lo\",\"cyclisme\",\"\\u00E9tape\",\"maillot jaune\",\"course\"]", "Tour de France", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000017"), "Sport", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"\\u00E9p\\u00E9e\",\"touche\",\"masque\",\"piste\",\"duel\"]", "Escrime", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000018"), "Histoire", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"1789\",\"Bastille\",\"libert\\u00E9\",\"peuple\",\"guillotine\"]", "Révolution", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000019"), "Histoire", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"empereur\",\"guerre\",\"Waterloo\",\"Corse\",\"conqu\\u00EAte\"]", "Napoléon", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000020"), "Histoire", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"ch\\u00E2teau\",\"roi\",\"jardin\",\"Louis\",\"palace\"]", "Versailles", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000021"), "Nature", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"couleur\",\"pluie\",\"soleil\",\"sept\",\"ciel\"]", "Arc-en-ciel", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000022"), "Nature", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"eau\",\"tomber\",\"rocher\",\"rivi\\u00E8re\",\"nature\"]", "Cascade", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000023"), "Animaux", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"aile\",\"chenille\",\"color\\u00E9\",\"voler\",\"nectar\"]", "Papillon", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000024"), "Animaux", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"mer\",\"intelligent\",\"sauter\",\"mammif\\u00E8re\",\"nager\"]", "Dauphin", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000025"), "Animaux", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"couleur\",\"reptile\",\"langue\",\"camouflage\",\"yeux\"]", "Caméléon", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000026"), "Culture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "fr", "[\"Paris\",\"fer\",\"tour\",\"monument\",\"Gustave\"]", "Tour Eiffel", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000027"), "Culture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "fr", "[\"Monet\",\"peinture\",\"lumi\\u00E8re\",\"art\",\"mouvement\"]", "Impressionnisme", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000028"), "Concepts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"vote\",\"peuple\",\"\\u00E9lection\",\"libert\\u00E9\",\"gouvernement\"]", "Démocratie", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000029"), "Concepts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "fr", "[\"bonheur\",\"vie\",\"plaisir\",\"enthousiasme\",\"vivre\"]", "Joie de vivre", null, null },
                    { new Guid("e3000001-0001-0001-0001-000000000030"), "Concepts", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "fr", "[\"libre\",\"droit\",\"prison\",\"ind\\u00E9pendance\",\"r\\u00E9volution\"]", "Liberté", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_CreatedByUserId",
                table: "Words",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_WordPackId",
                table: "Words",
                column: "WordPackId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallengeEntries_DailyChallengeId_UserId",
                table: "DailyChallengeEntries",
                columns: new[] { "DailyChallengeId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallengeEntries_GameSessionId",
                table: "DailyChallengeEntries",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallengeEntries_UserId",
                table: "DailyChallengeEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallenges_ChallengeDate_Language",
                table: "DailyChallenges",
                columns: new[] { "ChallengeDate", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallenges_WordId",
                table: "DailyChallenges",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordPacks_CreatedByUserId",
                table: "WordPacks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WordPacks_Language",
                table: "WordPacks",
                column: "Language");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Users_CreatedByUserId",
                table: "Words",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_WordPacks_WordPackId",
                table: "Words",
                column: "WordPackId",
                principalTable: "WordPacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Users_CreatedByUserId",
                table: "Words");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_WordPacks_WordPackId",
                table: "Words");

            migrationBuilder.DropTable(
                name: "DailyChallengeEntries");

            migrationBuilder.DropTable(
                name: "WordPacks");

            migrationBuilder.DropTable(
                name: "DailyChallenges");

            migrationBuilder.DropIndex(
                name: "IX_Words_CreatedByUserId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_WordPackId",
                table: "Words");

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000008"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000009"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000010"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000011"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000012"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000013"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000014"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000015"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000016"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000017"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000018"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000019"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000020"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000021"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000022"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000023"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000024"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000025"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000026"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000027"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000028"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000029"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e1000001-0001-0001-0001-000000000030"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000008"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000009"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000010"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000011"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000012"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000013"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000014"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000015"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000016"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000017"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000018"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000019"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000020"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000021"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000022"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000023"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000024"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e2000001-0001-0001-0001-000000000025"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000005"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000006"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000007"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000008"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000009"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000010"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000011"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000012"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000013"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000014"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000015"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000016"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000017"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000018"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000019"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000020"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000021"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000022"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000023"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000024"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000025"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000026"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000027"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000028"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000029"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("e3000001-0001-0001-0001-000000000030"));

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "WordPackId",
                table: "Words");
        }
    }
}
