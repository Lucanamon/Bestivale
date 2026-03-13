using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEggIsListed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsListed",
                table: "Eggs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 13, 9, 10, 57, 605, DateTimeKind.Utc).AddTicks(3398), "$2a$11$1CIs07VXvy8MkD.I7IZ0yeyXvbQb/..JiSmo5oxBUM1Xs3kMVW/kG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsListed",
                table: "Eggs");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 13, 7, 23, 8, 514, DateTimeKind.Utc).AddTicks(8190), "$2a$11$CsGtdsMT00mGXlJbqXATCejPOxLpCssvosuPhQ864t9STzQhRso9e" });
        }
    }
}
