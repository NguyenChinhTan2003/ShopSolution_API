using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class Sold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sold",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Sold" },
                values: new object[] { new DateTime(2024, 12, 11, 17, 6, 45, 71, DateTimeKind.Local).AddTicks(7471), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sold",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 12, 8, 1, 23, 31, 627, DateTimeKind.Local).AddTicks(8389));
        }
    }
}
