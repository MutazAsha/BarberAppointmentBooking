// ============================================
// INFRASTRUCTURE LAYER - Dependency Injection Registration
// ============================================
// This is where we register all services with the DI container.
// The API layer calls this during startup to wire everything up.
//
// WHY THIS PATTERN?
// - Keeps Program.cs clean and focused
// - Makes dependencies explicit
// - Easy to swap implementations (e.g., InMemory → SQL Server)

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpCuts.Application.Interfaces;
using SharpCuts.Application.Services;
using SharpCuts.Infrastructure.Data;
using SharpCuts.Infrastructure.Repositories;

namespace SharpCuts.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services.
/// Called from Program.cs during application startup.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure services with the DI container.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ========================================
        // DATABASE CONTEXT
        // ========================================
        // Use InMemory database for development
        // For production, switch to SQL Server:
        // services.AddDbContext<AppDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("SharpCutsDb"));

        // ========================================
        // REPOSITORIES (Data Access)
        // ========================================
        // Scoped = one instance per HTTP request
        // This ensures each request gets a fresh DbContext
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IBarberRepository, BarberRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();

        // ========================================
        // APPLICATION SERVICES (Business Logic)
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
