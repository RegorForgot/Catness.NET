using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catness.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerChannelFiltering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "guild_channels",
                columns: table => new
                {
                    channel_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    commands_disabled = table.Column<bool>(type: "boolean", nullable: false),
                    levelling_enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guild_channels", x => x.channel_id);
                    table.ForeignKey(
                        name: "fk_guild_channels_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "guild_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_guild_channels_guild_id",
                table: "guild_channels",
                column: "guild_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_channels");
        }
    }
}
