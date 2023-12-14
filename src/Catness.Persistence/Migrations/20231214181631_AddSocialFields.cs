using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catness.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastfmUsername",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SteamVanity",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastfmUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SteamVanity",
                table: "Users");
        }
    }
}
