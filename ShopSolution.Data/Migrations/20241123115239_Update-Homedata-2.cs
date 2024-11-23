using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomedata2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 11, 23, 18, 52, 39, 134, DateTimeKind.Local).AddTicks(9700));

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Spring Collection2");

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Spring Collection3");

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "SortOrder" },
                values: new object[] { "Spring Collection4", 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 11, 23, 18, 45, 24, 678, DateTimeKind.Local).AddTicks(886));

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Spring Collection");

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Spring Collection");

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Name", "SortOrder" },
                values: new object[] { "Spring Collection", 3 });
        }
    }
}
