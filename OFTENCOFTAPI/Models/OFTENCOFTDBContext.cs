using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace OFTENCOFTAPI.Models
{
    public partial class OFTENCOFTDBContext : DbContext
    {
        public OFTENCOFTDBContext()
        {
        }

        public OFTENCOFTDBContext(DbContextOptions<OFTENCOFTDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Draws> Draws { get; set; }
        public virtual DbSet<Itemcategories> Itemcategories { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Tickets> Tickets { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<Transaction> Logs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)   
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=OFTENCOFTDB;Username=postgres;Password=salvador");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Draws>(entity =>
            {
                entity.ToTable("draws");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                                    //.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
                    .ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);


                entity.Property(e => e.noofwinners)
                 .HasColumnName("noofwinners");

                entity.Property(e => e.drawwinners)
                .HasColumnName("drawwinners");

                entity.Property(e => e.Datecreated).HasColumnName("datecreated");

                entity.Property(e => e.Datemodified).HasColumnName("datemodified");

                entity.Property(e => e.Drawdate).HasColumnName("drawdate");

                entity.Property(e => e.Drawstatus)
                    .HasColumnName("drawstatus").HasColumnType("varchar(50)")
                    .HasMaxLength(255);

                entity.Property(e => e.Itemid).HasColumnName("itemid");

                entity.Property(e => e.DrawType)
                    .HasColumnName("drawtype").HasColumnType("varchar(50)")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Draws)
                    .HasForeignKey(d => d.Itemid)
                    .HasConstraintName("draws_itemid");
            });


            modelBuilder.Entity<Itemcategories>(entity =>
            {              
                entity.ToTable("itemcategories");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                                    //.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
                                    .ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);



                entity.Property(e => e.Categorydescription)
                    .HasColumnName("categorydescription")
                    .HasMaxLength(255);

                entity.Property(e => e.Categoryname)
                    .HasColumnName("categoryname")
                    .HasMaxLength(255);

                entity.Property(e => e.Datecreated).HasColumnName("datecreated");

                entity.Property(e => e.Datemodified).HasColumnName("datemodified");
            });

            modelBuilder.Entity<Items>(entity =>
            {
                entity.ToTable("items");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    //.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
                .ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                entity.Property(e => e.Categoryid).HasColumnName("categoryid");

                entity.Property(e => e.Datecreated).HasColumnName("datecreated");

                entity.Property(e => e.Datemodified).HasColumnName("datemodified");

                entity.Property(e => e.Itemname)
                    .HasColumnName("itemname")
                    .HasMaxLength(255);

                entity.Property(e => e.Ticketamount)
                    .HasColumnName("ticketamount")
                    .HasColumnType("decimal(20,2)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.Categoryid)
                    .HasConstraintName("item_catid");
            });

            modelBuilder.Entity<Tickets>(entity =>
            {
                entity.ToTable("tickets");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                                    //.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
                                    .ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);



                entity.Property(e => e.Claimstatus)
                    .HasColumnName("claimstatus").HasColumnType("varchar(50)")
                    .HasMaxLength(255);

                entity.Property(e => e.Datemodified).HasColumnName("datemodified");

                entity.Property(e => e.Drawid).HasColumnName("drawid");

                entity.Property(e => e.Emailaddress)
                    .HasColumnName("emailaddress")
                    .HasMaxLength(255);

                entity.Property(e => e.Firstname)
                    .HasColumnName("firstname")
                    .HasMaxLength(255);

                entity.Property(e => e.Lastname)
                    .HasColumnName("lastname")
                    .HasMaxLength(255);
               

                entity.Property(e => e.Phonenumber)
                    .HasColumnName("phonenumber")
                    .HasMaxLength(255);

                entity.Property(e => e.Ticketreference)
                    .HasColumnName("ticketreference")
                    .HasMaxLength(255);

                entity.Property(e => e.AccessCode)
                   .HasColumnName("accesscode")
                   .HasMaxLength(255); 
                
                entity.Property(e => e.PaystackReference)
                    .HasColumnName("paystackreference")
                    .HasMaxLength(255);

                entity.Property(e => e.Winstatus)
                    .HasColumnName("winstatus").HasColumnType("varchar(50)")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.ConfirmStatus)
            .HasColumnName("confirmstatus").HasColumnType("varchar(50)")
            .HasMaxLength(50);

                entity.HasOne(d => d.Draw)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.Drawid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("ticket_drawid");

                entity.HasOne(d => d.Transaction)
                   .WithMany(p => p.Tickets)
                   .HasForeignKey(d => d.transactionid)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("ticket_transactionid");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transaction");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                                    //.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
                                    .ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);


                entity.Property(e => e.Paymentdate).HasColumnName("paymentdate");

                entity.Property(e => e.Pspaymentreference)
                    .HasColumnName("pspaymentreference")
                    .HasMaxLength(255);

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Totalamount)
                    .HasColumnName("totalamount")
                    .HasColumnType("numeric(255,0)");
                //new

                entity.Property(e => e.customercode)
                    .HasColumnName("customercode")
                    .HasMaxLength(255);

                entity.Property(e => e.customerid)
                 .HasColumnName("customerid")
                 .HasMaxLength(255);

                entity.Property(e => e.cardlast4)
                 .HasColumnName("cardlast4")
                 .HasMaxLength(255);

                entity.Property(e => e.cardchannel)
                 .HasColumnName("cardchannel")
                 .HasMaxLength(255);

                entity.Property(e => e.cardtype)
                .HasColumnName("cardtype")
                .HasMaxLength(255);

                entity.Property(e => e.cardexpmonth)
                .HasColumnName("cardexpmonth")
                .HasMaxLength(255);

                entity.Property(e => e.cardexpyear)
                 .HasColumnName("cardexpyear")
                 .HasMaxLength(255);

                entity.Property(e => e.Bank)
                .HasColumnName("bank")
                .HasMaxLength(255);

                entity.Property(e => e.IPAddress)
             .HasColumnName("ipaddress")
             .HasMaxLength(255);

                entity.Property(e => e.Location)
             .HasColumnName("customerlocation")
             .HasMaxLength(255);

                entity.Property(e => e.countrycode)
              .HasColumnName("countrycode")
              .HasMaxLength(255);

                entity.Property(e => e.Email)
              .HasColumnName("email")
              .HasMaxLength(255);

                entity.Property(e => e.TransactionStatus)
             .HasColumnName("transactionstatus").HasColumnType("varchar(50)")
             .HasMaxLength(50);

                entity.Property(e => e.Pspaymentreference)
              .HasColumnName("paystackreference")
              .HasMaxLength(255);

               entity.Property(e => e.DateModified).HasColumnName("datemodified");
                //endnew
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.ToTable("logs");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                //.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);
                .ValueGeneratedOnAdd().HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                entity.Property(e => e.Application)
                    .HasColumnName("application").HasColumnType("varchar");

                entity.Property(e => e.Logged).HasColumnName("logged");

                entity.Property(e => e.Level)
                    .HasColumnName("level").HasColumnType("varchar");

                entity.Property(e => e.Message)
                 .HasColumnName("message").HasColumnType("varchar");

                entity.Property(e => e.Logger)
                 .HasColumnName("logger").HasColumnType("varchar");

                entity.Property(e => e.Callsite)
                 .HasColumnName("callsite").HasColumnType("varchar");

                entity.Property(e => e.Exception)
                .HasColumnName("exception").HasColumnType("varchar");  
            });
        }
    }
}
