using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Brands.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialBrandsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "brands");

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_brands_name",
                schema: "brands",
                table: "brands",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brands",
                schema: "brands");
        }
    }
}
