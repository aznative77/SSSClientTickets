using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSSClientTickets.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeRecordedByUserId",
                table: "TicketTime",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResolvedByUserId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ChangeLog",
                columns: table => new
                {
                    ChangeLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityRecordId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeLog", x => x.ChangeLogId);
                    table.ForeignKey(
                        name: "FK_ChangeLog_AppUser",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketTime_TimeRecordedByUserId",
                table: "TicketTime",
                column: "TimeRecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CreatedByUserId",
                table: "Ticket",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ResolvedByUserId",
                table: "Ticket",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Email",
                table: "AppUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeLog_UserId",
                table: "ChangeLog",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_CreatedBy_AppUser",
                table: "Ticket",
                column: "CreatedByUserId",
                principalTable: "AppUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_ResolvedBy_AppUser",
                table: "Ticket",
                column: "ResolvedByUserId",
                principalTable: "AppUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketTime_TimeRecordedBy_AppUser",
                table: "TicketTime",
                column: "TimeRecordedByUserId",
                principalTable: "AppUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_CreatedBy_AppUser",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_ResolvedBy_AppUser",
                table: "Ticket");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketTime_TimeRecordedBy_AppUser",
                table: "TicketTime");

            migrationBuilder.DropTable(
                name: "ChangeLog");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_TicketTime_TimeRecordedByUserId",
                table: "TicketTime");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_CreatedByUserId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_ResolvedByUserId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "TimeRecordedByUserId",
                table: "TicketTime");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "ResolvedByUserId",
                table: "Ticket");
        }
    }
}
