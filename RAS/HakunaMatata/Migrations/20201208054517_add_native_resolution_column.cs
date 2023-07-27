using Microsoft.EntityFrameworkCore.Migrations;

namespace HakunaMatata.Migrations
{
    public partial class add_native_resolution_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__MAP__RealEstateI__49C3F6B7",
                table: "MAP");

            migrationBuilder.DropForeignKey(
                name: "FK__PICTURE__RealEst__4CA06362",
                table: "PICTURE");

            migrationBuilder.DropForeignKey(
                name: "FK__REAL_ESTA__Agent__3F466844",
                table: "REAL_ESTATE");

            migrationBuilder.DropForeignKey(
                name: "FK__REAL_ESTA__ReaEs__3E52440B",
                table: "REAL_ESTATE");

            migrationBuilder.DropForeignKey(
                name: "FK__REAL_ESTA__RealE__5629CD9C",
                table: "REAL_ESTATE_DETAIL");

            migrationBuilder.AddColumn<int>(
                name: "NativeHeight",
                table: "PICTURE",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NativeWidth",
                table: "PICTURE",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK__MAP__RealEstateI__49C3F6B7",
                table: "MAP",
                column: "RealEstateId",
                principalTable: "REAL_ESTATE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__PICTURE__RealEst__4CA06362",
                table: "PICTURE",
                column: "RealEstateId",
                principalTable: "REAL_ESTATE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__REAL_ESTA__Agent__3F466844",
                table: "REAL_ESTATE",
                column: "AgentId",
                principalTable: "AGENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__REAL_ESTA__ReaEs__3E52440B",
                table: "REAL_ESTATE",
                column: "RealEstateTypeId",
                principalTable: "REAL_ESTATE_TYPE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__REAL_ESTA__RealE__5629CD9C",
                table: "REAL_ESTATE_DETAIL",
                column: "RealEstateId",
                principalTable: "REAL_ESTATE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__MAP__RealEstateI__49C3F6B7",
                table: "MAP");

            migrationBuilder.DropForeignKey(
                name: "FK__PICTURE__RealEst__4CA06362",
                table: "PICTURE");

            migrationBuilder.DropForeignKey(
                name: "FK__REAL_ESTA__Agent__3F466844",
                table: "REAL_ESTATE");

            migrationBuilder.DropForeignKey(
                name: "FK__REAL_ESTA__ReaEs__3E52440B",
                table: "REAL_ESTATE");

            migrationBuilder.DropForeignKey(
                name: "FK__REAL_ESTA__RealE__5629CD9C",
                table: "REAL_ESTATE_DETAIL");

            migrationBuilder.DropColumn(
                name: "NativeHeight",
                table: "PICTURE");

            migrationBuilder.DropColumn(
                name: "NativeWidth",
                table: "PICTURE");

            migrationBuilder.AddForeignKey(
                name: "FK__MAP__RealEstateI__49C3F6B7",
                table: "MAP",
                column: "RealEstateId",
                principalTable: "REAL_ESTATE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__PICTURE__RealEst__4CA06362",
                table: "PICTURE",
                column: "RealEstateId",
                principalTable: "REAL_ESTATE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__REAL_ESTA__Agent__3F466844",
                table: "REAL_ESTATE",
                column: "AgentId",
                principalTable: "AGENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__REAL_ESTA__ReaEs__3E52440B",
                table: "REAL_ESTATE",
                column: "RealEstateTypeId",
                principalTable: "REAL_ESTATE_TYPE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__REAL_ESTA__RealE__5629CD9C",
                table: "REAL_ESTATE_DETAIL",
                column: "RealEstateId",
                principalTable: "REAL_ESTATE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
