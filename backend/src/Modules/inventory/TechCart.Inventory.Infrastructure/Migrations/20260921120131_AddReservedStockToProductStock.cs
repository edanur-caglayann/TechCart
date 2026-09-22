using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservedStockToProductStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reserved_stock",
                schema: "inventory",
                table: "product_stock",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reserved_stock",
                schema: "inventory",
                table: "product_stock");
        }
    }
}
