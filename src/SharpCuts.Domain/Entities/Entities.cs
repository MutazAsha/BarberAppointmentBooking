// ============================================
// DOMAIN LAYER - Entities
// ============================================
// الكيانات الأساسية تمثل الكائنات الحقيقية في مجال العمل

using SharpCuts.Domain.Common;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Domain.Entities;

/// <summary>
/// كيان المستخدم - يمثل حساب المستخدم في النظام
/// </summary>
public class User : BaseEntity
{
    /// <summary>الاسم الكامل للمستخدم</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>البريد الإلكتروني (فريد)</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>كلمة المرور المشفرة (BCrypt)</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>رقم الهاتف (اختياري)</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>دور المستخدم (عميل، حلاق، مدير)</summary>
    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>هل الحساب مفعل؟</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>تاريخ آخر تسجيل دخول</summary>
    public DateTime? LastLoginAt { get; set; }

    // ========================================
    // Domain Rules (قواعد النطاق)
    // ========================================

    /// <summary>
    /// التحقق من صحة البريد الإلكتروني
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
    /// التحقق مما إذا كان المستخدم مدير
    /// </summary>
    public bool IsAdmin() => Role == UserRole.Admin;
}

/// <summary>
/// كيان الخدمة - يمثل خدمة يقدمها صالون الحلاقة
/// </summary>
public class Service : BaseEntity
{
    /// <summary>اسم الخدمة (مثل: قصة شعر كلاسيكية)</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>وصف تفصيلي للخدمة</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>مدة الخدمة بالدقائق</summary>
    public int DurationMinutes { get; set; }

    /// <summary>السعر بالدولار</summary>
    public decimal Price { get; set; }

    /// <summary>فئة الخدمة (شعر، لحية، إلخ)</summary>
    public ServiceCategory Category { get; set; }

    /// <summary>هل الخدمة متاحة حالياً؟</summary>
    public bool IsActive { get; set; } = true;

    // ========================================
    // Domain Rules
    // ========================================

    /// <summary>
    /// التحقق من صحة السعر (يجب أن يكون موجباً)
    /// </summary>
    public bool HasValidPrice() => Price > 0;

    /// <summary>
    /// التحقق من صحة المدة (يجب أن تكون معقولة)
    /// </summary>
    public bool HasValidDuration() => DurationMinutes > 0 && DurationMinutes <= 240;
}

/// <summary>
/// كيان الحلاق - يمثل الحلاق/المصفف
/// </summary>
public class Barber : BaseEntity
{
    /// <summary>الاسم الكامل للحلاق</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>الصورة الرمزية (Emoji)</summary>
    public string Avatar { get; set; } = string.Empty;

    /// <summary>التخصص (مثل: قصات كلاسيكية)</summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>التقييم (0.0 إلى 5.0)</summary>
    public decimal Rating { get; set; }

    /// <summary>عدد التقييمات</summary>
    public int ReviewCount { get; set; }

    /// <summary>سنوات الخبرة</summary>
    public int ExperienceYears { get; set; }

    /// <summary>هل الحلاق متاح حالياً؟</summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>رابط اختياري لحساب المستخدم (للحلاقين الذين يسجلون دخول)</summary>
    public Guid? UserId { get; set; }

    // ========================================
    // Domain Rules
    // ========================================

    /// <summary>
    /// التحقق من صحة التقييم (يجب أن يكون بين 0 و 5)
    /// </summary>
    public bool HasValidRating() => Rating >= 0 && Rating <= 5;
}

/// <summary>
/// كيان الموعد - يمثل موعد محجوز بين العميل والحلاق
/// </summary>
public class Appointment : BaseEntity
{
    /// <summary>معرّف الخدمة</summary>
    public Guid ServiceId { get; set; }

    /// <summary>الخدمة (علاقة تنقل)</summary>
    public Service? Service { get; set; }

    /// <summary>معرّف الحلاق</summary>
    public Guid BarberId { get; set; }

    /// <summary>الحلاق (علاقة تنقل)</summary>
    public Barber? Barber { get; set; }

    /// <summary>معرّف العميل</summary>
    public Guid CustomerId { get; set; }

    /// <summary>العميل (علاقة تنقل)</summary>
    public User? Customer { get; set; }

    /// <summary>تاريخ الموعد</summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>وقت البدء (HH:mm)</summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>حالة الموعد</summary>
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    /// <summary>ملاحظات من العميل</summary>
    public string? Notes { get; set; }

    // ========================================
    // Domain Rules
    // ========================================

    /// <summary>
    /// التحقق مما إذا كان الموعد في الماضي
    /// </summary>
    public bool IsInPast()
    {
        var appointmentDateTime = AppointmentDate.Date.Add(TimeSpan.Parse(StartTime));
        return appointmentDateTime < DateTime.UtcNow;
    }

    /// <summary>
    /// التحقق مما إذا كان يمكن إلغاء الموعد
    /// </summary>
    public bool CanBeCancelled() => 
        Status == AppointmentStatus.Pending || 
        Status == AppointmentStatus.Confirmed;

    /// <summary>
    /// التحقق من التعارض مع موعد آخر
    /// </summary>
    public bool ConflictsWith(Appointment other)
    {
        if (BarberId != other.BarberId) return false;
        if (Id == other.Id) return false;

        return StartTime == other.StartTime && 
               AppointmentDate.Date == other.AppointmentDate.Date;
    }
}

/// <summary>
/// كيان رسالة الاتصال - رسائل نموذج الاتصال
/// </summary>
public class ContactMessage : BaseEntity
{
    /// <summary>اسم المرسل</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>بريد المرسل</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>الموضوع</summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>نص الرسالة</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>هل تم قراءة الرسالة؟</summary>
    public bool IsRead { get; set; } = false;
}
