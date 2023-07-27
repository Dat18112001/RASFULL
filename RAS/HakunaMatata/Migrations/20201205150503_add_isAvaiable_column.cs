using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class add_isAvaiable_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvaiable",
                table: "REAL_ESTATE",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvaiable",
                table: "REAL_ESTATE");
        }
    }
}
