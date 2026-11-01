using Microsoft.EntityFrameworkCore;
using SharpCuts.Domain.Entities;

namespace SharpCuts.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Service> Services => Set<Service>();
    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Classic Haircut", Description = "Traditional haircut with precision cutting and styling", Duration = 30, Price = 25, Category = "Hair", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Beard Trim & Shape", Description = "Expert beard trimming, shaping, and conditioning", Duration = 20, Price = 15, Category = "Beard", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Hot Towel Shave", Description = "Luxurious hot towel straight razor shave experience", Duration = 45, Price = 35, Category = "Shave", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Hair & Beard Combo", Description = "Complete haircut and beard grooming package", Duration = 50, Price = 35, Category = "Combo", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Kids Haircut", Description = "Fun and friendly haircut for children under 12", Duration = 20, Price = 18, Category = "Hair", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Hair Coloring", Description = "Professional hair coloring with premium products", Duration = 60, Price = 55, Category = "Hair", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Name = "Scalp Treatment", Description = "Relaxing scalp massage and treatment for healthy hair", Duration = 30, Price = 30, Category = "Treatment", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Name = "Premium Package", Description = "Haircut, beard trim, hot towel shave, and scalp massage", Duration = 90, Price = 75, Category = "Combo", IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Barber>().HasData(
            new Barber { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "James Wilson", Avatar = "👨‍🦱", Specialty = "Classic Cuts & Fades", Rating = 4.9m, Reviews = 234, Experience = 8, Available = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Marcus Johnson", Avatar = "👨‍🦲", Specialty = "Beard Styling & Design", Rating = 4.8m, Reviews = 189, Experience = 6, Available = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "David Chen", Avatar = "👨", Specialty = "Modern Styles & Color", Rating = 4.7m, Reviews = 156, Experience = 5, Available = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Alex Rivera", Avatar = "🧔", Specialty = "Hot Towel Shaves", Rating = 4.9m, Reviews = 312, Experience = 12, Available = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Ryan Thompson", Avatar = "👱‍♂️", Specialty = "Kids & Teen Cuts", Rating = 4.6m, Reviews = 98, Experience = 3, Available = true, CreatedAt = DateTime.UtcNow }
        );
    }
}
