using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocumentManagementSystem.DataAccess.Migrations
{
    public partial class AddFileFieldsToDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Documents");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Documents");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Documents",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
