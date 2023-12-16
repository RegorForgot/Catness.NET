using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catness.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NamingChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Users_FollowedId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_Users_FollowerId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildUsers_Guilds_GuildId",
                table: "GuildUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildUsers_Users_UserId",
                table: "GuildUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follows",
                table: "Follows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuildUsers",
                table: "GuildUsers");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Reminders",
                newName: "reminders");

            migrationBuilder.RenameTable(
                name: "Guilds",
                newName: "guilds");

            migrationBuilder.RenameTable(
                name: "Follows",
                newName: "follows");

            migrationBuilder.RenameTable(
                name: "GuildUsers",
                newName: "guild_users");

            migrationBuilder.RenameColumn(
                name: "Rep",
                table: "users",
                newName: "rep");

            migrationBuilder.RenameColumn(
                name: "Locale",
                table: "users",
                newName: "locale");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "users",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Experience",
                table: "users",
                newName: "experience");

            migrationBuilder.RenameColumn(
                name: "Birthday",
                table: "users",
                newName: "birthday");

            migrationBuilder.RenameColumn(
                name: "SteamVanity",
                table: "users",
                newName: "steam_vanity");

            migrationBuilder.RenameColumn(
                name: "PrivateUser",
                table: "users",
                newName: "private_user");

            migrationBuilder.RenameColumn(
                name: "LevellingEnabled",
                table: "users",
                newName: "levelling_enabled");

            migrationBuilder.RenameColumn(
                name: "LastfmUsername",
                table: "users",
                newName: "lastfm_username");

            migrationBuilder.RenameColumn(
                name: "LastRepTime",
                table: "users",
                newName: "last_rep_time");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Reminded",
                table: "reminders",
                newName: "reminded");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "reminders",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "reminders",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TimeCreated",
                table: "reminders",
                newName: "time_created");

            migrationBuilder.RenameColumn(
                name: "ReminderTime",
                table: "reminders",
                newName: "reminder_time");

            migrationBuilder.RenameColumn(
                name: "PrivateReminder",
                table: "reminders",
                newName: "private_reminder");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "reminders",
                newName: "channel_id");

            migrationBuilder.RenameColumn(
                name: "ReminderGuid",
                table: "reminders",
                newName: "reminder_guid");

            migrationBuilder.RenameIndex(
                name: "IX_Reminders_UserId",
                table: "reminders",
                newName: "ix_reminders_user_id");

            migrationBuilder.RenameColumn(
                name: "LevellingEnabled",
                table: "guilds",
                newName: "levelling_enabled");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "guilds",
                newName: "guild_id");

            migrationBuilder.RenameColumn(
                name: "FollowedId",
                table: "follows",
                newName: "followed_id");

            migrationBuilder.RenameColumn(
                name: "FollowerId",
                table: "follows",
                newName: "follower_id");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowedId",
                table: "follows",
                newName: "ix_follows_followed_id");

            migrationBuilder.RenameColumn(
                name: "IsLevelBlacklisted",
                table: "guild_users",
                newName: "is_level_blacklisted");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "guild_users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "guild_users",
                newName: "guild_id");

            migrationBuilder.RenameIndex(
                name: "IX_GuildUsers_UserId",
                table: "guild_users",
                newName: "ix_guild_users_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_reminders",
                table: "reminders",
                column: "reminder_guid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_guilds",
                table: "guilds",
                column: "guild_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_follows",
                table: "follows",
                columns: new[] { "follower_id", "followed_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_guild_users",
                table: "guild_users",
                columns: new[] { "guild_id", "user_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_follows_users_followed_id",
                table: "follows",
                column: "followed_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_follows_users_follower_id",
                table: "follows",
                column: "follower_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_guild_users_guilds_guild_id",
                table: "guild_users",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "guild_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_guild_users_users_user_id",
                table: "guild_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_reminders_users_user_id",
                table: "reminders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_follows_users_followed_id",
                table: "follows");

            migrationBuilder.DropForeignKey(
                name: "fk_follows_users_follower_id",
                table: "follows");

            migrationBuilder.DropForeignKey(
                name: "fk_guild_users_guilds_guild_id",
                table: "guild_users");

            migrationBuilder.DropForeignKey(
                name: "fk_guild_users_users_user_id",
                table: "guild_users");

            migrationBuilder.DropForeignKey(
                name: "fk_reminders_users_user_id",
                table: "reminders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_reminders",
                table: "reminders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_guilds",
                table: "guilds");

            migrationBuilder.DropPrimaryKey(
                name: "pk_follows",
                table: "follows");

            migrationBuilder.DropPrimaryKey(
                name: "pk_guild_users",
                table: "guild_users");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "reminders",
                newName: "Reminders");

            migrationBuilder.RenameTable(
                name: "guilds",
                newName: "Guilds");

            migrationBuilder.RenameTable(
                name: "follows",
                newName: "Follows");

            migrationBuilder.RenameTable(
                name: "guild_users",
                newName: "GuildUsers");

            migrationBuilder.RenameColumn(
                name: "rep",
                table: "Users",
                newName: "Rep");

            migrationBuilder.RenameColumn(
                name: "locale",
                table: "Users",
                newName: "Locale");

            migrationBuilder.RenameColumn(
                name: "level",
                table: "Users",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "experience",
                table: "Users",
                newName: "Experience");

            migrationBuilder.RenameColumn(
                name: "birthday",
                table: "Users",
                newName: "Birthday");

            migrationBuilder.RenameColumn(
                name: "steam_vanity",
                table: "Users",
                newName: "SteamVanity");

            migrationBuilder.RenameColumn(
                name: "private_user",
                table: "Users",
                newName: "PrivateUser");

            migrationBuilder.RenameColumn(
                name: "levelling_enabled",
                table: "Users",
                newName: "LevellingEnabled");

            migrationBuilder.RenameColumn(
                name: "lastfm_username",
                table: "Users",
                newName: "LastfmUsername");

            migrationBuilder.RenameColumn(
                name: "last_rep_time",
                table: "Users",
                newName: "LastRepTime");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "reminded",
                table: "Reminders",
                newName: "Reminded");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "Reminders",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Reminders",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "time_created",
                table: "Reminders",
                newName: "TimeCreated");

            migrationBuilder.RenameColumn(
                name: "reminder_time",
                table: "Reminders",
                newName: "ReminderTime");

            migrationBuilder.RenameColumn(
                name: "private_reminder",
                table: "Reminders",
                newName: "PrivateReminder");

            migrationBuilder.RenameColumn(
                name: "channel_id",
                table: "Reminders",
                newName: "ChannelId");

            migrationBuilder.RenameColumn(
                name: "reminder_guid",
                table: "Reminders",
                newName: "ReminderGuid");

            migrationBuilder.RenameIndex(
                name: "ix_reminders_user_id",
                table: "Reminders",
                newName: "IX_Reminders_UserId");

            migrationBuilder.RenameColumn(
                name: "levelling_enabled",
                table: "Guilds",
                newName: "LevellingEnabled");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "Guilds",
                newName: "GuildId");

            migrationBuilder.RenameColumn(
                name: "followed_id",
                table: "Follows",
                newName: "FollowedId");

            migrationBuilder.RenameColumn(
                name: "follower_id",
                table: "Follows",
                newName: "FollowerId");

            migrationBuilder.RenameIndex(
                name: "ix_follows_followed_id",
                table: "Follows",
                newName: "IX_Follows_FollowedId");

            migrationBuilder.RenameColumn(
                name: "is_level_blacklisted",
                table: "GuildUsers",
                newName: "IsLevelBlacklisted");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "GuildUsers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "guild_id",
                table: "GuildUsers",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "ix_guild_users_user_id",
                table: "GuildUsers",
                newName: "IX_GuildUsers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders",
                column: "ReminderGuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "GuildId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follows",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowedId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuildUsers",
                table: "GuildUsers",
                columns: new[] { "GuildId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Users_FollowedId",
                table: "Follows",
                column: "FollowedId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_Users_FollowerId",
                table: "Follows",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuildUsers_Guilds_GuildId",
                table: "GuildUsers",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "GuildId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuildUsers_Users_UserId",
                table: "GuildUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Users_UserId",
                table: "Reminders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
