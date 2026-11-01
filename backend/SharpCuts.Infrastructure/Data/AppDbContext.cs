namespace SharpCuts.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using SharpCuts.Domain.Entities;

/// <summary>
/// Entity Framework DbContext - Database configuration and entity mappings
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Service> Services => Set<Service>();
    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Service Configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasIndex(e => e.Category);
        });

        // Barber Configuration
        modelBuilder.Entity<Barber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
        });

        // Appointment Configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(20);

            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Barber)
                .WithMany()
                .HasForeignKey(e => e.BarberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.BarberId, e.AppointmentDate, e.AppointmentTime })
                .IsUnique()
                .HasFilter("Status != 3"); // Exclude cancelled
        });

        // TimeSlot Configuration
        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.BarberId, e.Date, e.Time }).IsUnique();
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var serviceId1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var serviceId2 = Guid.Parse("00000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = serviceId1, Name = "Classic Haircut", Description = "Traditional haircut", DurationMinutes = 30, Price = 25, Category = "Hair" },
            new Service { Id = serviceId2, Name = "Beard Trim", Description = "Beard shaping", DurationMinutes = 20, Price = 15, Category = "Beard" }
        );
    }
}
