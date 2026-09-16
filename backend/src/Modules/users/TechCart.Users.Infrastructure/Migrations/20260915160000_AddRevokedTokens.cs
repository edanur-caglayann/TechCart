using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechCart.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRevokedTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.CreateTable(
                name: "revoked_tokens",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    jti = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revoked_tokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_revoked_tokens_expires_at",
                schema: "users",
                table: "revoked_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_revoked_tokens_jti",
                schema: "users",
                table: "revoked_tokens",
                column: "jti",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "revoked_tokens",
                schema: "users");
        }
    }
}
