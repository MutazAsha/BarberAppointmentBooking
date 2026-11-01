// ============================================
// APPLICATION LAYER - Data Transfer Objects (DTOs)
// ============================================
// DTOs define the shape of data flowing in/out of the API.
// They are SEPARATE from domain entities to:
// 1. Control exactly what data is exposed
// 2. Prevent accidental data leaks (e.g., password hashes)
// 3. Allow API contracts to evolve independently
//
// Using 'record' types for immutability and value equality.

using SharpCuts.Domain.Enums;

namespace SharpCuts.Application.DTOs;

// ============================================
// AUTHENTICATION DTOs
// ============================================

/// <summary>
/// Request to register a new user account.
/// </summary>
public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    string? PhoneNumber = null
);

/// <summary>
/// Request to authenticate an existing user.
/// </summary>
public record LoginRequest(
    string Email,
    string Password
);

/// <summary>
/// Response after successful authentication.
/// Contains the JWT token needed for subsequent requests.
/// </summary>
public record AuthResponse(
    string Token,
    string FullName,
    string Email,
    string Role,
    DateTime ExpiresAt
);

// ============================================
// SERVICE DTOs
// ============================================

/// <summary>
/// Read-only view of a service (safe to expose to clients).
/// </summary>
public record ServiceDto(
    Guid Id,
    string Name,
    string Description,
    int DurationMinutes,
    decimal Price,
    string Category,
    bool IsActive
);

// ============================================
// BARBER DTOs
// ============================================

/// <summary>
/// Read-only view of a barber.
/// </summary>
public record BarberDto(
    Guid Id,
    string FullName,
    string Avatar,
    string Specialty,
    decimal Rating,
    int ReviewCount,
    int ExperienceYears,
    bool IsAvailable
);

// ============================================
// APPOINTMENT DTOs
// ============================================

/// <summary>
/// Request to create a new appointment.
/// </summary>
public record CreateAppointmentRequest(
    Guid ServiceId,
    Guid BarberId,
    DateTime AppointmentDate,
    string StartTime,
    string? Notes = null
);

/// <summary>
/// Read-only view of an appointment.
/// Includes related entity names for display.
/// </summary>
public record AppointmentDto(
    Guid Id,
    string ServiceName,
    decimal ServicePrice,
    int ServiceDuration,
    string BarberName,
    string CustomerName,
    string CustomerEmail,
    DateTime AppointmentDate,
    string StartTime,
    string Status,
    string? Notes,
    DateTime CreatedAt
);

/// <summary>
/// Available time slot for booking.
/// </summary>
public record TimeSlotDto(
    string Time,
    bool IsAvailable,
    string? Reason = null  // Why a slot is unavailable (if applicable)
);

// ============================================
// DASHBOARD DTOs
// ============================================

/// <summary>
/// Aggregated statistics for the admin dashboard.
/// </summary>
public record DashboardStatsDto(
    int TotalAppointments,
    int TodayAppointments,
    int PendingAppointments,
    int ConfirmedAppointments,
    int CompletedAppointments,
    int CancelledAppointments,
    decimal TotalRevenue,
    decimal TodayRevenue
);

// ============================================
// COMMON DTOs
// ============================================

/// <summary>
/// Standard API response wrapper.
/// Ensures consistent response format across all endpoints.
/// </summary>
public record ApiResponse<T>(
    bool Success,
    T? Data = default,
    string? Message = null,
    string? Error = null
);

/// <summary>
/// Paginated response for large data sets.
/// PERFORMANCE: Prevents loading all records at once.
/// </summary>
public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>
/// Contact form submission.
/// </summary>
public record ContactRequest(
    string Name,
    string Email,
    string Subject,
    string Message
);
