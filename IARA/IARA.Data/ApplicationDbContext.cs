using Microsoft.EntityFrameworkCore;
using IARA.Domain.Entities;

namespace IARA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Store configuration
            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Manager).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Region).HasMaxLength(100);
                entity.Property(e => e.Territory).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
                entity.Property(e => e.Settlement).HasMaxLength(100);
                entity.HasIndex(e => e.Name);
            });

            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MonthlySalary).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Employees)
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SellingPrice).HasColumnType("decimal(18,2)");
                
                entity.HasIndex(e => e.Code).IsUnique();
                
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Sale configuration
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                
                entity.HasOne(s => s.Store)
                    .WithMany(st => st.Sales)
                    .HasForeignKey(s => s.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(s => s.Employee)
                    .WithMany(e => e.Sales)
                    .HasForeignKey(s => s.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasIndex(e => e.SaleDateTime);
            });

            // SaleItem configuration
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                
                entity.HasOne(si => si.Sale)
                    .WithMany(s => s.SaleItems)
                    .HasForeignKey(si => si.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(si => si.Product)
                    .WithMany(p => p.SaleItems)
                    .HasForeignKey(si => si.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}