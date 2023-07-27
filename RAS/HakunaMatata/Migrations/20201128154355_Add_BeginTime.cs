using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class Add_BeginTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BeginTime",
                table: "REAL_ESTATE",
                nullable: false,
                defaultValue: new DateTime(2019,12,21));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeginTime",
                table: "REAL_ESTATE");
        }
    }
}
