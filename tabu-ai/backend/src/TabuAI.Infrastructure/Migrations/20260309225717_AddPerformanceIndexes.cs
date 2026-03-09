using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Words_IsActive",
                table: "Words",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Words_IsActive_Language",
                table: "Words",
                columns: new[] { "IsActive", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_StartedAt",
                table: "GameSessions",
                column: "StartedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_IsActive",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_IsActive_Language",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_StartedAt",
                table: "GameSessions");
        }
    }
}
