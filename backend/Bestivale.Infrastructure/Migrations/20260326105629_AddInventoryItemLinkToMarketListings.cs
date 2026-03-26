using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryItemLinkToMarketListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "MarketListings",
                type: "uuid",
                nullable: true);

            // Backfill: egg listings can link directly to InventoryItem (we use Egg.Id as InventoryItem.Id).
            migrationBuilder.Sql(@"
UPDATE ""MarketListings"" ml
SET ""InventoryItemId"" = ml.""EggId""
WHERE ml.""InventoryItemId"" IS NULL
  AND ml.""EggId"" IS NOT NULL;
");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 26, 10, 56, 28, 728, DateTimeKind.Utc).AddTicks(4366), "$2a$11$V05ed4ZVNbkc3QK70Mdjh.ysKavuvCQZbwmF5qTYZdSLIqujf0/cu" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_InventoryItemId",
                table: "MarketListings",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketListings_InventoryItems_InventoryItemId",
                table: "MarketListings",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketListings_InventoryItems_InventoryItemId",
                table: "MarketListings");

            migrationBuilder.DropIndex(
                name: "IX_MarketListings_InventoryItemId",
                table: "MarketListings");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "MarketListings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 26, 10, 36, 33, 181, DateTimeKind.Utc).AddTicks(2655), "$2a$11$VsALkrxPYU9CnKYlvJ1YlOR0CHoN7BGDlbUH5oGz91PariXY51Ixa" });
        }
    }
}
