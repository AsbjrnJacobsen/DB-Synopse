using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class dummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VisableFlag",
                table: "ordersTable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "ProductsTables",
                columns: new[] { "ProductId", "ProductDescription", "ProductName" },
                values: new object[,]
                {
                    { 1, "Product 1 Description", "Product 1" },
                    { 2, "Product 2 Description", "Product 2" },
                    { 3, "Product 3 Description", "Product 3" }
                });

            migrationBuilder.InsertData(
                table: "ordersTable",
                columns: new[] { "OrderId", "ProductId", "VisableFlag" },
                values: new object[,]
                {
                    { 1, 1, true },
                    { 2, 2, true },
                    { 3, 3, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ordersTable",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ordersTable",
                keyColumn: "OrderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ordersTable",
                keyColumn: "OrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductsTables",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductsTables",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductsTables",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "VisableFlag",
                table: "ordersTable");
        }
    }
}
