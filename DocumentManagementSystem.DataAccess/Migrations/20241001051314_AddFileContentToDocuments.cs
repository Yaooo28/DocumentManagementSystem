using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.DataAccess.Migrations
{
    public partial class AddFileContentToDocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Documents",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Documents");
        }
    }
}
