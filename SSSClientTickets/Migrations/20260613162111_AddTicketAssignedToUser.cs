using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSSClientTickets.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAssignedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Ticket",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Ticket
                SET AssignedToUserId = CreatedByUserId
                WHERE AssignedToUserId IS NULL
                    AND CreatedByUserId IS NOT NULL
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_AssignedToUserId",
                table: "Ticket",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_AssignedTo_AppUser",
                table: "Ticket",
                column: "AssignedToUserId",
                principalTable: "AppUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_AssignedTo_AppUser",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_AssignedToUserId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Ticket");
        }
    }
}
