// ============================================
// INFRASTRUCTURE LAYER - Repository Implementations
// ============================================
// تطبيقات المستودعات - تستخدم Entity Framework Core

using Microsoft.EntityFrameworkCore;
using SharpCuts.Application.Interfaces;
using SharpCuts.Domain.Common;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;
using SharpCuts.Infrastructure.Data;

namespace SharpCuts.Infrastructure.Repositories;

/// <summary>
/// المستودع العام - يوفر عمليات CRUD الأساسية
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// الأداء: AsNoTracking() أسرع للاستعلامات للقراءة فقط
    /// </summary>
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
}

/// <summary>
/// مستودع المستخدمين
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}

/// <summary>
/// مستودع الخدمات
/// </summary>
public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Service>> GetActiveAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Service>> GetByCategoryAsync(ServiceCategory category)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Category == category && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}

/// <summary>
/// مستودع الحلاقين
/// </summary>
public class BarberRepository : Repository<Barber>, IBarberRepository
{
    public BarberRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Barber>> GetAvailableAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.IsAvailable)
            .OrderByDescending(b => b.Rating)
            .ToListAsync();
    }
}

/// <summary>
/// مستودع المواعيد - يحتوي على استعلامات معقدة
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Appointment>> GetByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(a => a.Service)
            .Include(a => a.Barber)
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Appointment>> GetByBarberAsync(Guid barberId)
    {
        return await _dbSet
            .Include(a => a.Service)
            .Include(a => a.Customer)
            .Where(a => a.BarberId == barberId)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Appointment>> GetByDateAsync(DateTime date)
    {
        return await _dbSet
            .Include(a => a.Service)
            .Include(a => a.Barber)
            .Include(a => a.Customer)
            .Where(a => a.AppointmentDate.Date == date.Date)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// حاسم: التحقق من التعارض في المواعيد
    /// </summary>
    public async Task<bool> HasConflictAsync(Guid barberId, DateTime date, string startTime)
    {
        return await _dbSet
            .AnyAsync(a => 
                a.BarberId == barberId &&
                a.AppointmentDate.Date == date.Date &&
                a.StartTime == startTime &&
                a.Status != AppointmentStatus.Cancelled);
    }

    public async Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status)
    {
        return await _dbSet
            .Include(a => a.Service)
            .Include(a => a.Barber)
            .Include(a => a.Customer)
            .Where(a => a.Status == status)
            .ToListAsync();
    }

    /// <summary>
    /// الأداء: ترقيم الصفحات يمنع تحميل جميع السجلات
    /// </summary>
    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? customerId = null)
    {
        var query = _dbSet
            .Include(a => a.Service)
            .Include(a => a.Barber)
            .Include(a => a.Customer)
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(a => a.CustomerId == customerId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}

/// <summary>
/// مستودع رسائل الاتصال
/// </summary>
public class ContactRepository : Repository<ContactMessage>, IContactRepository
{
    public ContactRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ContactMessage>> GetUnreadAsync()
    {
        return await _dbSet
            .Where(m => !m.IsRead)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}
