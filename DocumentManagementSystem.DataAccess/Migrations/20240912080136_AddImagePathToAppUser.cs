using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.DataAccess.Migrations
{
    public partial class AddImagePathToAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AppUsers");
        }
    }
}
