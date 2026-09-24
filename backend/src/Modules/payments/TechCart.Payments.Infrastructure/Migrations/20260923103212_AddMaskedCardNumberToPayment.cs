using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaskedCardNumberToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "masked_card_number",
                schema: "payments",
                table: "payments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "masked_card_number",
                schema: "payments",
                table: "payments");
        }
    }
}
