// ============================================
// INFRASTRUCTURE LAYER - Database Context
// ============================================
// سياق قاعدة البيانات - Entity Framework Core

using Microsoft.EntityFrameworkCore;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Infrastructure.Data;

/// <summary>
/// سياق قاعدة البيانات - البوابة للبيانات
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // جداول قاعدة البيانات
    public DbSet<User> Users => Set<User>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    /// <summary>
    /// إعداد هيكل قاعدة البيانات والعلاقات
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // إعداد المستخدم
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // إعداد الخدمة
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasIndex(e => e.Category);
        });

        // إعداد الحلاق
        modelBuilder.Entity<Barber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Rating).HasPrecision(3, 2);
        });

        // إعداد الموعد
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Barber)
                .WithMany()
                .HasForeignKey(e => e.BarberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // فهرس مركب للتحقق من التعارض
            entity.HasIndex(e => new { e.BarberId, e.AppointmentDate, e.StartTime });
        });

        // إعداد رسالة الاتصال
        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
        });

        // بيانات أولية
        SeedData(modelBuilder);
    }

    /// <summary>
    /// إضافة بيانات أولية للاختبار
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // مدير (كلمة المرور: Admin123!)
        var adminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            FullName = "المدير",
            Email = "admin@sharpcuts.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // عميل (كلمة المرور: Customer123!)
        var customerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = customerId,
            FullName = "أحمد محمد",
            Email = "customer@sharpcuts.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
            PhoneNumber = "+1234567890",
            Role = UserRole.Customer,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        // خدمات
        var serviceIds = Enumerable.Range(1, 8).Select(i => 
            Guid.Parse($"11111111-1111-1111-1111-11111111111{i}")).ToArray();

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = serviceIds[0], Name = "قصة شعر كلاسيكية", Description = "قصة شعر تقليدية بدقة عالية", DurationMinutes = 30, Price = 25, Category = ServiceCategory.Hair, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[1], Name = "تهذيب اللحية", Description = "تهذيب وتشكيل اللحية", DurationMinutes = 20, Price = 15, Category = ServiceCategory.Beard, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[2], Name = "حلاقة بالمنشفة الساخنة", Description = "حلاقة فاخرة بالمنشفة الساخنة", DurationMinutes = 45, Price = 35, Category = ServiceCategory.Shave, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[3], Name = "باقة الشعر واللحية", Description = "قصة شعر وتهذيب لحية", DurationMinutes = 50, Price = 35, Category = ServiceCategory.Combo, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[4], Name = "قصة شعر أطفال", Description = "قصة شعر ممتعة للأطفال", DurationMinutes = 20, Price = 18, Category = ServiceCategory.Hair, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[5], Name = "صبغة شعر", Description = "صبغة شعر احترافية", DurationMinutes = 60, Price = 55, Category = ServiceCategory.Hair, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[6], Name = "علاج فروة الرأس", Description = "تدليك وعلاج فروة الرأس", DurationMinutes = 30, Price = 30, Category = ServiceCategory.Treatment, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Service { Id = serviceIds[7], Name = "الباقة المميزة", Description = "قصة شعر، لحية، حلاقة، وتدليك", DurationMinutes = 90, Price = 75, Category = ServiceCategory.Combo, IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        // حلاقين
        var barberIds = Enumerable.Range(1, 5).Select(i => 
            Guid.Parse($"22222222-2222-2222-2222-22222222222{i}")).ToArray();

        modelBuilder.Entity<Barber>().HasData(
            new Barber { Id = barberIds[0], FullName = "James Wilson", Avatar = "👨‍🦱", Specialty = "قصات كلاسيكية", Rating = 4.9m, ReviewCount = 234, ExperienceYears = 8, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[1], FullName = "Marcus Johnson", Avatar = "👨‍🦲", Specialty = "تصميم اللحية", Rating = 4.8m, ReviewCount = 189, ExperienceYears = 6, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[2], FullName = "David Chen", Avatar = "👨", Specialty = "الصبغات", Rating = 4.7m, ReviewCount = 156, ExperienceYears = 5, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[3], FullName = "Alex Rivera", Avatar = "🧔", Specialty = "الحلاقة الفاخرة", Rating = 4.9m, ReviewCount = 312, ExperienceYears = 12, IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new Barber { Id = barberIds[4], FullName = "Ryan Thompson", Avatar = "👱‍♂️", Specialty = "قصات الأطفال", Rating = 4.6m, ReviewCount = 98, ExperienceYears = 3, IsAvailable = true, CreatedAt = DateTime.UtcNow }
        );
    }
}
