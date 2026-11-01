using Microsoft.EntityFrameworkCore;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Interfaces;
using SharpCuts.Infrastructure.Data;

namespace SharpCuts.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;
    public ServiceRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Service>> GetAllAsync() => await _context.Services.ToListAsync();
    public async Task<Service?> GetByIdAsync(Guid id) => await _context.Services.FindAsync(id);
    public async Task<Service> AddAsync(Service entity) { await _context.Services.AddAsync(entity); await _context.SaveChangesAsync(); return entity; }
    public async Task UpdateAsync(Service entity) { _context.Services.Update(entity); await _context.SaveChangesAsync(); }
    public async Task<bool> DeleteAsync(Guid id) { var e = await GetByIdAsync(id); if (e == null) return false; _context.Services.Remove(e); await _context.SaveChangesAsync(); return true; }
    public async Task<IEnumerable<Service>> GetByCategoryAsync(string category) => await _context.Services.Where(s => s.Category == category && s.IsActive).ToListAsync();
    public async Task<IEnumerable<Service>> GetActiveServicesAsync() => await _context.Services.Where(s => s.IsActive).ToListAsync();
}

public class BarberRepository : IBarberRepository
{
    private readonly AppDbContext _context;
    public BarberRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Barber>> GetAllAsync() => await _context.Barbers.ToListAsync();
    public async Task<Barber?> GetByIdAsync(Guid id) => await _context.Barbers.FindAsync(id);
    public async Task<Barber> AddAsync(Barber entity) { await _context.Barbers.AddAsync(entity); await _context.SaveChangesAsync(); return entity; }
    public async Task UpdateAsync(Barber entity) { _context.Barbers.Update(entity); await _context.SaveChangesAsync(); }
    public async Task<bool> DeleteAsync(Guid id) { var e = await GetByIdAsync(id); if (e == null) return false; _context.Barbers.Remove(e); await _context.SaveChangesAsync(); return true; }
    public async Task<IEnumerable<Barber>> GetAvailableBarbersAsync() => await _context.Barbers.Where(b => b.Available).ToListAsync();
}

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;
    public AppointmentRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Appointment>> GetAllAsync() => await _context.Appointments.ToListAsync();
    public async Task<Appointment?> GetByIdAsync(Guid id) => await _context.Appointments.FindAsync(id);
    public async Task<Appointment> AddAsync(Appointment entity) { await _context.Appointments.AddAsync(entity); await _context.SaveChangesAsync(); return entity; }
    public async Task UpdateAsync(Appointment entity) { _context.Appointments.Update(entity); await _context.SaveChangesAsync(); }
    public async Task<bool> DeleteAsync(Guid id) { var e = await GetByIdAsync(id); if (e == null) return false; _context.Appointments.Remove(e); await _context.SaveChangesAsync(); return true; }
    public async Task<IEnumerable<Appointment>> GetByCustomerEmailAsync(string email) => await _context.Appointments.Where(a => a.CustomerEmail == email).ToListAsync();
    public async Task<IEnumerable<Appointment>> GetByBarberIdAsync(Guid barberId) => await _context.Appointments.Where(a => a.BarberId == barberId).ToListAsync();
    public async Task<IEnumerable<Appointment>> GetByDateAsync(string date) => await _context.Appointments.Where(a => a.Date == date).ToListAsync();
    public async Task<bool> HasConflictAsync(Guid barberId, string date, string time) => await _context.Appointments.AnyAsync(a => a.BarberId == barberId && a.Date == date && a.Time == time && a.Status != "cancelled");
}

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;
    public ContactRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<ContactMessage>> GetAllAsync() => await _context.ContactMessages.ToListAsync();
    public async Task<ContactMessage?> GetByIdAsync(Guid id) => await _context.ContactMessages.FindAsync(id);
    public async Task<ContactMessage> AddAsync(ContactMessage entity) { await _context.ContactMessages.AddAsync(entity); await _context.SaveChangesAsync(); return entity; }
    public async Task UpdateAsync(ContactMessage entity) { _context.ContactMessages.Update(entity); await _context.SaveChangesAsync(); }
    public async Task<bool> DeleteAsync(Guid id) { var e = await GetByIdAsync(id); if (e == null) return false; _context.ContactMessages.Remove(e); await _context.SaveChangesAsync(); return true; }
}
