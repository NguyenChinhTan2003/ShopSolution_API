using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopSolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 11, 23, 17, 24, 39, 560, DateTimeKind.Local).AddTicks(976));

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Image", "Name" },
                values: new object[] { "20% off the all order", "img/offer-1.png", "Spring Collection" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2024, 11, 23, 17, 3, 47, 420, DateTimeKind.Local).AddTicks(4405));

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Image", "Name" },
                values: new object[] { "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", "/themes/images/carousel/1.png", "Second Thumbnail label" });
        }
    }
}
