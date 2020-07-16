using Microsoft.EntityFrameworkCore.Migrations;

namespace OFTENCOFTAPI.Data.Migrations
{
    public partial class removed_navigation_prop_from_App_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResetTokens_AspNetUsers_UserId",
                table: "ResetTokens");

            migrationBuilder.DropIndex(
                name: "IX_ResetTokens_UserId",
                table: "ResetTokens");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ResetTokens");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ResetTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ResetTokens",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "ResetTokens",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ResetTokens_UserId",
                table: "ResetTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResetTokens_AspNetUsers_UserId",
                table: "ResetTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
