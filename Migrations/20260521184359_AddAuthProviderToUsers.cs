using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andromeda.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthProviderToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "auth_provider",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "local");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auth_provider",
                table: "users");
        }
    }
}
