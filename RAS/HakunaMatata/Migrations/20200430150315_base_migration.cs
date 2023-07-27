using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class base_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ABOUT_US");

            migrationBuilder.DropTable(
                name: "FAQ");

            migrationBuilder.DropTable(
                name: "MAP");

            migrationBuilder.DropTable(
                name: "PICTURE");

            migrationBuilder.DropTable(
                name: "POLICY");

            migrationBuilder.DropTable(
                name: "RATING");

            migrationBuilder.DropTable(
                name: "REAL_ESTATE_DETAIL");

            migrationBuilder.DropTable(
                name: "SOCIAL_LOGIN");

            migrationBuilder.DropTable(
                name: "WARD");

            migrationBuilder.DropTable(
                name: "REAL_ESTATE");

            migrationBuilder.DropTable(
                name: "DISTRICT");

            migrationBuilder.DropTable(
                name: "AGENT");

            migrationBuilder.DropTable(
                name: "REAL_ESTATE_TYPE");

            migrationBuilder.DropTable(
                name: "CITY");

            migrationBuilder.DropTable(
                name: "LEVEL");
        }
    }
}
