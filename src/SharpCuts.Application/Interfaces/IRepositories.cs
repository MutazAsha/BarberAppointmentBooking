// ============================================
// APPLICATION LAYER - Repository Interfaces
// ============================================
// واجهات المستودعات - تحدد عقود الوصول للبيانات
// Infrastructure Layer يوفر التطبيق

using SharpCuts.Domain.Common;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Application.Interfaces;

/// <summary>
/// واجهة المستودع العامة - عمليات CRUD الأساسية
/// </summary>
/// <typeparam name="T">نوع الكيان</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>الحصول على جميع السجلات</summary>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>الحصول على سجل بالمعرف</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>إنشاء سجل جديد</summary>
    Task<T> AddAsync(T entity);

    /// <summary>تحديث سجل موجود</summary>
    Task UpdateAsync(T entity);

    /// <summary>حذف سجل</summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>التحقق من وجود سجل</summary>
    Task<bool> ExistsAsync(Guid id);
}

/// <summary>
/// واجهة مستودع المستخدمين - عمليات خاصة بالمستخدمين
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>الحصول على مستخدم بالبريد الإلكتروني</summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>التحقق من وجود البريد الإلكتروني</summary>
    Task<bool> EmailExistsAsync(string email);
}

/// <summary>
/// واجهة مستودع الخدمات
/// </summary>
public interface IServiceRepository : IRepository<Service>
{
    /// <summary>الحصول على الخدمات النشطة</summary>
    Task<IReadOnlyList<Service>> GetActiveAsync();

    /// <summary>الحصول على الخدمات حسب الفئة</summary>
    Task<IReadOnlyList<Service>> GetByCategoryAsync(ServiceCategory category);
}

/// <summary>
/// واجهة مستودع الحلاقين
/// </summary>
public interface IBarberRepository : IRepository<Barber>
{
    /// <summary>الحصول على الحلاقين المتاحين</summary>
    Task<IReadOnlyList<Barber>> GetAvailableAsync();
}

/// <summary>
/// واجهة مستودع المواعيد - تحتوي على استعلامات معقدة
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>الحصول على مواعيد عميل معين</summary>
    Task<IReadOnlyList<Appointment>> GetByCustomerAsync(Guid customerId);

    /// <summary>الحصول على مواعيد حلاق معين</summary>
    Task<IReadOnlyList<Appointment>> GetByBarberAsync(Guid barberId);

    /// <summary>الحصول على مواعيد تاريخ معين</summary>
    Task<IReadOnlyList<Appointment>> GetByDateAsync(DateTime date);

    /// <summary>التحقق من وجود تعارض في المواعيد</summary>
    Task<bool> HasConflictAsync(Guid barberId, DateTime date, string startTime);

    /// <summary>الحصول على مواعيد حسب الحالة</summary>
    Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status);

    /// <summary>الحصول على مواعيد مع ترقيم الصفحات</summary>
    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? customerId = null);
}

/// <summary>
/// واجهة مستودع رسائل الاتصال
/// </summary>
public interface IContactRepository : IRepository<ContactMessage>
{
    /// <summary>الحصول على الرسائل غير المقروءة</summary>
    Task<IReadOnlyList<ContactMessage>> GetUnreadAsync();
}
