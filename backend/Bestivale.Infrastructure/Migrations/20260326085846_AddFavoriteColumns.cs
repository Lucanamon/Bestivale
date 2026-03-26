using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bestivale.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Eggs"
                ADD COLUMN IF NOT EXISTS "IsFavorite" boolean NOT NULL DEFAULT FALSE;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "Monsters"
                ADD COLUMN IF NOT EXISTS "IsFavorite" boolean NOT NULL DEFAULT FALSE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Monsters"
                DROP COLUMN IF EXISTS "IsFavorite";
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "Eggs"
                DROP COLUMN IF EXISTS "IsFavorite";
                """);
        }
    }
}
