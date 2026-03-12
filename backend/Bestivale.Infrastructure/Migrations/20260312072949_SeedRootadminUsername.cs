using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRootadminUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "Username" },
                values: new object[] { new DateTime(2026, 3, 12, 7, 29, 48, 433, DateTimeKind.Utc).AddTicks(1257), "$2a$11$.Xh.V.uo6cvoLEPdXz2WpOwesMlxzqfYzhATanYVcl/eM.z4UxA4K", "rootadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "PasswordHash", "Username" },
                values: new object[] { new DateTime(2026, 3, 6, 11, 17, 54, 99, DateTimeKind.Utc).AddTicks(4231), "$2a$11$gVAcwO2YT04otBIssZhiZeAyw7LyMjdzkBAAnrLDpqUTS0kJFadjS", "madmin" });
        }
    }
}
