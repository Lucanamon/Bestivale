using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEggs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eggs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ColorHex = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ColorDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 12, 9, 30, 53, 926, DateTimeKind.Utc).AddTicks(7404), "$2a$11$e6NYVA/ULur6V3b0uxoTrekAnJ.dymAWB8feZfkQwTxZLoI94TOwe" });

            migrationBuilder.CreateIndex(
                name: "IX_Eggs_OwnerUserId",
                table: "Eggs",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Eggs");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 12, 8, 18, 49, 458, DateTimeKind.Utc).AddTicks(1755), "$2a$11$m3S3oC8rnurPWE89LOWW1.8obFTCpJ7BvBy4LEJ41d.TzSvfZQRzW" });
        }
    }
}
