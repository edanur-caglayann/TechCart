using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Products.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "stock",
                schema: "products",
                table: "products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stock",
                schema: "products",
                table: "products");
        }
    }
}
