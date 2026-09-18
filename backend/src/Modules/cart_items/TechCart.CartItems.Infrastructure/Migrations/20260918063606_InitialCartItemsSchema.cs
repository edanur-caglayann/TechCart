using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.CartItems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCartItemsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cart_items");

            migrationBuilder.CreateTable(
                name: "cart_items",
                schema: "cart_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart_items", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_user_id_product_id",
                schema: "cart_items",
                table: "cart_items",
                columns: new[] { "user_id", "product_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_items",
                schema: "cart_items");
        }
    }
}
