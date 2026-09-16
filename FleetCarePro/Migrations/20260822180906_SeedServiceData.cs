using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FleetCarePro.Migrations
{
    /// <inheritdoc />
    public partial class SeedServiceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServiceCategories",
                columns: new[] { "Id", "CategoryName", "Description", "RecommendedIntervalMonths" },
                values: new object[,]
                {
                    { 1, "Oil Change", null, 0 },
                    { 2, "Tire Replacement", null, 0 },
                    { 3, "Brake Repair", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "ServiceCenters",
                columns: new[] { "Id", "Address", "Email", "IsActive", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "", "", true, "Main Workshop - Cairo", "" },
                    { 2, "", "", true, "Giza Auto Maintenance", "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceCenters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceCenters",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
