using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarRental.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "FeatureId",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$JcY6Zx3UzLhS5Q5vW8nKj.xC1FgS9Q2M3p4R5t6Y7z8A9b0c1d2e3f4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "FeatureId", "FeatureName", "Icon" },
                values: new object[,]
                {
                    { 5, "GPS Navigation", "map" },
                    { 6, "Bluetooth", "bluetooth" },
                    { 7, "Climate Control", "thermometer" },
                    { 8, "Parking Sensors", "sensor" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 2, 4, 9, 54, 28, 396, DateTimeKind.Utc).AddTicks(5526), "$2a$11$6qqWdLhMbm1bFqkLcTGZpO.smWA/lWhjka9SCLrnyi8m4xI7hsg4u" });
        }
    }
}
