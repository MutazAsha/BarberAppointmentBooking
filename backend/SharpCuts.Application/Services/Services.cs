using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Interfaces;

namespace SharpCuts.Application.Services;

public class BookingService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly IBarberRepository _barberRepo;

    public BookingService(IAppointmentRepository appointmentRepo, IServiceRepository serviceRepo, IBarberRepository barberRepo)
    {
        _appointmentRepo = appointmentRepo;
        _serviceRepo = serviceRepo;
        _barberRepo = barberRepo;
    }

    public async Task<Appointment> CreateBookingAsync(Guid serviceId, Guid barberId, string date, string time, string customerName, string customerPhone, string customerEmail, string? notes)
    {
        var service = await _serviceRepo.GetByIdAsync(serviceId) ?? throw new Exception("Service not found");
        var barber = await _barberRepo.GetByIdAsync(barberId) ?? throw new Exception("Barber not found");
        if (!barber.Available) throw new Exception("Barber is not available");

        var hasConflict = await _appointmentRepo.HasConflictAsync(barberId, date, time);
        if (hasConflict) throw new Exception("This time slot is already booked");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ServiceId = serviceId,
            ServiceName = service.Name,
            BarberId = barberId,
            BarberName = barber.Name,
            Date = date,
            Time = time,
            Duration = service.Duration,
            Price = service.Price,
            Status = "confirmed",
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            CustomerEmail = customerEmail,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        return await _appointmentRepo.AddAsync(appointment);
    }

    public async Task<bool> CancelBookingAsync(Guid id)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(id) ?? throw new Exception("Appointment not found");
        appointment.Status = "cancelled";
        await _appointmentRepo.UpdateAsync(appointment);
        return true;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync() => await _appointmentRepo.GetAllAsync();
    public async Task<IEnumerable<Appointment>> GetAppointmentsByEmailAsync(string email) => await _appointmentRepo.GetByCustomerEmailAsync(email);
}

public class BarberService
{
    private readonly IBarberRepository _barberRepo;
    public BarberService(IBarberRepository barberRepo) => _barberRepo = barberRepo;

    public async Task<IEnumerable<Barber>> GetAllBarbersAsync() => await _barberRepo.GetAllAsync();
    public async Task<IEnumerable<Barber>> GetAvailableBarbersAsync() => await _barberRepo.GetAvailableBarbersAsync();
    public async Task<Barber?> GetBarberByIdAsync(Guid id) => await _barberRepo.GetByIdAsync(id);
    public async Task<Barber?> ToggleAvailabilityAsync(Guid id)
    {
        var barber = await _barberRepo.GetByIdAsync(id);
        if (barber == null) return null;
        barber.Available = !barber.Available;
        await _barberRepo.UpdateAsync(barber);
        return barber;
    }
}

public class ServiceManagementService
{
    private readonly IServiceRepository _serviceRepo;
    public ServiceManagementService(IServiceRepository serviceRepo) => _serviceRepo = serviceRepo;

    public async Task<IEnumerable<Service>> GetAllServicesAsync() => await _serviceRepo.GetActiveServicesAsync();
    public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category) => await _serviceRepo.GetByCategoryAsync(category);
    public async Task<Service?> GetServiceByIdAsync(Guid id) => await _serviceRepo.GetByIdAsync(id);
}

public class ContactService
{
    private readonly IContactRepository _contactRepo;
    public ContactService(IContactRepository contactRepo) => _contactRepo = contactRepo;

    public async Task<ContactMessage> SubmitMessageAsync(string name, string email, string subject, string message)
    {
        var contactMessage = new ContactMessage
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Subject = subject,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
        return await _contactRepo.AddAsync(contactMessage);
    }

    public async Task<IEnumerable<ContactMessage>> GetAllMessagesAsync() => await _contactRepo.GetAllAsync();
}

public class DashboardService
{
    private readonly IAppointmentRepository _appointmentRepo;
    public DashboardService(IAppointmentRepository appointmentRepo) => _appointmentRepo = appointmentRepo;

    public async Task<object> GetStatsAsync()
    {
        var appointments = await _appointmentRepo.GetAllAsync();
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        return new
        {
            TotalAppointments = appointments.Count(),
            TodayAppointments = appointments.Count(a => a.Date == today && a.Status != "cancelled"),
            PendingAppointments = appointments.Count(a => a.Status == "pending"),
            ConfirmedAppointments = appointments.Count(a => a.Status == "confirmed"),
            CompletedAppointments = appointments.Count(a => a.Status == "completed"),
            CancelledAppointments = appointments.Count(a => a.Status == "cancelled"),
            TotalRevenue = appointments.Where(a => a.Status != "cancelled").Sum(a => a.Price)
        };
    }
}
