using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSSClientTickets.Migrations
{
    /// <inheritdoc />
    public partial class RequireFirstAndLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AppUser",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE AppUser
                SET
                    FirstName = LEFT(LTRIM(RTRIM(ISNULL(DisplayName, ''))), 50),
                    LastName = ''
                WHERE NULLIF(LTRIM(RTRIM(ISNULL(DisplayName, ''))), '') IS NOT NULL;

                UPDATE AppUser
                SET
                    FirstName = LEFT(LEFT(LTRIM(RTRIM(DisplayName)), CHARINDEX(' ', LTRIM(RTRIM(DisplayName)) + ' ') - 1), 50),
                    LastName = LEFT(LTRIM(SUBSTRING(LTRIM(RTRIM(DisplayName)), CHARINDEX(' ', LTRIM(RTRIM(DisplayName)) + ' ') + 1, 100)), 50)
                WHERE CHARINDEX(' ', LTRIM(RTRIM(DisplayName))) > 0;
                """);

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AppUser");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AppUser",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AppUser",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE AppUser
                SET DisplayName = NULLIF(LTRIM(RTRIM(FirstName + ' ' + LastName)), '');
                """);

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AppUser");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AppUser");
        }
    }
}
