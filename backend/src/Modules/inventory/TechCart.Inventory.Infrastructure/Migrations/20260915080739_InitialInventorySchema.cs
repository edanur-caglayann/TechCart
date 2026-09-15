using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialInventorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.CreateTable(
                name: "product_stock",
                schema: "inventory",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    is_ready_to_ship = table.Column<bool>(type: "boolean", nullable: false),
                    has_fast_delivery = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_stock", x => x.product_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_stock",
                schema: "inventory");
        }
    }
}
