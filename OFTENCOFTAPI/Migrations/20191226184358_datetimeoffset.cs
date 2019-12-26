using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OFTENCOFTAPI.Migrations
{
    public partial class datetimeoffset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "logged",
                table: "logs",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "logged",
                table: "logs",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);
        }
    }
}
