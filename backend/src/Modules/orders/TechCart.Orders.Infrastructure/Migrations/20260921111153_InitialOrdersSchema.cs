using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Orders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrdersSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    vat_total = table.Column<decimal>(type: "numeric", nullable: false),
                    shipping_fee = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    shipping_full_name = table.Column<string>(type: "text", nullable: false),
                    shipping_phone = table.Column<string>(type: "text", nullable: false),
                    shipping_city = table.Column<string>(type: "text", nullable: false),
                    shipping_district = table.Column<string>(type: "text", nullable: false),
                    shipping_address_line = table.Column<string>(type: "text", nullable: false),
                    shipping_postal_code = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_number",
                schema: "orders",
                table: "orders",
                column: "order_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders",
                schema: "orders");
        }
    }
}
