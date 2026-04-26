using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSSClientTickets.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Client_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Client_Addr1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Client_Addr2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Client_City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Client_State = table.Column<string>(type: "nchar(2)", fixedLength: true, maxLength: 2, nullable: true),
                    Client_Zip = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientRec);
                });

            migrationBuilder.CreateTable(
                name: "TicketStatus",
                columns: table => new
                {
                    StatusRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketStatus", x => x.StatusRec);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientRec = table.Column<int>(type: "int", nullable: false),
                    Customer_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Customer_Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Customer_Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Customer_Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerRec);
                    table.ForeignKey(
                        name: "FK_Customer_Client",
                        column: x => x.ClientRec,
                        principalTable: "Client",
                        principalColumn: "ClientRec");
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    SiteRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientRec = table.Column<int>(type: "int", nullable: false),
                    Site_Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Site_Address1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Site_Address2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Site_City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Site_State = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Site_Zip = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.SiteRec);
                    table.ForeignKey(
                        name: "FK_Site_Client",
                        column: x => x.ClientRec,
                        principalTable: "Client",
                        principalColumn: "ClientRec");
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientRec = table.Column<int>(type: "int", nullable: false),
                    CustomerRec = table.Column<int>(type: "int", nullable: false),
                    Issue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateLogged = table.Column<DateTime>(type: "datetime", nullable: true),
                    DateResolved = table.Column<DateTime>(type: "datetime", nullable: true),
                    StatusRec = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    SiteRec = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.TicketRec);
                    table.ForeignKey(
                        name: "FK_Ticket_Client",
                        column: x => x.ClientRec,
                        principalTable: "Client",
                        principalColumn: "ClientRec");
                    table.ForeignKey(
                        name: "FK_Ticket_Customer",
                        column: x => x.CustomerRec,
                        principalTable: "Customer",
                        principalColumn: "CustomerRec");
                    table.ForeignKey(
                        name: "FK_Ticket_Site",
                        column: x => x.SiteRec,
                        principalTable: "Site",
                        principalColumn: "SiteRec");
                    table.ForeignKey(
                        name: "FK_Ticket_TicketStatus",
                        column: x => x.StatusRec,
                        principalTable: "TicketStatus",
                        principalColumn: "StatusRec");
                });

            migrationBuilder.CreateTable(
                name: "TicketTime",
                columns: table => new
                {
                    TimeRec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketRec = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTime", x => x.TimeRec);
                    table.ForeignKey(
                        name: "FK_TicketTime_Ticket",
                        column: x => x.TicketRec,
                        principalTable: "Ticket",
                        principalColumn: "TicketRec");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ClientRec",
                table: "Customer",
                column: "ClientRec");

            migrationBuilder.CreateIndex(
                name: "IX_Site_ClientRec",
                table: "Site",
                column: "ClientRec");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_ClientRec",
                table: "Ticket",
                column: "ClientRec");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CustomerRec",
                table: "Ticket",
                column: "CustomerRec");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_SiteRec",
                table: "Ticket",
                column: "SiteRec");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_StatusRec",
                table: "Ticket",
                column: "StatusRec");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTime_TicketRec",
                table: "TicketTime",
                column: "TicketRec");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketTime");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "TicketStatus");

            migrationBuilder.DropTable(
                name: "Client");
        }
    }
}
