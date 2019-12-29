using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Process_Guard_Admin.Models
{
    public partial class processguarddbContext : DbContext
    {
        public virtual DbSet<Colors> Colors { get; set; }
        public virtual DbSet<ProcessList> ProcessList { get; set; }

        public processguarddbContext(DbContextOptions<processguarddbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Colors>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("colors");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(30)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ProcessList>(entity =>
            {
                entity.HasKey(e => new { e.Exe, e.Filename });

                entity.ToTable("process_list");

                entity.HasIndex(e => e.Color)
                    .HasName("color");

                entity.Property(e => e.Exe)
                    .HasColumnName("exe")
                    .HasMaxLength(60);

                entity.Property(e => e.Filename)
                    .HasColumnName("filename")
                    .HasMaxLength(200);

                entity.Property(e => e.Color)
                    .HasColumnName("color")
                    .HasMaxLength(30);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(150);

                entity.HasOne(d => d.ColorNavigation)
                    .WithMany(p => p.ProcessList)
                    .HasForeignKey(d => d.Color)
                    .HasConstraintName("process_list_ibfk_1");
            });
        }
    }
}
