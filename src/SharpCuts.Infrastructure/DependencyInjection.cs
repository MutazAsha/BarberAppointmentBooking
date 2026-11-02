// ============================================
// INFRASTRUCTURE LAYER - Dependency Injection
// ============================================
// تسجيل جميع الخدمات مع حاوية DI

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpCuts.Application.Interfaces;
using SharpCuts.Application.Services;
using SharpCuts.Infrastructure.Data;
using SharpCuts.Infrastructure.Repositories;

namespace SharpCuts.Infrastructure;

/// <summary>
/// ملحق لتسجيل خدمات البنية التحتية
/// يُستدعى من Program.cs أثناء بدء التطبيق
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// تسجيل جميع خدمات البنية التحتية مع حاوية DI
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ========================================
        // سياق قاعدة البيانات
        // ========================================
        // استخدام قاعدة بيانات InMemory للتطوير
        // للإنتاج، استخدم SQL Server:
        // services.AddDbContext<AppDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("SharpCutsDb"));

        // ========================================
        // المستودعات (الوصول للبيانات)
        // ========================================
        // Scoped = نسخة واحدة لكل طلب HTTP
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IBarberRepository, BarberRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();

        // ========================================
        // خدمات التطبيق (منطق العمل)
        // ========================================
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IBarberService, BarberService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IContactService, ContactService>();

        return services;
    }
}
