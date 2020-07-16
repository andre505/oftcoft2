using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OFTENCOFTAPI.ApplicationCore.Models;
using OFTENCOFTAPI.ApplicationCore.Models.User;

namespace OFTENCOFTAPI.Data.Models
{
    public partial class OFTENCOFTDBContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
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
        public virtual DbSet<ResetToken> ResetTokens { get; set; }
        
    }
}
