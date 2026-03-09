using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabuAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMicrosoftSSO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MicrosoftId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "MicrosoftId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MicrosoftId",
                table: "Users",
                column: "MicrosoftId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_MicrosoftId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MicrosoftId",
                table: "Users");
        }
    }
}
