using Microsoft.Extensions.DependencyInjection;
using SharpCuts.Application.Services;
using SharpCuts.Domain.Interfaces;
using SharpCuts.Infrastructure.Repositories;
using SharpCuts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SharpCuts.Infrastructure;

/// <summary>
/// Dependency Injection Configuration
/// Registers all services, repositories, and DbContext with the DI container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with connection string from configuration
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Register Repositories (Scoped - one per request)
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IBarberRepository, BarberRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();

        // Register Application Services (Scoped - one per request)
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IBarberService, BarberService>();
        services.AddScoped<IServiceManagementService, ServiceManagementService>();

        return services;
    }
}
