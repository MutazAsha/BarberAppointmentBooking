// ============================================
// APPLICATION LAYER - Service Interfaces
// ============================================
// These define the business operations available.
// Controllers depend on these interfaces (not implementations).

using SharpCuts.Application.DTOs;

namespace SharpCuts.Application.Interfaces;

/// <summary>
/// Handles user authentication and authorization.
/// </summary>
public interface IAuthService
{
    /// <summary>Register a new user account</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>Authenticate user and return JWT token</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>Validate a JWT token</summary>
    Task<bool> ValidateTokenAsync(string token);
}

/// <summary>
/// Manages service catalog operations.
/// </summary>
public interface IServiceService
{
    Task<IReadOnlyList<ServiceDto>> GetAllAsync();
    Task<ServiceDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ServiceDto>> GetByCategoryAsync(string category);
}

/// <summary>
/// Manages barber operations.
/// </summary>
public interface IBarberService
{
    Task<IReadOnlyList<BarberDto>> GetAllAsync();
    Task<BarberDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<BarberDto>> GetAvailableAsync();
}

/// <summary>
/// Core booking business logic.
/// Handles appointment creation, cancellation, and availability.
/// </summary>
public interface IBookingService
{
    /// <summary>Create a new appointment (with conflict checking)</summary>
    Task<AppointmentDto> CreateAsync(Guid customerId, CreateAppointmentRequest request);

    /// <summary>Cancel an appointment</summary>
    Task<bool> CancelAsync(Guid appointmentId, Guid customerId);

    /// <summary>Get customer's appointments</summary>
    Task<IReadOnlyList<AppointmentDto>> GetByCustomerAsync(Guid customerId);

    /// <summary>Get available time slots for a barber on a date</summary>
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(Guid barberId, DateTime date);
}

/// <summary>
/// Dashboard analytics and reporting.
/// </summary>
public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
    Task<IReadOnlyList<AppointmentDto>> GetTodayScheduleAsync();
}

/// <summary>
/// Contact form handling.
/// </summary>
public interface IContactService
{
    Task<bool> SubmitAsync(ContactRequest request);
}
