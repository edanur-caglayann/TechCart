using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Addresses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNeighborhoodToAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                schema: "addresses",
                table: "addresses",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "neighborhood",
                schema: "addresses",
                table: "addresses");
        }
    }
}
