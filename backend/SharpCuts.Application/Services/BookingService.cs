namespace SharpCuts.Application.Services;

using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Interfaces;

/// <summary>
/// Booking Service - Core business logic for appointment booking
/// Implements IBookingService with dependency injection of repositories
/// </summary>
public class BookingService : IBookingService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IBarberRepository _barberRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly ILogger<BookingService> _logger;

    // Constructor Injection - all dependencies injected via DI container
    public BookingService(
        IAppointmentRepository appointmentRepository,
        IServiceRepository serviceRepository,
        IBarberRepository barberRepository,
        ITimeSlotRepository timeSlotRepository,
        ILogger<BookingService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _barberRepository = barberRepository;
        _timeSlotRepository = timeSlotRepository;
        _logger = logger;
    }

    public async Task<BookingResult> CreateBookingAsync(CreateBookingRequest request)
    {
        try
        {
            // Validate service exists
            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                return new BookingResult(false, null, "Service not found");

            // Validate barber exists and is available
            var barber = await _barberRepository.GetByIdAsync(request.BarberId);
            if (barber == null || !barber.IsAvailable)
                return new BookingResult(false, null, "Barber not available");

            // Check for scheduling conflicts
            var hasConflict = await _appointmentRepository.HasConflictAsync(
                request.BarberId, request.Date, request.Time);
            
            if (hasConflict)
                return new BookingResult(false, null, "Time slot already booked");

            // Create appointment
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                ServiceId = request.ServiceId,
                BarberId = request.BarberId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                Notes = request.Notes,
                AppointmentDate = request.Date,
                AppointmentTime = request.Time,
                Status = AppointmentStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment);

            _logger.LogInformation(
                "Booking created: {AppointmentId} for {Customer} with {Barber} on {Date} at {Time}",
                appointment.Id, request.CustomerName, barber.Name, request.Date, request.Time);

            return new BookingResult(true, appointment.Id, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for {Customer}", request.CustomerEmail);
            return new BookingResult(false, null, "An error occurred while creating the booking");
        }
    }

    public async Task<bool> CancelBookingAsync(Guid appointmentId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null) return false;

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _appointmentRepository.UpdateAsync(appointment);

        _logger.LogInformation("Appointment {Id} cancelled", appointmentId);
        return true;
    }

    public async Task<IEnumerable<AppointmentDto>> GetCustomerAppointmentsAsync(string email)
    {
        var appointments = await _appointmentRepository.GetByCustomerEmailAsync(email);
        
        return appointments.Select(a => new AppointmentDto(
            a.Id,
            a.Service?.Name ?? "Unknown",
            a.Barber?.Name ?? "Unknown",
            a.AppointmentDate,
            a.AppointmentTime,
            a.Status.ToString(),
            a.CustomerName,
            a.CustomerPhone,
            a.CustomerEmail,
            a.Service?.Price ?? 0,
            a.Service?.DurationMinutes ?? 0
        ));
    }
}
