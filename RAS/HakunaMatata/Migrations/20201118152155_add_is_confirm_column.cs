using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class add_is_confirm_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirm",
                table: "REAL_ESTATE",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirm",
                table: "REAL_ESTATE");
        }
    }
}
