using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OFTENCOFTAPI.Data.Migrations
{
    public partial class removefieldtodebug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordTokenExpiryTime",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordToken",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordTokenExpiryTime",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
