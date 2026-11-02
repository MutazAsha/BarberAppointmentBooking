// ============================================
// APPLICATION LAYER - Repository Interfaces
// ============================================
// These interfaces define the CONTRACT for data access.
// The Infrastructure layer provides the IMPLEMENTATION.
//
// WHY INTERFACES?
// - Enables Dependency Injection
// - Allows swapping data sources (InMemory → SQL Server → PostgreSQL)
// - Makes testing easy (mock repositories in unit tests)
// - Enforces separation of concerns

using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Application.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// All specific repositories extend this.
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Get all records</summary>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>Get a single record by ID</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>Create a new record</summary>
    Task<T> AddAsync(T entity);

    /// <summary>Update an existing record</summary>
    Task UpdateAsync(T entity);

    /// <summary>Delete a record by ID</summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>Check if a record exists</summary>
    Task<bool> ExistsAsync(Guid id);
}

/// <summary>
/// User-specific repository operations.
/// Extends basic CRUD with authentication-related queries.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Find user by email (for login)</summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>Check if email is already registered</summary>
    Task<bool> EmailExistsAsync(string email);
}

/// <summary>
/// Service-specific repository operations.
/// </summary>
public interface IServiceRepository : IRepository<Service>
{
    /// <summary>Get all active services</summary>
    Task<IReadOnlyList<Service>> GetActiveAsync();

    /// <summary>Get services filtered by category</summary>
    Task<IReadOnlyList<Service>> GetByCategoryAsync(ServiceCategory category);
}

/// <summary>
/// Barber-specific repository operations.
/// </summary>
public interface IBarberRepository : IRepository<Barber>
{
    /// <summary>Get all available barbers</summary>
    Task<IReadOnlyList<Barber>> GetAvailableAsync();
}

/// <summary>
/// Appointment-specific repository operations.
/// Contains complex queries for scheduling logic.
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    /// <summary>Get appointments for a specific customer</summary>
    Task<IReadOnlyList<Appointment>> GetByCustomerAsync(Guid customerId);

    /// <summary>Get appointments for a specific barber</summary>
    Task<IReadOnlyList<Appointment>> GetByBarberAsync(Guid barberId);

    /// <summary>Get appointments for a specific date</summary>
    Task<IReadOnlyList<Appointment>> GetByDateAsync(DateTime date);

    /// <summary>
    /// CRITICAL: Check if a barber has a conflicting appointment.
    /// Used to prevent double-booking.
    /// </summary>
    Task<bool> HasConflictAsync(Guid barberId, DateTime date, string startTime);

    /// <summary>Get appointments with status filter</summary>
    Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status);

    /// <summary>Get paginated appointments</summary>
    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? customerId = null);
}

/// <summary>
/// Contact message repository operations.
/// </summary>
public interface IContactRepository : IRepository<ContactMessage>
{
    /// <summary>Get unread messages</summary>
    Task<IReadOnlyList<ContactMessage>> GetUnreadAsync();
}
