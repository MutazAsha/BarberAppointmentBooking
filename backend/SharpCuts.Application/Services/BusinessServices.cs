// ============================================
// APPLICATION LAYER - Business Services
// ============================================
// These services implement the core business logic.
// They depend on repository interfaces (not implementations).
// This allows easy testing and swapping data sources.

using Microsoft.Extensions.Logging;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Application.Services;

/// <summary>
/// Service catalog management.
/// Handles listing and filtering services.
/// </summary>
public class ServiceService : IServiceService
{
    private readonly IServiceRepository _repository;
    private readonly ILogger<ServiceService> _logger;

    public ServiceService(IServiceRepository repository, ILogger<ServiceService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ServiceDto>> GetAllAsync()
    {
        _logger.LogDebug("Fetching all active services");
        var services = await _repository.GetActiveAsync();
        return services.Select(MapToDto).ToList();
    }

    public async Task<ServiceDto?> GetByIdAsync(Guid id)
    {
        var service = await _repository.GetByIdAsync(id);
        return service == null ? null : MapToDto(service);
    }

    public async Task<IReadOnlyList<ServiceDto>> GetByCategoryAsync(string category)
    {
        if (Enum.TryParse<ServiceCategory>(category, true, out var cat))
        {
            var services = await _repository.GetByCategoryAsync(cat);
            return services.Select(MapToDto).ToList();
        }
        
        // If "All" or invalid category, return all
        return await GetAllAsync();
    }

    /// <summary>
    /// Maps domain entity to DTO.
    /// Separation prevents leaking internal structure to clients.
    /// </summary>
    private static ServiceDto MapToDto(Service s) => new(
        s.Id, s.Name, s.Description, s.DurationMinutes, 
        s.Price, s.Category.ToString(), s.IsActive
    );
}

/// <summary>
/// Barber management service.
/// </summary>
public class BarberService : IBarberService
{
    private readonly IBarberRepository _repository;
    private readonly ILogger<BarberService> _logger;

    public BarberService(IBarberRepository repository, ILogger<BarberService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<BarberDto>> GetAllAsync()
    {
        var barbers = await _repository.GetAllAsync();
        return barbers.Select(MapToDto).ToList();
    }

    public async Task<BarberDto?> GetByIdAsync(Guid id)
    {
        var barber = await _repository.GetByIdAsync(id);
        return barber == null ? null : MapToDto(barber);
    }

    public async Task<IReadOnlyList<BarberDto>> GetAvailableAsync()
    {
        var barbers = await _repository.GetAvailableAsync();
        return barbers.Select(MapToDto).ToList();
    }

    private static BarberDto MapToDto(Barber b) => new(
        b.Id, b.FullName, b.Avatar, b.Specialty,
        b.Rating, b.ReviewCount, b.ExperienceYears, b.IsAvailable
    );
}

/// <summary>
/// Core booking service - handles appointment creation with conflict detection.
/// 
/// KEY BUSINESS RULES:
/// - A barber cannot have overlapping appointments
/// - Appointments must be in the future
/// - Service and barber must exist and be active
/// </summary>
public class BookingService : IBookingService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly IBarberRepository _barberRepo;
    private readonly IUserRepository _userRepo;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IAppointmentRepository appointmentRepo,
        IServiceRepository serviceRepo,
        IBarberRepository barberRepo,
        IUserRepository userRepo,
        ILogger<BookingService> logger)
    {
        _appointmentRepo = appointmentRepo;
        _serviceRepo = serviceRepo;
        _barberRepo = barberRepo;
        _userRepo = userRepo;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new appointment with full validation.
    /// 
    /// VALIDATION STEPS:
    /// 1. Verify service exists and is active
    /// 2. Verify barber exists and is available
    /// 3. Verify customer exists
    /// 4. Check for scheduling conflicts
    /// 5. Create appointment
    /// </summary>
    public async Task<AppointmentDto> CreateAsync(Guid customerId, CreateAppointmentRequest request)
    {
        _logger.LogInformation("Creating appointment for customer {CustomerId}", customerId);

        // Step 1: Validate service
        var service = await _serviceRepo.GetByIdAsync(request.ServiceId)
            ?? throw new InvalidOperationException("Service not found");
        
        if (!service.IsActive)
            throw new InvalidOperationException("Service is not available");

        // Step 2: Validate barber
        var barber = await _barberRepo.GetByIdAsync(request.BarberId)
            ?? throw new InvalidOperationException("Barber not found");
        
        if (!barber.IsAvailable)
            throw new InvalidOperationException("Barber is not available");

        // Step 3: Validate customer
        var customer = await _userRepo.GetByIdAsync(customerId)
            ?? throw new InvalidOperationException("Customer not found");

        // Step 4: CRITICAL - Check for scheduling conflicts
        // This prevents double-booking the same barber at the same time
        if (await _appointmentRepo.HasConflictAsync(request.BarberId, request.AppointmentDate, request.StartTime))
        {
            _logger.LogWarning("Booking conflict detected for barber {BarberId} at {Time}", 
                request.BarberId, request.StartTime);
            throw new InvalidOperationException("This time slot is already booked");
        }

        // Step 5: Create appointment
        var appointment = new Appointment
        {
            ServiceId = request.ServiceId,
            Service = service,
            BarberId = request.BarberId,
            Barber = barber,
            CustomerId = customerId,
            Customer = customer,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            Status = AppointmentStatus.Confirmed, // Auto-confirm for now
            Notes = request.Notes
        };

        await _appointmentRepo.AddAsync(appointment);

        _logger.LogInformation("Appointment created: {AppointmentId}", appointment.Id);

        return MapToDto(appointment);
    }

    /// <summary>
    /// Cancels an appointment.
    /// Only the customer who booked can cancel.
    /// </summary>
    public async Task<bool> CancelAsync(Guid appointmentId, Guid customerId)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId)
            ?? throw new InvalidOperationException("Appointment not found");

        // SECURITY: Only the customer who booked can cancel
        if (appointment.CustomerId != customerId)
        {
            _logger.LogWarning("Unauthorized cancel attempt by {CustomerId} on appointment {AppointmentId}", 
                customerId, appointmentId);
            throw new UnauthorizedAccessException("You can only cancel your own appointments");
        }

        // BUSINESS RULE: Check if cancellation is allowed
        if (!appointment.CanBeCancelled())
            throw new InvalidOperationException("This appointment cannot be cancelled");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _appointmentRepo.UpdateAsync(appointment);

        _logger.LogInformation("Appointment cancelled: {AppointmentId}", appointmentId);
        return true;
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetByCustomerAsync(Guid customerId)
    {
        var appointments = await _appointmentRepo.GetByCustomerAsync(customerId);
        return appointments.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Generates available time slots for a barber on a specific date.
    /// 
    /// PERFORMANCE: Checks existing appointments to determine availability.
    /// Slots that are already booked or in the past are marked unavailable.
    /// </summary>
    public async Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(Guid barberId, DateTime date)
    {
        var appointments = await _appointmentRepo.GetByBarberAsync(barberId);
        var slots = new List<TimeSlotDto>();

        // Generate slots from 9 AM to 6 PM, every 30 minutes
        for (int hour = 9; hour <= 17; hour++)
        {
            foreach (var minute in new[] { "00", "30" })
            {
                var time = $"{hour:D2}:{minute}";
                
                // Check if this slot is already booked
                var isBooked = appointments.Any(a => 
                    a.AppointmentDate.Date == date.Date && 
                    a.StartTime == time && 
                    a.Status != AppointmentStatus.Cancelled);

                // Check if slot is in the past
                var slotDateTime = date.Date.AddHours(hour).AddMinutes(minute == "30" ? 30 : 0);
                var isPast = slotDateTime < DateTime.UtcNow;

                slots.Add(new TimeSlotDto(
                    Time: time,
                    IsAvailable: !isBooked && !isPast,
                    Reason: isBooked ? "Already booked" : isPast ? "Time has passed" : null
                ));
            }
        }

        return slots;
    }

    private static AppointmentDto MapToDto(Appointment a) => new(
        a.Id,
        a.Service?.Name ?? "Unknown",
        a.Service?.Price ?? 0,
        a.Service?.DurationMinutes ?? 0,
        a.Barber?.FullName ?? "Unknown",
        a.Customer?.FullName ?? "Unknown",
        a.Customer?.Email ?? "",
        a.AppointmentDate,
        a.StartTime,
        a.Status.ToString(),
        a.Notes,
        a.CreatedAt
    );
}

/// <summary>
/// Dashboard analytics service.
/// Aggregates data for admin reporting.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(IAppointmentRepository appointmentRepo, ILogger<DashboardService> logger)
    {
        _appointmentRepo = appointmentRepo;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var allAppointments = await _appointmentRepo.GetAllAsync();
        var today = DateTime.Today;

        var todayAppts = allAppointments.Where(a => a.AppointmentDate.Date == today).ToList();
        
        return new DashboardStatsDto(
            TotalAppointments: allAppointments.Count,
            TodayAppointments: todayAppts.Count(a => a.Status != AppointmentStatus.Cancelled),
            PendingAppointments: allAppointments.Count(a => a.Status == AppointmentStatus.Pending),
            ConfirmedAppointments: allAppointments.Count(a => a.Status == AppointmentStatus.Confirmed),
            CompletedAppointments: allAppointments.Count(a => a.Status == AppointmentStatus.Completed),
            CancelledAppointments: allAppointments.Count(a => a.Status == AppointmentStatus.Cancelled),
            TotalRevenue: allAppointments
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .Sum(a => a.Service?.Price ?? 0),
            TodayRevenue: todayAppts
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .Sum(a => a.Service?.Price ?? 0)
        );
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetTodayScheduleAsync()
    {
        var appointments = await _appointmentRepo.GetByDateAsync(DateTime.Today);
        return appointments
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.StartTime)
            .Select(a => new AppointmentDto(
                a.Id,
                a.Service?.Name ?? "",
                a.Service?.Price ?? 0,
                a.Service?.DurationMinutes ?? 0,
                a.Barber?.FullName ?? "",
                a.Customer?.FullName ?? "",
                a.Customer?.Email ?? "",
                a.AppointmentDate,
                a.StartTime,
                a.Status.ToString(),
                a.Notes,
                a.CreatedAt
            ))
            .ToList();
    }
}

/// <summary>
/// Contact form submission service.
/// </summary>
public class ContactService : IContactService
{
    private readonly IContactRepository _repository;
    private readonly ILogger<ContactService> _logger;

    public ContactService(IContactRepository repository, ILogger<ContactService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> SubmitAsync(ContactRequest request)
    {
        var message = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message
        };

        await _repository.AddAsync(message);
        _logger.LogInformation("Contact message received from: {Email}", request.Email);
        return true;
    }
}
