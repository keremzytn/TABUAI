using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameEconomyAndCaching : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BestStreak",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPlayedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromptCoins",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CoinTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoinTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "LastPlayedAt",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_CoinTransactions_UserId_CreatedAt",
                table: "CoinTransactions",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoinTransactions");

            migrationBuilder.DropColumn(
                name: "BestStreak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastPlayedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PromptCoins",
                table: "Users");
        }
    }
}
