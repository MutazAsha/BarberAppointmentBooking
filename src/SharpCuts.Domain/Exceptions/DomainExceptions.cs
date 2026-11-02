// ============================================
// DOMAIN LAYER - Exceptions
// ============================================
// استثناءات خاصة بالنطاق (Domain)

namespace SharpCuts.Domain.Exceptions;

/// <summary>
/// استثناء عام للنطاق
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// استثناء عند عدم العثور على كيان
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, Guid id) 
        : base($"{entityName} with ID {id} was not found") { }

    public NotFoundException(string message) : base(message) { }
}

/// <summary>
/// استثناء عند انتهاك قاعدة عمل
/// </summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>
/// استثناء عند وجود تعارض (مثل حجز مزدوج)
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// استثناء عند عدم الصلاحية
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "You do not have permission to perform this action") 
        : base(message) { }
}
