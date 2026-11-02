// ============================================
// DOMAIN LAYER - Core Entities
// ============================================
// These are the fundamental business objects.
// Each entity represents a real-world concept in our barber shop domain.
//
// DESIGN PRINCIPLES:
// - Entities are independent of any framework (no EF Core attributes here)
// - They contain business validation logic
// - They use Guid for IDs (globally unique, secure)

using SharpCuts.Domain.Enums;

namespace SharpCuts.Domain.Entities;

/// <summary>
/// Base entity class providing common properties.
/// All entities inherit from this to ensure consistency.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Globally unique identifier (secure, non-sequential)</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Audit: When this record was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Audit: When this record was last modified</summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Represents a user account in the system.
/// Handles authentication (login) and authorization (roles).
/// 
/// SECURITY NOTES:
/// - Password is stored as a BCrypt hash (never plain text)
/// - Email must be unique across the system
/// - Role determines what the user can access
/// </summary>
public class User : BaseEntity
{
    /// <summary>Full name of the user</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Unique email address (used for login)</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt hashed password.
    /// NEVER store plain text passwords!
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Phone number for appointment reminders</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>User's role - determines permissions</summary>
    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>Whether the account is active (soft delete support)</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>When the user last logged in</summary>
    public DateTime? LastLoginAt { get; set; }

    // ========================================
    // BUSINESS RULES (Domain Logic)
    // ========================================

    /// <summary>
    /// Validates that the email format is correct.
    /// Business rule: All users must have a valid email.
    /// </summary>
    public bool IsValidEmail()
    {
        if (string.IsNullOrWhiteSpace(Email)) return false;
        
        try
        {
            var addr = new System.Net.Mail.MailAddress(Email);
            return addr.Address == Email.Trim();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if user has admin privileges.
    /// Used for authorization checks throughout the app.
    /// </summary>
    public bool IsAdmin() => Role == UserRole.Admin;
}

/// <summary>
/// Represents a grooming service offered by the barber shop.
/// Examples: Haircut, Beard Trim, Hot Towel Shave.
/// </summary>
public class Service : BaseEntity
{
    /// <summary>Display name (e.g., "Classic Haircut")</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Detailed description for customers</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>How long the service takes (in minutes)</summary>
    public int DurationMinutes { get; set; }

    /// <summary>Price in USD</summary>
    public decimal Price { get; set; }

    /// <summary>Category for filtering (Hair, Beard, etc.)</summary>
    public ServiceCategory Category { get; set; }

    /// <summary>Whether this service is currently offered</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Business rule: Price must be positive.
    /// Prevents invalid pricing data.
    /// </summary>
    public bool HasValidPrice() => Price > 0;

    /// <summary>
    /// Business rule: Duration must be reasonable.
    /// Prevents data entry errors.
    /// </summary>
    public bool HasValidDuration() => DurationMinutes > 0 && DurationMinutes <= 240;
}

/// <summary>
/// Represents a barber/stylist working at the shop.
/// Each barber has their own schedule and specialties.
/// </summary>
public class Barber : BaseEntity
{
    /// <summary>Barber's full name</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Emoji/avatar for UI display</summary>
    public string Avatar { get; set; } = string.Empty;

    /// <summary>Area of expertise (e.g., "Classic Cuts & Fades")</summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>Customer rating (0.0 to 5.0)</summary>
    public decimal Rating { get; set; }

    /// <summary>Total number of customer reviews</summary>
    public int ReviewCount { get; set; }

    /// <summary>Years of professional experience</summary>
    public int ExperienceYears { get; set; }

    /// <summary>Whether currently accepting appointments</summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>Optional link to user account (for barbers who log in)</summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Business rule: Rating must be between 0 and 5.
    /// </summary>
    public bool HasValidRating() => Rating >= 0 && Rating <= 5;
}

/// <summary>
/// Represents a booked appointment between a customer and barber.
/// This is the core transaction entity of the system.
/// 
/// BUSINESS RULES:
/// - A barber cannot have overlapping appointments
/// - Appointments must be in the future when created
/// - Status transitions follow a specific workflow
/// </summary>
public class Appointment : BaseEntity
{
    /// <summary>The service being performed</summary>
    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }

    /// <summary>The barber performing the service</summary>
    public Guid BarberId { get; set; }
    public Barber? Barber { get; set; }

    /// <summary>The customer who booked (linked to User)</summary>
    public Guid CustomerId { get; set; }
    public User? Customer { get; set; }

    /// <summary>Appointment date (YYYY-MM-DD)</summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>Start time (HH:mm format)</summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>Current status in the appointment lifecycle</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    /// <summary>Optional notes from the customer</summary>
    public string? Notes { get; set; }

    /// <summary>Calculated end time based on service duration</summary>
    public DateTime EndTime => AppointmentDate.Add(TimeSpan.Parse(StartTime))
        .AddMinutes(Service?.DurationMinutes ?? 0);

    /// <summary>
    /// Business rule: Check if appointment is in the past.
    /// Used to prevent editing past appointments.
    /// </summary>
    public bool IsInPast() => EndTime < DateTime.UtcNow;

    /// <summary>
    /// Business rule: Check if appointment can be cancelled.
    /// Only pending or confirmed appointments can be cancelled.
    /// </summary>
    public bool CanBeCancelled() => 
        Status == AppointmentStatus.Pending || 
        Status == AppointmentStatus.Conirmed;

    /// <summary>
    /// Business rule: Check for time conflict with another appointment.
    /// Two appointments conflict if they overlap in time for the same barber.
    /// </summary>
    public bool ConflictsWith(Appointment other)
    {
        if (BarberId != other.BarberId) return false;
        if (Id == other.Id) return false; // Same appointment

        return StartTime == other.StartTime && 
               AppointmentDate.Date == other.AppointmentDate.Date;
    }
}

/// <summary>
/// Stores contact form messages from website visitors.
/// Not linked to user accounts (anyone can submit).
/// </summary>
public class ContactMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
}
