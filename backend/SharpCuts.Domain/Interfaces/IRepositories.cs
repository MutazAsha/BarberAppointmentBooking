namespace SharpCuts.Domain.Interfaces;

using SharpCuts.Domain.Entities;

// Repository Pattern - Generic Repository Interface
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}

// Specific Repository Interfaces
public interface IServiceRepository : IRepository<Service>
{
    Task<IEnumerable<Service>> GetByCategoryAsync(string category);
    Task<IEnumerable<Service>> GetActiveServicesAsync();
}

public interface IBarberRepository : IRepository<Barber>
{
    Task<IEnumerable<Barber>> GetAvailableBarbersAsync();
    Task<Barber?> GetBySpecialtyAsync(string specialty);
}

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByCustomerEmailAsync(string email);
    Task<IEnumerable<Appointment>> GetByBarberIdAsync(Guid barberId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<bool> HasConflictAsync(Guid barberId, DateTime date, string time);
}

public interface ITimeSlotRepository : IRepository<TimeSlot>
{
    Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(Guid barberId, DateTime date);
    Task BookSlotAsync(Guid slotId);
}
