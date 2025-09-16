using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExamAPI.Models
{
    public partial class ExamSQLContext : DbContext
    {
        public ExamSQLContext()
        {
        }

        public ExamSQLContext(DbContextOptions<ExamSQLContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bom> Boms { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=ExamSQL;User Id=exam;Password=exam;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bom>(entity =>
            {
                entity.HasKey(e => e.SerialNo);

                entity.ToTable("Bom");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Creator).HasMaxLength(50);

                entity.Property(e => e.MaterialNo).HasMaxLength(50);

                entity.Property(e => e.MaterialUseQuantity).HasDefaultValueSql("((1))");

                entity.Property(e => e.Modifier).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ProductNo).HasMaxLength(50);

                entity.HasOne(d => d.MaterialNoNavigation)
                    .WithMany(p => p.Boms)
                    .HasForeignKey(d => d.MaterialNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bom_Material");

                entity.HasOne(d => d.ProductNoNavigation)
                    .WithMany(p => p.Boms)
                    .HasForeignKey(d => d.ProductNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bom_Product");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasKey(e => e.MaterialNo);

                entity.ToTable("Material");

                entity.Property(e => e.MaterialNo).HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Creator).HasMaxLength(50);

                entity.Property(e => e.MaterialCost).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.MaterialName).HasMaxLength(50);

                entity.Property(e => e.Modifier).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderNo);

                entity.ToTable("Order");

                entity.Property(e => e.OrderNo).HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Creator).HasMaxLength(50);

                entity.Property(e => e.Modifier).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderApplicant).HasMaxLength(50);

                entity.Property(e => e.OrderSubject).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.SerialNo);

                entity.ToTable("OrderDetail");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Creator).HasMaxLength(50);

                entity.Property(e => e.Modifier).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OrderNo).HasMaxLength(50);

                entity.Property(e => e.ProductNo).HasMaxLength(50);

                entity.HasOne(d => d.OrderNoNavigation)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductNo);

                entity.ToTable("Product");

                entity.Property(e => e.ProductNo).HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Creator).HasMaxLength(50);

                entity.Property(e => e.Modifier).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ProductName).HasMaxLength(50);

                entity.Property(e => e.ProductPrice).HasColumnType("decimal(12, 2)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.SerialNo);

                entity.ToTable("User");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Creator).HasMaxLength(50);

                entity.Property(e => e.Modifier).HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserAccount).HasMaxLength(50);

                entity.Property(e => e.UserEmail).HasMaxLength(100);

                entity.Property(e => e.UserName).HasMaxLength(50);

                entity.Property(e => e.UserPassword).HasMaxLength(50);

                entity.Property(e => e.UserRole)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'customer')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
