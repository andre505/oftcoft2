using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OFTENCOFTAPI.Migrations
{
    public partial class logg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    application = table.Column<string>(type: "varchar", nullable: true),
                    logged = table.Column<DateTime>(nullable: true),
                    level = table.Column<string>(type: "varchar", nullable: true),
                    message = table.Column<string>(type: "varchar", nullable: true),
                    logger = table.Column<string>(type: "varchar", nullable: true),
                    callsite = table.Column<string>(type: "varchar", nullable: true),
                    exception = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs");
        }
    }
}
