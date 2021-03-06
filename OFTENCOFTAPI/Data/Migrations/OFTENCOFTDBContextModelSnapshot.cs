﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OFTENCOFTAPI.Data.Models;

namespace OFTENCOFTAPI.Data.Migrations
{
    [DbContext(typeof(OFTENCOFTDBContext))]
    partial class OFTENCOFTDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Draws", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("Datecreated");

                    b.Property<DateTime?>("Datemodified");

                    b.Property<int?>("DrawType");

                    b.Property<DateTime?>("Drawdate");

                    b.Property<int?>("Drawstatus");

                    b.Property<int>("Itemid");

                    b.Property<int>("drawwinners");

                    b.Property<int>("noofwinners");

                    b.HasKey("Id");

                    b.HasIndex("Itemid");

                    b.ToTable("Draws");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Itemcategories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Categorydescription");

                    b.Property<string>("Categoryname");

                    b.Property<DateTime?>("Datecreated");

                    b.Property<DateTime?>("Datemodified");

                    b.HasKey("Id");

                    b.ToTable("Itemcategories");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Items", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Categoryid");

                    b.Property<DateTime?>("Datecreated");

                    b.Property<DateTime?>("Datemodified");

                    b.Property<string>("Itemdescription");

                    b.Property<string>("Itemname");

                    b.Property<decimal?>("Ticketamount");

                    b.HasKey("Id");

                    b.HasIndex("Categoryid");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.ResetToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Token");

                    b.Property<DateTime>("TokenExpiry");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ResetTokens");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Tickets", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessCode");

                    b.Property<int?>("Claimstatus");

                    b.Property<int?>("ConfirmStatus");

                    b.Property<DateTime?>("Datemodified");

                    b.Property<int?>("Drawid");

                    b.Property<string>("Emailaddress");

                    b.Property<string>("Firstname");

                    b.Property<string>("Lastname");

                    b.Property<string>("PaystackReference");

                    b.Property<string>("Phonenumber");

                    b.Property<string>("Ticketreference");

                    b.Property<int?>("Winstatus");

                    b.Property<int?>("transactionid");

                    b.HasKey("Id");

                    b.HasIndex("Drawid");

                    b.HasIndex("transactionid");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Bank");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Email");

                    b.Property<string>("IPAddress");

                    b.Property<string>("Location");

                    b.Property<DateTime?>("Paymentdate");

                    b.Property<string>("Pspaymentreference");

                    b.Property<int?>("Quantity");

                    b.Property<string>("TicketReferences");

                    b.Property<decimal?>("Totalamount");

                    b.Property<int?>("TransactionStatus");

                    b.Property<string>("cardchannel");

                    b.Property<string>("cardexpmonth");

                    b.Property<string>("cardexpyear");

                    b.Property<string>("cardlast4");

                    b.Property<string>("cardtype");

                    b.Property<string>("countrycode");

                    b.Property<string>("customercode");

                    b.Property<string>("customerid");

                    b.HasKey("Id");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.User.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Draws", b =>
                {
                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.Items", "Item")
                        .WithMany("Draws")
                        .HasForeignKey("Itemid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Items", b =>
                {
                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.Itemcategories", "Category")
                        .WithMany("Items")
                        .HasForeignKey("Categoryid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OFTENCOFTAPI.ApplicationCore.Models.Tickets", b =>
                {
                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.Draws", "Draw")
                        .WithMany("Tickets")
                        .HasForeignKey("Drawid");

                    b.HasOne("OFTENCOFTAPI.ApplicationCore.Models.Transaction", "Transaction")
                        .WithMany("Tickets")
                        .HasForeignKey("transactionid");
                });
#pragma warning restore 612, 618
        }
    }
}
