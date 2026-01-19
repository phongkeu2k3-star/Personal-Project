using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "CurrentPrice", "LastUpdated", "Name", "PriceChange24h", "Symbol" },
                values: new object[,]
                {
                    { 1, 65000m, new DateTime(2026, 1, 19, 16, 5, 39, 476, DateTimeKind.Local).AddTicks(5630), "Bitcoin", 0.0, "BTC" },
                    { 2, 3500m, new DateTime(2026, 1, 19, 16, 5, 39, 476, DateTimeKind.Local).AddTicks(5645), "Ethereum", 0.0, "ETH" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
