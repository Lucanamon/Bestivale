using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SoldAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BuyerUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketListings_Monsters_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monsters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketListings_Users_SellerUserId",
                        column: x => x.SellerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 12, 8, 18, 49, 458, DateTimeKind.Utc).AddTicks(1755), "$2a$11$m3S3oC8rnurPWE89LOWW1.8obFTCpJ7BvBy4LEJ41d.TzSvfZQRzW" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_CreatedAt",
                table: "MarketListings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_MonsterId",
                table: "MarketListings",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_SellerUserId",
                table: "MarketListings",
                column: "SellerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_Status",
                table: "MarketListings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_Status_Price",
                table: "MarketListings",
                columns: new[] { "Status", "Price" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketListings");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 12, 7, 29, 48, 433, DateTimeKind.Utc).AddTicks(1257), "$2a$11$.Xh.V.uo6cvoLEPdXz2WpOwesMlxzqfYzhATanYVcl/eM.z4UxA4K" });
        }
    }
}
