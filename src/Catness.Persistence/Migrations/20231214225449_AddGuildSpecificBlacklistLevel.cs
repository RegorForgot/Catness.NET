using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catness.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildSpecificBlacklistLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLevelBlacklisted",
                table: "GuildUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLevelBlacklisted",
                table: "GuildUsers");
        }
    }
}
