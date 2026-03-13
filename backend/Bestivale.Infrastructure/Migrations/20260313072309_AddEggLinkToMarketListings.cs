using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEggLinkToMarketListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EggId",
                table: "MarketListings",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 13, 7, 23, 8, 514, DateTimeKind.Utc).AddTicks(8190), "$2a$11$CsGtdsMT00mGXlJbqXATCejPOxLpCssvosuPhQ864t9STzQhRso9e" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_EggId",
                table: "MarketListings",
                column: "EggId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketListings_Eggs_EggId",
                table: "MarketListings",
                column: "EggId",
                principalTable: "Eggs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketListings_Eggs_EggId",
                table: "MarketListings");

            migrationBuilder.DropIndex(
                name: "IX_MarketListings_EggId",
                table: "MarketListings");

            migrationBuilder.DropColumn(
                name: "EggId",
                table: "MarketListings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 12, 9, 30, 53, 926, DateTimeKind.Utc).AddTicks(7404), "$2a$11$e6NYVA/ULur6V3b0uxoTrekAnJ.dymAWB8feZfkQwTxZLoI94TOwe" });
        }
    }
}
