using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "shippingCost",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 12, 21, 21, 6, 25, 322, DateTimeKind.Local).AddTicks(4882));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shippingCost",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 12, 21, 16, 24, 13, 590, DateTimeKind.Local).AddTicks(7408));
        }
    }
}
