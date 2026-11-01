namespace SharpCuts.Domain.Interfaces;

// Service Layer Interfaces (Business Logic)
public interface IBookingService
{
    Task<BookingResult> CreateBookingAsync(CreateBookingRequest request);
    Task<bool> CancelBookingAsync(Guid appointmentId);
    Task<IEnumerable<AppointmentDto>> GetCustomerAppointmentsAsync(string email);
}

public interface IBarberService
{
    Task<IEnumerable<BarberDto>> GetAllBarbersAsync();
    Task<IEnumerable<BarberDto>> GetAvailableBarbersAsync();
    Task<BarberDto?> GetBarberByIdAsync(Guid id);
}

public interface IServiceManagementService
{
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync();
    Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(string category);
    Task<ServiceDto?> GetServiceByIdAsync(Guid id);
}

// DTOs
public record BarberDto(
    Guid Id,
    string Name,
    string Avatar,
    string Specialty,
    decimal Rating,
    int ReviewCount,
    int ExperienceYears,
    bool IsAvailable
);

public record ServiceDto(
    Guid Id,
    string Name,
    string Description,
    int DurationMinutes,
    decimal Price,
    string Category
);

public record AppointmentDto(
    Guid Id,
    string ServiceName,
    string BarberName,
    DateTime AppointmentDate,
    string AppointmentTime,
    string Status,
    string CustomerName,
    string CustomerPhone,
    string CustomerEmail,
    decimal Price,
    int DurationMinutes
);

// Request/Response Models
public record CreateBookingRequest(
    Guid ServiceId,
    Guid BarberId,
    DateTime Date,
    string Time,
    string CustomerName,
    string CustomerPhone,
    string CustomerEmail,
    string? Notes
);

public record BookingResult(
    bool Success,
    Guid? AppointmentId,
    string? ErrorMessage
);
