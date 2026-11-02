// ============================================
// INFRASTRUCTURE LAYER - Repository Implementations
// ============================================
// These classes implement the repository interfaces from the Application layer.
// They use Entity Framework Core to query and persist data.
//
// PERFORMANCE NOTES:
// - All methods are async (non-blocking)
// - Uses AsNoTracking() for read-only queries (faster)
// - Uses proper indexing (configured in DbContext)

using Microsoft.EntityFrameworkCore;
using SharpCuts.Application.Interfaces;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;
using SharpCuts.Infrastructure.Data;

namespace SharpCuts.Infrastructure.Repositories;

/// <summary>
/// Generic repository base class.
/// Provides common CRUD operations for all entities.
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
    /// PERFORMANCE: AsNoTracking() tells EF not to track changes.
    /// This is faster for read-only queries.
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
/// User repository with authentication-specific queries.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // PERFORMANCE: Email is indexed, so this is fast
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        // PERFORMANCE: Any() is optimized by EF to use EXISTS in SQL
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}

/// <summary>
/// Service repository with category filtering.
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
        // PERFORMANCE: Category is indexed
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.Category == category && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}

/// <summary>
/// Barber repository with availability filtering.
/// </summary>
public class BarberRepository : Repository<Barber>, IBarberRepository
{
    public BarberRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Barber>> GetAvailableAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(b => b.IsAvailable)
            .OrderBy(b => b.Rating descending) // Show highest-rated first
            .ToListAsync();
    }
}

/// <summary>
/// Appointment repository with complex scheduling queries.
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Appointment>> GetByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(a => a.Service) // Eager load related data
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
    /// CRITICAL: Checks for scheduling conflicts.
    /// Uses the composite index for fast lookup.
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
    /// PERFORMANCE: Pagination prevents loading all records.
    /// Uses Skip/Take for efficient paging.
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
/// Contact message repository.
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
