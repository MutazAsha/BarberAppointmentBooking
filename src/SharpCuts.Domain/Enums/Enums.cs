// ============================================
// DOMAIN LAYER - Enums
// ============================================
// الأنواع المحددة المستخدمة في النظام

namespace SharpCuts.Domain.Enums;

/// <summary>
/// حالة الموعد - تتبع دورة حياة الموعد
/// </summary>
public enum AppointmentStatus
{
    /// <summary>في الانتظار - تم الإنشاء لكن لم يُؤكد</summary>
    Pending = 0,

    /// <summary>مؤكد - الحلاق وافق على الموعد</summary>
    Confirmed = 1,

    /// <summary>مكتمل - الخدمة تمت بنجاح</summary>
    Completed = 2,

    /// <summary>ملغي - العميل أو الحلاق ألغى الموعد</summary>
    Cancelled = 3
}

/// <summary>
/// أدوار المستخدمين - للتحكم في الصلاحيات
/// </summary>
public enum UserRole
{
    /// <summary>عميل عادي - يمكنه حجز المواعيد</summary>
    Customer = 0,

    /// <summary>حلاق - يمكنه رؤية جدوله</summary>
    Barber = 1,

    /// <summary>مدير - صلاحيات كاملة</summary>
    Admin = 2
}

/// <summary>
/// فئات الخدمات - لتصنيف الخدمات
/// </summary>
public enum ServiceCategory
{
    /// <summary>خدمات الشعر</summary>
    Hair,

    /// <summary>خدمات اللحية</summary>
    Beard,

    /// <summary>خدمات الحلاقة</summary>
    Shave,

    /// <summary>باقات مجمعة</summary>
    Combo,

    /// <summary>علاجات</summary>
    Treatment
}
