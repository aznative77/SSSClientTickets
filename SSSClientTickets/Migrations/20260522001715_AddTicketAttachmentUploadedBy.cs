using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSSClientTickets.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAttachmentUploadedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UploadedByUserId",
                table: "TicketAttachment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachment_UploadedByUserId",
                table: "TicketAttachment",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachment_UploadedBy_AppUser",
                table: "TicketAttachment",
                column: "UploadedByUserId",
                principalTable: "AppUser",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachment_UploadedBy_AppUser",
                table: "TicketAttachment");

            migrationBuilder.DropIndex(
                name: "IX_TicketAttachment_UploadedByUserId",
                table: "TicketAttachment");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "TicketAttachment");
        }
    }
}
