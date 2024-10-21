using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmondoBackendAssignment.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountBalances",
                columns: table => new
                {
                    amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    currencyCode = table.Column<string>(type: "TEXT", nullable: false),
                    updateInstant = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    packageNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalances_updateInstant",
                table: "AccountBalances",
                column: "updateInstant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalances");

            migrationBuilder.DropTable(
                name: "Shipments");
        }
    }
}
