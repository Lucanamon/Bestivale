using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Mythology = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "User"),
                    CurrencyBalance = table.Column<int>(type: "integer", nullable: false, defaultValue: 10),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRootAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Eggs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ColorHex = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ColorDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsListed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eggs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Eggs_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Rarity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsListed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketListings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonsterId = table.Column<Guid>(type: "uuid", nullable: false),
                    EggId = table.Column<Guid>(type: "uuid", nullable: true),
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
                        name: "FK_MarketListings_Eggs_EggId",
                        column: x => x.EggId,
                        principalTable: "Eggs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CurrencyBalance", "IsRootAdmin", "PasswordHash", "Role", "Username" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 3, 19, 9, 43, 42, 194, DateTimeKind.Utc).AddTicks(7391), 9999, true, "$2a$11$fh8CNisPuswzmFV3bkjDIeAq49nu9DFLvkh18a53sNM4BEeTp1wMG", "RootAdmin", "rootadmin" });

            migrationBuilder.CreateIndex(
                name: "IX_Eggs_OwnerUserId",
                table: "Eggs",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CreatedAt",
                table: "InventoryItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_OwnerUserId",
                table: "InventoryItems",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_CreatedAt",
                table: "MarketListings",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MarketListings_EggId",
                table: "MarketListings",
                column: "EggId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "MarketListings");

            migrationBuilder.DropTable(
                name: "Eggs");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
