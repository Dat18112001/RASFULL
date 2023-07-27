using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class add_contact_number : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "REAL_ESTATE",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "REAL_ESTATE");
        }
    }
}
