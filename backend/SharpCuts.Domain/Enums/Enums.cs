// ============================================
// DOMAIN LAYER - Enumeration Types
// ============================================
// Centralized enums for type safety across the application.
// Using enums prevents magic strings and makes code self-documenting.

namespace SharpCuts.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an appointment.
/// Used for filtering, business rules, and UI display.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>Booking created, awaiting confirmation</summary>
    Pending = 0,

    /// <summary>Barber has confirmed the appointment</summary>
    Confirmed = 1,

    /// <summary>Service was completed successfully</summary>
    Completed = 2,

    /// <summary>Customer or barber cancelled</summary>
    Cancelled = 3
}

/// <summary>
/// User roles for authorization.
/// Controls access to protected endpoints.
/// </summary>
public enum UserRole
{
    /// <summary>Regular customer - can book appointments</summary>
    Customer = 0,

    /// <summary>Barber - can view their schedule</summary>
    Barber = 1,

    /// <summary>Administrator - full system access</summary>
    Admin = 2
}

/// <summary>
/// Service categories for filtering and organization.
/// </summary>
public enum ServiceCategory
{
    Hair,
    Beard,
    Shave,
    Combo,
    Treatment
}
