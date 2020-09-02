using Microsoft.EntityFrameworkCore.Migrations;

namespace SportStore.Migrations
{
    public partial class AddDepartmentToOutfi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Outfits");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "Outfits",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Outfits_DepartmentName",
                table: "Outfits",
                column: "DepartmentName");

            migrationBuilder.AddForeignKey(
                name: "FK_Outfits_Departments_DepartmentName",
                table: "Outfits",
                column: "DepartmentName",
                principalTable: "Departments",
                principalColumn: "DepartmentName",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Outfits_Departments_DepartmentName",
                table: "Outfits");

            migrationBuilder.DropIndex(
                name: "IX_Outfits_DepartmentName",
                table: "Outfits");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "Outfits");

            migrationBuilder.AddColumn<int>(
                name: "Department",
                table: "Outfits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
