using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OFTENCOFTAPI.Migrations
{
    public partial class datenowstring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "logged",
                table: "logs",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "logged",
                table: "logs",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
