using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class shipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Shippings",
                newName: "Ward");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 12, 21, 16, 24, 13, 590, DateTimeKind.Local).AddTicks(7408));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Shippings");

            migrationBuilder.RenameColumn(
                name: "Ward",
                table: "Shippings",
                newName: "Address");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 12, 20, 22, 21, 27, 612, DateTimeKind.Local).AddTicks(3942));
        }
    }
}
