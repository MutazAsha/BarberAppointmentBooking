// ============================================
// APPLICATION LAYER - Service Interfaces
// ============================================
// واجهات خدمات التطبيق - تحدد عمليات العمل

using SharpCuts.Application.DTOs;

namespace SharpCuts.Application.Interfaces;

/// <summary>
/// واجهة خدمة المصادقة - تسجيل الدخول والتسجيل
/// </summary>
public interface IAuthService
{
    /// <summary>تسجيل مستخدم جديد</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>تسجيل الدخول وإرجاع JWT</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>التحقق من صحة JWT</summary>
    Task<bool> ValidateTokenAsync(string token);
}

/// <summary>
/// واجهة خدمة إدارة الخدمات
/// </summary>
public interface IServiceService
{
    Task<IReadOnlyList<ServiceDto>> GetAllAsync();
    Task<ServiceDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ServiceDto>> GetByCategoryAsync(string category);
}

/// <summary>
/// واجهة خدمة إدارة الحلاقين
/// </summary>
public interface IBarberService
{
    Task<IReadOnlyList<BarberDto>> GetAllAsync();
    Task<BarberDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<BarberDto>> GetAvailableAsync();
}

/// <summary>
/// واجهة خدمة الحجز - منطق العمل الأساسي للمواعيد
/// </summary>
public interface IBookingService
{
    /// <summary>إنشاء موعد جديد (مع التحقق من التعارض)</summary>
    Task<AppointmentDto> CreateAsync(Guid customerId, CreateAppointmentRequest request);

    /// <summary>إلغاء موعد</summary>
    Task<bool> CancelAsync(Guid appointmentId, Guid customerId);

    /// <summary>الحصول على مواعيد العميل</summary>
    Task<IReadOnlyList<AppointmentDto>> GetByCustomerAsync(Guid customerId);

    /// <summary>الحصول على الفترات الزمنية المتاحة</summary>
    Task<IReadOnlyList<TimeSlotDto>> GetAvailableSlotsAsync(Guid barberId, DateTime date);
}

/// <summary>
/// واجهة خدمة لوحة التحكم
/// </summary>
public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
    Task<IReadOnlyList<AppointmentDto>> GetTodayScheduleAsync();
}

/// <summary>
/// واجهة خدمة الاتصال
/// </summary>
public interface IContactService
{
    Task<bool> SubmitAsync(ContactRequest request);
}
