using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GameEconomyGamification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoubleCoinGamesLeft",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasStreakShield",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectedAvatar",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCardDesign",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShopPurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopPurchases_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "IsActive", "Name", "RequiredValue", "Type" },
                values: new object[,]
                {
                    { new Guid("c1d2e3f4-7890-1234-5634-567890abcdef"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "5 günlük seri yaptın!", "/badges/streak-5.svg", true, "Seri Başlatıcı", 5, 6 },
                    { new Guid("d2e3f4a5-8901-2345-6745-67890abcdef0"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "15 günlük seri yaptın!", "/badges/streak-15.svg", true, "Seri Ustası", 15, 6 },
                    { new Guid("e3f4a5b6-9012-3456-7856-7890abcdef01"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "30 günlük seri yaptın!", "/badges/streak-30.svg", true, "Seri Efsanesi", 30, 6 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "SelectedAvatar", "SelectedCardDesign" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ShopPurchases_UserId_ItemId",
                table: "ShopPurchases",
                columns: new[] { "UserId", "ItemId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopPurchases");

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: new Guid("c1d2e3f4-7890-1234-5634-567890abcdef"));

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: new Guid("d2e3f4a5-8901-2345-6745-67890abcdef0"));

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: new Guid("e3f4a5b6-9012-3456-7856-7890abcdef01"));

            migrationBuilder.DropColumn(
                name: "DoubleCoinGamesLeft",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HasStreakShield",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedAvatar",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedCardDesign",
                table: "Users");
        }
    }
}
