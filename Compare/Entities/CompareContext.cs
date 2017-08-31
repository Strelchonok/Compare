using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Compare.Entities
{
    public partial class CompareContext : DbContext
    {
        public CompareContext(DbContextOptions<CompareContext> options) : base(options)
        { }

            public virtual DbSet<Products> Products { get; set; }
            public virtual DbSet<Vendors> Vendors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK_Products");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnType("varchar(max)");

                entity.HasOne(d => d.VendorRef)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.VendorRefId)
                    .HasConstraintName("FK_Products_Vendors");
            });

            modelBuilder.Entity<Vendors>(entity =>
            {
                entity.HasKey(e => e.VendorId)
                    .HasName("PK_Vendors");

                entity.Property(e => e.VendorName)
                    .IsRequired()
                    .HasColumnType("varchar(max)");
            });

        }
    }
}