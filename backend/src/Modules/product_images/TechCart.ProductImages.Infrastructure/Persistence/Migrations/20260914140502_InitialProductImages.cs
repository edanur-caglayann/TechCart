using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.ProductImages.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "product_images");

            migrationBuilder.CreateTable(
                name: "product_images",
                schema: "product_images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_images_product_id",
                schema: "product_images",
                table: "product_images",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_images",
                schema: "product_images");
        }
    }
}
