// ============================================
// INFRASTRUCTURE LAYER - Database Context
// ============================================
// This is the bridge between our domain entities and the database.
// Entity Framework Core uses this to:
// 1. Create database tables from entities
// 2. Track changes to entities
// 3. Execute queries and commands
//
// NOTE: This uses InMemory database for development.
// For production, switch to SQL Server by changing the connection string.

using Microsoft.EntityFrameworkCore;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext - the gateway to the database.
/// Each DbSet represents a table in the database.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tables in our database
    public DbSet<User> Users => Set<User>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    /// <summary>
    /// Configures the database schema and relationships.
    /// Called once when the context is created.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // USER CONFIGURATION
        // ========================================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            
            // Email must be unique (for login)
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // ========================================
        // SERVICE CONFIGURATION
        // ========================================
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2); // 10 digits, 2 decimal places
            
            // Index for category filtering (performance optimization)
            entity.HasIndex(e => e.Category);
        });

        // ========================================
        // BARBER CONFIGURATION
        // ========================================
        modelBuilder.Entity<Barber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Rating).HasPrecision(3, 2); // 0.00 to 5.00
        });

        // ========================================
        // APPOINTMENT CONFIGURATION
        // ========================================
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Relationships
            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            entity.HasOne(e => e.Barber)
                .WithMany()
                .HasForeignKey(e => e.BarberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PERFORMANCE: Composite index for conflict checking
            // This makes HasConflictAsync queries very fast
            entity.HasIndex(e => new { e.BarberId, e.AppointmentDate, e.StartTime });
        });

        // ========================================
        // CONTACT MESSAGE CONFIGURATION
        // ========================================
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
        });

        // ========================================
        // SEED DATA - Initial data for testing
        // ========================================
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Populates the database with sample data.
    /// This runs automatically when the database is created.
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed an admin user (password: "Admin123!")
        // In production, create users through the registration API
        var adminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            FullName = "Admin User",
            Email = "admin@sharpcuts.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Seed a test customer (password: "Customer123!")
        var customerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = customerId,
            FullName = "John Doe",
            Email = "customer@sharpcuts.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
            PhoneNumber = "+1234567890",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // Seed services
        var serviceIds = Enumerable.Range(1, 8).Select(i => 
            Guid.Parse($"11111111-1111-1111-1111-11111111111{i}")).ToArray();

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = serviceIds[0], Name = "Classic Haircut", Description = "Traditional haircut with precision cutting", DurationMinutes = 30, Price = 25, Category = ServiceCategory.Hair, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[1], Name = "Beard Trim & Shape", Description = "Expert beard trimming and shaping", DurationMinutes = 20, Price = 15, Category = ServiceCategory.Beard, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[2], Name = "Hot Towel Shave", Description = "Luxurious hot towel straight razor shave", DurationMinutes = 45, Price = 35, Category = ServiceCategory.Shave, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[3], Name = "Hair & Beard Combo", Description = "Complete haircut and beard package", DurationMinutes = 50, Price = 35, Category = ServiceCategory.Combo, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[4], Name = "Kids Haircut", Description = "Fun haircut for children under 12", DurationMinutes = 20, Price = 18, Category = ServiceCategory.Hair, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[5], Name = "Hair Coloring", Description = "Professional hair coloring", DurationMinutes = 60, Price = 55, Category = ServiceCategory.Hair, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[6], Name = "Scalp Treatment", Description = "Relaxing scalp massage and treatment", DurationMinutes = 30, Price = 30, Category = ServiceCategory.Treatment, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[7], Name = "Premium Package", Description = "Haircut, beard, shave, and massage", DurationMinutes = 90, Price = 75, Category = ServiceCategory.Combo, IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        // Seed barbers
        var barberIds = Enumerable.Range(1, 5).Select(i => 
            Guid.Parse($"22222222-2222-2222-2222-22222222222{i}")).ToArray();

        modelBuilder.Entity<Barber>().HasData(
            new Barber { Id = barberIds[0], FullName = "James Wilson", Avatar = "👨‍🦱", Specialty = "Classic Cuts & Fades", Rating = 4.9m, ReviewCount = 234, ExperienceYears = 8, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[1], FullName = "Marcus Johnson", Avatar = "👨‍🦲", Specialty = "Beard Styling & Design", Rating = 4.8m, ReviewCount = 189, ExperienceYears = 6, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[2], FullName = "David Chen", Avatar = "👨", Specialty = "Modern Styles & Color", Rating = 4.7m, ReviewCount = 156, ExperienceYears = 5, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[3], FullName = "Alex Rivera", Avatar = "🧔", Specialty = "Hot Towel Shaves", Rating = 4.9m, ReviewCount = 312, ExperienceYears = 12, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[4], FullName = "Ryan Thompson", Avatar = "👱‍♂️", Specialty = "Kids & Teen Cuts", Rating = 4.6m, ReviewCount = 98, ExperienceYears = 3, IsAvailable = true, CreatedAt = DateTime.UtcNow }
        );
    }
}
