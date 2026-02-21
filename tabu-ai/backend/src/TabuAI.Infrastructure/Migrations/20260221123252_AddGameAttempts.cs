using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGameAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiFeedback",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "PromptQuality",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "Suggestions",
                table: "GameSessions");

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

            migrationBuilder.CreateIndex(
                name: "IX_GameAttempts_GameSessionId",
                table: "GameAttempts",
                column: "GameSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameAttempts");

            migrationBuilder.AddColumn<string>(
                name: "AiFeedback",
                table: "GameSessions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromptQuality",
                table: "GameSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suggestions",
                table: "GameSessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
