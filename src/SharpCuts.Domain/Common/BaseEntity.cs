// ============================================
// DOMAIN LAYER - Common Base Entity
// ============================================
// الكيان الأساسي الذي ترث منه جميع الكيانات
// يوفر خصائص مشتركة مثل ID وتواريخ الإنشاء والتحديث

namespace SharpCuts.Domain.Common;

/// <summary>
/// الكيان الأساسي - جميع الكيانات ترث من هذه الفئة
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// المعرف الفريد للكيان
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// تاريخ إنشاء السجل
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تاريخ آخر تحديث (nullable لأنه قد لا يُحدث)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// معرّف المستخدم الذي أنشأ السجل (للتدقيق)
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// معرّف المستخدم الذي آخر من أحدث السجل
    /// </summary>
    public string? UpdatedBy { get; set; }
}
