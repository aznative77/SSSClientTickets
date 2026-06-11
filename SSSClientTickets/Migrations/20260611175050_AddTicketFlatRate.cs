using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSSClientTickets.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketFlatRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFlatRate",
                table: "Ticket",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlatRate",
                table: "Ticket");
        }
    }
}
