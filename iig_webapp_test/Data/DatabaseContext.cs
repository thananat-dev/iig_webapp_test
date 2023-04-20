using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using iig_webapp_test.Entities;

namespace iig_webapp_test.Data
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChangeUserPassword> ChangeUserPasswords { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=iig_pgsel_db;User Id=Administrator;Password=P@ssw0rd;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChangeUserPassword>(entity =>
            {
                entity.ToTable("ChangeUserPassword");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.LastUpdate).HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).UseIdentityAlwaysColumn();

                entity.Property(e => e.FirstName).HasMaxLength(60);

                entity.Property(e => e.LastName).HasMaxLength(60);

                entity.Property(e => e.Username).HasMaxLength(12);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
