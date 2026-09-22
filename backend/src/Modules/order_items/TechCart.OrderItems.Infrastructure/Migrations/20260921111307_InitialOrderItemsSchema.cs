using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.OrderItems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrderItemsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "order_items");

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "order_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    product_name = table.Column<string>(type: "text", nullable: false),
                    product_model = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                schema: "order_items",
                table: "order_items",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items",
                schema: "order_items");
        }
    }
}
