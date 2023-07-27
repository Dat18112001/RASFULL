using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class add_confirm_phone_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmPhoneNumber",
                table: "AGENT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ABOUT_US",
                type: "ntext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmPhoneNumber",
                table: "AGENT");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ABOUT_US",
                type: "ntext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "ntext");
        }
    }
}
