// ============================================
// APPLICATION LAYER - Business Services
// ============================================
// خدمات التطبيق - تحتوي على منطق العمل

using Microsoft.Extensions.Logging;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;
using SharpCuts.Domain.Exceptions;

namespace SharpCuts.Application.Services;

/// <summary>
/// خدمة إدارة الخدمات
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
        return await GetAllAsync();
    }

    private static ServiceDto MapToDto(Service s) => new(
        s.Id, s.Name, s.Description, s.DurationMinutes, 
        s.Price, s.Category.ToString(), s.IsActive
    );
}

/// <summary>
/// خدمة إدارة الحلاقين
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
/// خدمة الحجز - منطق العمل الأساسي للمواعيد
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
    /// إنشاء موعد جديد مع التحقق الكامل
    /// </summary>
    public async Task<AppointmentDto> CreateAsync(Guid customerId, CreateAppointmentRequest request)
    {
        _logger.LogInformation("إنشاء موعد للعميل {CustomerId}", customerId);

        // التحقق من الخدمة
        var service = await _serviceRepo.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException("الخدمة", request.ServiceId);
        
        if (!service.IsActive)
            throw new BusinessRuleException("الخدمة غير متاحة");

        // التحقق من الحلاق
        var barber = await _barberRepo.GetByIdAsync(request.BarberId)
            ?? throw new NotFoundException("الحلاق", request.BarberId);
        
        if (!barber.IsAvailable)
            throw new BusinessRuleException("الحلاق غير متاح");

        // التحقق من العميل
        var customer = await _userRepo.GetByIdAsync(customerId)
            ?? throw new NotFoundException("العميل", customerId);

        // التحقق من التعارض
        if (await _appointmentRepo.HasConflictAsync(request.BarberId, request.AppointmentDate, request.StartTime))
        {
            throw new ConflictException("هذا الوقت محجوز مسبقاً");
        }

        // إنشاء الموعد
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
            Status = AppointmentStatus.Confirmed,
            Notes = request.Notes
        };

        await _appointmentRepo.AddAsync(appointment);
        _logger.LogInformation("تم إنشاء الموعد: {AppointmentId}", appointment.Id);

        return MapToDto(appointment);
    }

    /// <summary>
    /// إلغاء موعد
    /// </summary>
    public async Task<bool> CancelAsync(Guid appointmentId, Guid customerId)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId)
            ?? throw new NotFoundException("الموعد", appointmentId);

        // التحقق من الملكية
        if (appointment.CustomerId != customerId)
            throw new ForbiddenException("لا يمكنك إلغاء مواعيد الآخرين");

        if (!appointment.CanBeCancelled())
            throw new BusinessRuleException("لا يمكن إلغاء هذا الموعد");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;
        await _appointmentRepo.UpdateAsync(appointment);

        _logger.LogInformation("تم إلغاء الموعد: {AppointmentId}", appointmentId);
        return true;
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetByCustomerAsync(Guid customerId)
    {
        var appointments = await _appointmentRepo.GetByCustomerAsync(customerId);
        return appointments.Select(MapToDto).ToList();
    }

    /// <summary>
    /// الحصول على الفترات الزمنية المتاحة
    /// </summary>
    public async Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(Guid barberId, DateTime date)
    {
        var appointments = await _appointmentRepo.GetByBarberAsync(barberId);
        var slots = new List<TimeSlotDto>();

        for (int hour = 9; hour <= 17; hour++)
        {
            foreach (var minute in new[] { "00", "30" })
            {
                var time = $"{hour:D2}:{minute}";
                
                var isBooked = appointments.Any(a => 
                    a.AppointmentDate.Date == date.Date && 
                    a.StartTime == time && 
                    a.Status != AppointmentStatus.Cancelled);

                var slotDateTime = date.Date.AddHours(hour).AddMinutes(minute == "30" ? 30 : 0);
                var isPast = slotDateTime < DateTime.UtcNow;

                slots.Add(new TimeSlotDto(
                    Time: time,
                    IsAvailable: !isBooked && !isPast,
                    Reason: isBooked ? "محجوز" : isPast ? "انتهى الوقت" : null
                ));
            }
        }

        return slots;
    }

    private static AppointmentDto MapToDto(Appointment a) => new(
        a.Id,
        a.Service?.Name ?? "غير معروف",
        a.Service?.Price ?? 0,
        a.Service?.DurationMinutes ?? 0,
        a.Barber?.FullName ?? "غير معروف",
        a.Customer?.FullName ?? "غير معروف",
        a.Customer?.Email ?? "",
        a.AppointmentDate,
        a.StartTime,
        a.Status.ToString(),
        a.Notes,
        a.CreatedAt
    );
}

/// <summary>
/// خدمة لوحة التحكم
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
/// خدمة الاتصال
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
        _logger.LogInformation("تم استلام رسالة من: {Email}", request.Email);
        return true;
    }
}
