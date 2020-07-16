using Microsoft.EntityFrameworkCore.Migrations;

namespace OFTENCOFTAPI.Data.Migrations
{
    public partial class changed_user_id_in_reset_token_table_to_string : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ResetTokens",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ResetTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
