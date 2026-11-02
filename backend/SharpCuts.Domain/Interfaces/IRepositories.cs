namespace SharpCuts.Domain.Interfaces;

using SharpCuts.Domain.Entities;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
}

public interface IServiceRepository : IRepository<Service>
{
    Task<IEnumerable<Service>> GetByCategoryAsync(string category);
    Task<IEnumerable<Service>> GetActiveServicesAsync();
}

public interface IBarberRepository : IRepository<Barber>
{
    Task<IEnumerable<Barber>> GetAvailableBarbersAsync();
}

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByCustomerEmailAsync(string email);
    Task<IEnumerable<Appointment>> GetByBarberIdAsync(Guid barberId);
    Task<IEnumerable<Appointment>> GetByDateAsync(string date);
    Task<bool> HasConflictAsync(Guid barberId, string date, string time);
}

public interface IContactRepository : IRepository<ContactMessage>
{
}
