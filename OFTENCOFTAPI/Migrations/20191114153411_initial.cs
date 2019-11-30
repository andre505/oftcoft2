using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OFTENCOFTAPI.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "itemcategories",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    categoryname = table.Column<string>(maxLength: 255, nullable: true),
                    categorydescription = table.Column<string>(maxLength: 255, nullable: true),
                    datecreated = table.Column<DateTime>(nullable: true),
                    datemodified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemcategories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    quantity = table.Column<int>(nullable: true),
                    customercode = table.Column<string>(maxLength: 255, nullable: true),
                    paystackreference = table.Column<string>(maxLength: 255, nullable: true),
                    email = table.Column<string>(maxLength: 255, nullable: true),
                    TicketReferences = table.Column<string>(nullable: true),
                    customerid = table.Column<string>(maxLength: 255, nullable: true),
                    cardlast4 = table.Column<string>(maxLength: 255, nullable: true),
                    cardexpmonth = table.Column<string>(maxLength: 255, nullable: true),
                    cardexpyear = table.Column<string>(maxLength: 255, nullable: true),
                    cardchannel = table.Column<string>(maxLength: 255, nullable: true),
                    cardtype = table.Column<string>(maxLength: 255, nullable: true),
                    countrycode = table.Column<string>(maxLength: 255, nullable: true),
                    ipaddress = table.Column<string>(maxLength: 255, nullable: true),
                    customerlocation = table.Column<string>(maxLength: 255, nullable: true),
                    bank = table.Column<string>(maxLength: 255, nullable: true),
                    transactionstatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    totalamount = table.Column<decimal>(type: "numeric(255,0)", nullable: true),
                    paymentdate = table.Column<DateTime>(nullable: true),
                    datemodified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    itemname = table.Column<string>(maxLength: 255, nullable: true),
                    Itemdescription = table.Column<string>(nullable: true),
                    categoryid = table.Column<int>(nullable: false),
                    ticketamount = table.Column<decimal>(type: "decimal(20,2)", nullable: true),
                    datecreated = table.Column<DateTime>(nullable: true),
                    datemodified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                    table.ForeignKey(
                        name: "item_catid",
                        column: x => x.categoryid,
                        principalTable: "itemcategories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "draws",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    itemid = table.Column<int>(nullable: false),
                    noofwinners = table.Column<int>(nullable: false),
                    drawwinners = table.Column<int>(nullable: false),
                    drawtype = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    drawdate = table.Column<DateTime>(nullable: true),
                    drawstatus = table.Column<string>(type: "varchar(50)", maxLength: 255, nullable: true),
                    datecreated = table.Column<DateTime>(nullable: true),
                    datemodified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_draws", x => x.id);
                    table.ForeignKey(
                        name: "draws_itemid",
                        column: x => x.itemid,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    drawid = table.Column<int>(nullable: true),
                    firstname = table.Column<string>(maxLength: 255, nullable: true),
                    lastname = table.Column<string>(maxLength: 255, nullable: true),
                    emailaddress = table.Column<string>(maxLength: 255, nullable: true),
                    phonenumber = table.Column<string>(maxLength: 255, nullable: true),
                    ticketreference = table.Column<string>(maxLength: 255, nullable: true),
                    accesscode = table.Column<string>(maxLength: 255, nullable: true),
                    paystackreference = table.Column<string>(maxLength: 255, nullable: true),
                    winstatus = table.Column<string>(type: "varchar(50)", maxLength: 255, nullable: true, defaultValueSql: "NULL::character varying"),
                    claimstatus = table.Column<string>(type: "varchar(50)", maxLength: 255, nullable: true),
                    transactionid = table.Column<int>(nullable: true),
                    datemodified = table.Column<DateTime>(nullable: true),
                    confirmstatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.id);
                    table.ForeignKey(
                        name: "ticket_drawid",
                        column: x => x.drawid,
                        principalTable: "draws",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ticket_transactionid",
                        column: x => x.transactionid,
                        principalTable: "transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_draws_itemid",
                table: "draws",
                column: "itemid");

            migrationBuilder.CreateIndex(
                name: "IX_items_categoryid",
                table: "items",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_drawid",
                table: "tickets",
                column: "drawid");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_transactionid",
                table: "tickets",
                column: "transactionid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "draws");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "itemcategories");
        }
    }
}
