// ============================================
// APPLICATION LAYER - DTOs
// ============================================
// كائنات نقل البيانات - تحدد شكل البيانات المتدفقة من وإلى API
// منفصلة عن الكيانات للتحكم في البيانات المعروضة

using SharpCuts.Domain.Enums;

namespace SharpCuts.Application.DTOs;

// ============================================
// AUTHENTICATION DTOs
// ============================================

/// <summary>
/// طلب تسجيل مستخدم جديد
/// </summary>
public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    string? PhoneNumber = null
);

/// <summary>
/// طلب تسجيل الدخول
/// </summary>
public record LoginRequest(
    string Email,
    string Password
);

/// <summary>
/// استجابة المصادقة الناجحة
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
/// عرض الخدمة (للقراءة فقط)
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
/// عرض الحلاق (للقراءة فقط)
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
/// طلب إنشاء موعد جديد
/// </summary>
public record CreateAppointmentRequest(
    Guid ServiceId,
    Guid BarberId,
    DateTime AppointmentDate,
    string StartTime,
    string? Notes = null
);

/// <summary>
/// عرض الموعد (للقراءة فقط)
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
/// فترة زمنية متاحة للحجز
/// </summary>
public record TimeSlotDto(
    string Time,
    bool IsAvailable,
    string? Reason = null
);

// ============================================
// DASHBOARD DTOs
// ============================================

/// <summary>
/// إحصائيات لوحة التحكم
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
/// استجابة API موحدة
/// </summary>
public record ApiResponse<T>(
    bool Success,
    T? Data = default,
    string? Message = null,
    string? Error = null
);

/// <summary>
/// استجابة مع ترقيم الصفحات
/// </summary>
public record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

/// <summary>
/// طلب نموذج الاتصال
/// </summary>
public record ContactRequest(
    string Name,
    string Email,
    string Subject,
    string Message
);
