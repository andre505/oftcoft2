﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OFTENCOFTAPI.Models;

namespace OFTENCOFTAPI.Migrations
{
    [DbContext(typeof(OFTENCOFTDBContext))]
    [Migration("20191225184807_logg")]
    partial class logg
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("OFTENCOFTAPI.Models.Draws", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<DateTime?>("Datecreated")
                        .HasColumnName("datecreated");

                    b.Property<DateTime?>("Datemodified")
                        .HasColumnName("datemodified");

                    b.Property<string>("DrawType")
                        .HasColumnName("drawtype")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("Drawdate")
                        .HasColumnName("drawdate");

                    b.Property<string>("Drawstatus")
                        .HasColumnName("drawstatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(255);

                    b.Property<int>("Itemid")
                        .HasColumnName("itemid");

                    b.Property<int>("drawwinners")
                        .HasColumnName("drawwinners");

                    b.Property<int>("noofwinners")
                        .HasColumnName("noofwinners");

                    b.HasKey("Id");

                    b.HasIndex("Itemid");

                    b.ToTable("draws");
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Itemcategories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Categorydescription")
                        .HasColumnName("categorydescription")
                        .HasMaxLength(255);

                    b.Property<string>("Categoryname")
                        .HasColumnName("categoryname")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("Datecreated")
                        .HasColumnName("datecreated");

                    b.Property<DateTime?>("Datemodified")
                        .HasColumnName("datemodified");

                    b.HasKey("Id");

                    b.ToTable("itemcategories");
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Items", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<int>("Categoryid")
                        .HasColumnName("categoryid");

                    b.Property<DateTime?>("Datecreated")
                        .HasColumnName("datecreated");

                    b.Property<DateTime?>("Datemodified")
                        .HasColumnName("datemodified");

                    b.Property<string>("Itemdescription");

                    b.Property<string>("Itemname")
                        .HasColumnName("itemname")
                        .HasMaxLength(255);

                    b.Property<decimal?>("Ticketamount")
                        .HasColumnName("ticketamount")
                        .HasColumnType("decimal(20,2)");

                    b.HasKey("Id");

                    b.HasIndex("Categoryid");

                    b.ToTable("items");
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Logs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Application")
                        .HasColumnName("application")
                        .HasColumnType("varchar");

                    b.Property<string>("Callsite")
                        .HasColumnName("callsite")
                        .HasColumnType("varchar");

                    b.Property<string>("Exception")
                        .HasColumnName("exception")
                        .HasColumnType("varchar");

                    b.Property<string>("Level")
                        .HasColumnName("level")
                        .HasColumnType("varchar");

                    b.Property<DateTime?>("Logged")
                        .HasColumnName("logged");

                    b.Property<string>("Logger")
                        .HasColumnName("logger")
                        .HasColumnType("varchar");

                    b.Property<string>("Message")
                        .HasColumnName("message")
                        .HasColumnType("varchar");

                    b.HasKey("Id");

                    b.ToTable("logs");
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Tickets", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("AccessCode")
                        .HasColumnName("accesscode")
                        .HasMaxLength(255);

                    b.Property<string>("Claimstatus")
                        .HasColumnName("claimstatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(255);

                    b.Property<string>("ConfirmStatus")
                        .HasColumnName("confirmstatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("Datemodified")
                        .HasColumnName("datemodified");

                    b.Property<int?>("Drawid")
                        .HasColumnName("drawid");

                    b.Property<string>("Emailaddress")
                        .HasColumnName("emailaddress")
                        .HasMaxLength(255);

                    b.Property<string>("Firstname")
                        .HasColumnName("firstname")
                        .HasMaxLength(255);

                    b.Property<string>("Lastname")
                        .HasColumnName("lastname")
                        .HasMaxLength(255);

                    b.Property<string>("PaystackReference")
                        .HasColumnName("paystackreference")
                        .HasMaxLength(255);

                    b.Property<string>("Phonenumber")
                        .HasColumnName("phonenumber")
                        .HasMaxLength(255);

                    b.Property<string>("Ticketreference")
                        .HasColumnName("ticketreference")
                        .HasMaxLength(255);

                    b.Property<string>("Winstatus")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("winstatus")
                        .HasColumnType("varchar(50)")
                        .HasDefaultValueSql("NULL::character varying")
                        .HasMaxLength(255);

                    b.Property<int?>("transactionid");

                    b.HasKey("Id");

                    b.HasIndex("Drawid");

                    b.HasIndex("transactionid");

                    b.ToTable("tickets");
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("Bank")
                        .HasColumnName("bank")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("DateModified")
                        .HasColumnName("datemodified");

                    b.Property<string>("Email")
                        .HasColumnName("email")
                        .HasMaxLength(255);

                    b.Property<string>("IPAddress")
                        .HasColumnName("ipaddress")
                        .HasMaxLength(255);

                    b.Property<string>("Location")
                        .HasColumnName("customerlocation")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("Paymentdate")
                        .HasColumnName("paymentdate");

                    b.Property<string>("Pspaymentreference")
                        .HasColumnName("paystackreference")
                        .HasMaxLength(255);

                    b.Property<int?>("Quantity")
                        .HasColumnName("quantity");

                    b.Property<string>("TicketReferences");

                    b.Property<decimal?>("Totalamount")
                        .HasColumnName("totalamount")
                        .HasColumnType("numeric(255,0)");

                    b.Property<string>("TransactionStatus")
                        .HasColumnName("transactionstatus")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("cardchannel")
                        .HasColumnName("cardchannel")
                        .HasMaxLength(255);

                    b.Property<string>("cardexpmonth")
                        .HasColumnName("cardexpmonth")
                        .HasMaxLength(255);

                    b.Property<string>("cardexpyear")
                        .HasColumnName("cardexpyear")
                        .HasMaxLength(255);

                    b.Property<string>("cardlast4")
                        .HasColumnName("cardlast4")
                        .HasMaxLength(255);

                    b.Property<string>("cardtype")
                        .HasColumnName("cardtype")
                        .HasMaxLength(255);

                    b.Property<string>("countrycode")
                        .HasColumnName("countrycode")
                        .HasMaxLength(255);

                    b.Property<string>("customercode")
                        .HasColumnName("customercode")
                        .HasMaxLength(255);

                    b.Property<string>("customerid")
                        .HasColumnName("customerid")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("transaction");
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Draws", b =>
                {
                    b.HasOne("OFTENCOFTAPI.Models.Items", "Item")
                        .WithMany("Draws")
                        .HasForeignKey("Itemid")
                        .HasConstraintName("draws_itemid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Items", b =>
                {
                    b.HasOne("OFTENCOFTAPI.Models.Itemcategories", "Category")
                        .WithMany("Items")
                        .HasForeignKey("Categoryid")
                        .HasConstraintName("item_catid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OFTENCOFTAPI.Models.Tickets", b =>
                {
                    b.HasOne("OFTENCOFTAPI.Models.Draws", "Draw")
                        .WithMany("Tickets")
                        .HasForeignKey("Drawid")
                        .HasConstraintName("ticket_drawid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OFTENCOFTAPI.Models.Transaction", "Transaction")
                        .WithMany("Tickets")
                        .HasForeignKey("transactionid")
                        .HasConstraintName("ticket_transactionid")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
