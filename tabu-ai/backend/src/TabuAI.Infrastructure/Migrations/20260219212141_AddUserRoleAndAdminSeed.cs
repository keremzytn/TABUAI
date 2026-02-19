using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAndAdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "DisplayName", "Email", "GamesPlayed", "GamesWon", "IsActive", "LastLoginAt", "Level", "PasswordHash", "Role", "TotalScore", "Username" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin User", "admin@tabuai.com", 0, 0, true, null, 1, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgwd966VQRfAk5U5Z6.t.15v8vS6", 2, 0, "admin" });

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
                    { new Guid("b7777777-7777-7777-7777-777777777777"), "Müzik", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, true, "[\"\\u015Fark\\u0131\",\"sahne\",\"soprano\",\"orkestra\",\"tiyatro\"]", "Opera", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a3333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a4444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a5555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a6666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a7777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a8888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("a9999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b1111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b2222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b3333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b4444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b5555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b6666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: new Guid("b7777777-7777-7777-7777-777777777777"));

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
