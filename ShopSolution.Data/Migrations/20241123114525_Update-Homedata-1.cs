using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopSolution.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomedata1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 6);

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
                columns: new[] { "Description", "Image", "Name" },
                values: new object[] { "20% off the all order", "img/offer-2.png", "Spring Collection" });

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Image", "Name" },
                values: new object[] { "20% off the all order", "img/offer-1.png", "Spring Collection" });

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Image", "Name", "SortOrder" },
                values: new object[] { "20% off the all order", "img/offer-2.png", "Spring Collection", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                keyValue: 2,
                columns: new[] { "Description", "Image", "Name" },
                values: new object[] { "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", "/themes/images/carousel/2.png", "Second Thumbnail label" });

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Image", "Name" },
                values: new object[] { "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", "/themes/images/carousel/3.png", "Second Thumbnail label" });

            migrationBuilder.UpdateData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Image", "Name", "SortOrder" },
                values: new object[] { "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", "/themes/images/carousel/4.png", "Second Thumbnail label", 4 });

            migrationBuilder.InsertData(
                table: "Slides",
                columns: new[] { "Id", "Description", "Image", "Name", "SortOrder", "Status", "Url" },
                values: new object[,]
                {
                    { 5, "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", "/themes/images/carousel/5.png", "Second Thumbnail label", 5, 1, "#" },
                    { 6, "Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec id elit non mi porta gravida at eget metus. Nullam id dolor id nibh ultricies vehicula ut id elit.", "/themes/images/carousel/6.png", "Second Thumbnail label", 6, 1, "#" }
                });
        }
    }
}
