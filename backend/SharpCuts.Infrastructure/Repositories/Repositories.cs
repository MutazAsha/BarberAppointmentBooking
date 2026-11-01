namespace SharpCuts.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Interfaces;
using SharpCuts.Infrastructure.Data;

/// <summary>
/// Generic Repository Implementation using Entity Framework Core
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

public class ServiceRepository : Repository<Service>, IServiceRepository
{
    public ServiceRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Service>> GetByCategoryAsync(string category)
        => await _dbSet.Where(s => s.Category == category && s.IsActive).ToListAsync();

    public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        => await _dbSet.Where(s => s.IsActive).ToListAsync();
}

public class BarberRepository : Repository<Barber>, IBarberRepository
{
    public BarberRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Barber>> GetAvailableBarbersAsync()
        => await _dbSet.Where(b => b.IsAvailable).ToListAsync();

    public async Task<Barber?> GetBySpecialtyAsync(string specialty)
        => await _dbSet.FirstOrDefaultAsync(b => b.Specialty.Contains(specialty));
}

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Appointment>> GetByCustomerEmailAsync(string email)
        => await _dbSet
            .Include(a => a.Service)
            .Include(a => a.Barber)
            .Where(a => a.CustomerEmail == email)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByBarberIdAsync(Guid barberId)
        => await _dbSet
            .Include(a => a.Service)
            .Where(a => a.BarberId == barberId)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime start, DateTime end)
        => await _dbSet
            .Include(a => a.Service)
            .Include(a => a.Barber)
            .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
            .ToListAsync();

    public async Task<bool> HasConflictAsync(Guid barberId, DateTime date, string time)
        => await _dbSet.AnyAsync(a =>
            a.BarberId == barberId &&
            a.AppointmentDate == date &&
            a.AppointmentTime == time &&
            a.Status != AppointmentStatus.Cancelled);
}

public class TimeSlotRepository : Repository<TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid barberId, DateTime date)
        => await _dbSet
            .Where(t => t.BarberId == barberId && t.Date == date && t.IsAvailable && !t.IsBooked)
            .OrderBy(t => t.Time)
            .ToListAsync();

    public async Task BookSlotAsync(Guid slotId)
    {
        var slot = await GetByIdAsync(slotId);
        if (slot != null)
        {
            slot.IsBooked = true;
            await UpdateAsync(slot);
        }
    }
}
