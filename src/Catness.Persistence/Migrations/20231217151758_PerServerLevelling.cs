using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catness.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PerServerLevelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "experience",
                table: "users");

            migrationBuilder.DropColumn(
                name: "level",
                table: "users");

            migrationBuilder.DropColumn(
                name: "private_user",
                table: "users");

            migrationBuilder.AddColumn<decimal>(
                name: "experience",
                table: "guild_users",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "level",
                table: "guild_users",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "experience",
                table: "guild_users");

            migrationBuilder.DropColumn(
                name: "level",
                table: "guild_users");

            migrationBuilder.AddColumn<decimal>(
                name: "experience",
                table: "users",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "level",
                table: "users",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<bool>(
                name: "private_user",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
