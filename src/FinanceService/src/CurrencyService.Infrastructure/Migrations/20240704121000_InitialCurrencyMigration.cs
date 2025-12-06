using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCurrencyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currency",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currency", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_currency_code",
                table: "currency",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currency");
        }
    }
}
