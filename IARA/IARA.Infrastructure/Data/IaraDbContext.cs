using Microsoft.EntityFrameworkCore;
using IARA.Domain.Entities;
using IARA.Domain.Enums;

namespace IARA.Infrastructure.Data;

public class IaraDbContext : DbContext
{
    public IaraDbContext(DbContextOptions<IaraDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Ship> Ships { get; set; }
    public DbSet<Permit> Permits { get; set; }
    public DbSet<FishingTrip> FishingTrips { get; set; }
    public DbSet<Catch> Catches { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Inspection> Inspections { get; set; }
    public DbSet<Fine> Fines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasConversion<string>();
            
            entity.HasMany(e => e.Ships)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Tickets)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Inspections)
                .WithOne(e => e.Inspector)
                .HasForeignKey(e => e.InspectorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ship>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RegistrationNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.RegistrationNumber).IsUnique();
            entity.Property(e => e.EnginePower).HasPrecision(10, 2);
            entity.Property(e => e.FuelType).HasConversion<string>();
            
            entity.HasMany(e => e.Permits)
                .WithOne(e => e.Ship)
                .HasForeignKey(e => e.ShipId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.FishingTrips)
                .WithOne(e => e.Ship)
                .HasForeignKey(e => e.ShipId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Permit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AllowedGear).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.ValidTo);
        });

        modelBuilder.Entity<FishingTrip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FuelUsed).HasPrecision(10, 2);
            
            entity.HasMany(e => e.Catches)
                .WithOne(e => e.FishingTrip)
                .HasForeignKey(e => e.FishingTripId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.EndTime);
        });

        modelBuilder.Entity<Catch>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FishType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.QuantityKg).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TicketType).HasConversion<string>();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasIndex(e => e.ValidTo);
        });

        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TargetType).HasConversion<string>();
            entity.Property(e => e.Result).HasConversion<string>();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Fine)
                .WithOne(e => e.Inspection)
                .HasForeignKey<Fine>(e => e.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasIndex(e => e.InspectionDate);
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@iara.bg",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.ADMIN,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                FullName = "Ivan Petrov",
                Email = "inspector@iara.bg",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Inspector123!"),
                Role = UserRole.INSPECTOR,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 3,
                FullName = "Georgi Dimitrov",
                Email = "fisher@iara.bg",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Fisher123!"),
                Role = UserRole.PROFESSIONAL_FISHER,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 4,
                FullName = "Maria Ivanova",
                Email = "recreational@iara.bg",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Fisher123!"),
                Role = UserRole.RECREATIONAL_FISHER,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Ship>().HasData(
            new Ship
            {
                Id = 1,
                Name = "Sea Hunter",
                RegistrationNumber = "VAR-001",
                OwnerId = 3,
                EnginePower = 450.00m,
                FuelType = FuelType.DIESEL,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Ship
            {
                Id = 2,
                Name = "Ocean Explorer",
                RegistrationNumber = "BUR-002",
                OwnerId = 3,
                EnginePower = 320.00m,
                FuelType = FuelType.DIESEL,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Permit>().HasData(
            new Permit
            {
                Id = 1,
                ShipId = 1,
                ValidFrom = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidTo = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                AllowedGear = "Trawl, Gillnet, Longline",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Permit
            {
                Id = 2,
                ShipId = 2,
                ValidFrom = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidTo = new DateTime(2025, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                AllowedGear = "Hook and Line, Gillnet",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Ticket>().HasData(
            new Ticket
            {
                Id = 1,
                UserId = 4,
                ValidFrom = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidTo = new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                TicketType = TicketType.YEARLY,
                Price = 120.00m,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}