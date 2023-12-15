using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catness.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReminderIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ReminderId",
                table: "Reminders");

            migrationBuilder.AddColumn<Guid>(
                name: "ReminderGuid",
                table: "Reminders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders",
                column: "ReminderGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ReminderGuid",
                table: "Reminders");

            migrationBuilder.AddColumn<decimal>(
                name: "ReminderId",
                table: "Reminders",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reminders",
                table: "Reminders",
                column: "ReminderId");
        }
    }
}
