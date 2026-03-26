using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryItemsHub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // InventoryItems table already exists from 20260319094342_AddInventoryItems.
            // Upgrade it in-place to support polymorphic InventoryItem head.
            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "InventoryItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InventoryEggs",
                columns: table => new
                {
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ColorHex = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ColorDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEggs", x => x.InventoryItemId);
                    table.ForeignKey(
                        name: "FK_InventoryEggs_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Backfill: convert existing Eggs into InventoryItems + InventoryEggs (one-time)
            // This is safe to run multiple times because we only insert when no InventoryEgg exists for the same Id.
            migrationBuilder.Sql(@"
INSERT INTO ""InventoryItems"" (""Id"", ""OwnerUserId"", ""ItemType"", ""IsFavorite"", ""IsListed"", ""CreatedAt"", ""Name"", ""Type"", ""Rarity"")
SELECT e.""Id"", e.""OwnerUserId"", 1, e.""IsFavorite"", e.""IsListed"", e.""CreatedAt"", 'Embryo mutagen egg', 'Egg', 'Common'
FROM ""Eggs"" e
WHERE NOT EXISTS (
  SELECT 1 FROM ""InventoryEggs"" ie WHERE ie.""InventoryItemId"" = e.""Id""
);

INSERT INTO ""InventoryEggs"" (""InventoryItemId"", ""TemplateCode"", ""ColorHex"", ""ColorDescription"", ""CreatedAt"")
SELECT e.""Id"", e.""TemplateCode"", e.""ColorHex"", e.""ColorDescription"", e.""CreatedAt""
FROM ""Eggs"" e
WHERE NOT EXISTS (
  SELECT 1 FROM ""InventoryEggs"" ie WHERE ie.""InventoryItemId"" = e.""Id""
);
");

            migrationBuilder.CreateTable(
                name: "InventoryMonsters",
                columns: table => new
                {
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryMonsters", x => x.InventoryItemId);
                    table.ForeignKey(
                        name: "FK_InventoryMonsters_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryMonsters_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 26, 10, 36, 33, 181, DateTimeKind.Utc).AddTicks(2655), "$2a$11$VsALkrxPYU9CnKYlvJ1YlOR0CHoN7BGDlbUH5oGz91PariXY51Ixa" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_OwnerUserId_CreatedAt",
                table: "InventoryItems",
                columns: new[] { "OwnerUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_OwnerUserId_IsFavorite",
                table: "InventoryItems",
                columns: new[] { "OwnerUserId", "IsFavorite" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_OwnerUserId_ItemType",
                table: "InventoryItems",
                columns: new[] { "OwnerUserId", "ItemType" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMonsters_MonsterId",
                table: "InventoryMonsters",
                column: "MonsterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryEggs");

            migrationBuilder.DropTable(
                name: "InventoryMonsters");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_OwnerUserId_CreatedAt",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_OwnerUserId_IsFavorite",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_OwnerUserId_ItemType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "InventoryItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 26, 8, 58, 45, 419, DateTimeKind.Utc).AddTicks(5428), "$2a$11$/D4IU7FIRskKrJVXUlqVROCJqc6jePQIBuPHz6j0COH2PYCyiKowq" });
        }
    }
}
