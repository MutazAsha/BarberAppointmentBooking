namespace SharpCuts.Application.Services;

using SharpCuts.Domain.Interfaces;

public class BarberService : IBarberService
{
    private readonly IBarberRepository _barberRepository;

    public BarberService(IBarberRepository barberRepository)
    {
        _barberRepository = barberRepository;
    }

    public async Task<IEnumerable<BarberDto>> GetAllBarbersAsync()
    {
        var barbers = await _barberRepository.GetAllAsync();
        return barbers.Select(MapToDto);
    }

    public async Task<IEnumerable<BarberDto>> GetAvailableBarbersAsync()
    {
        var barbers = await _barberRepository.GetAvailableBarbersAsync();
        return barbers.Select(MapToDto);
    }

    public async Task<BarberDto?> GetBarberByIdAsync(Guid id)
    {
        var barber = await _barberRepository.GetByIdAsync(id);
        return barber == null ? null : MapToDto(barber);
    }

    private static BarberDto MapToDto(Domain.Entities.Barber barber) => new(
        barber.Id,
        barber.Name,
        barber.Avatar,
        barber.Specialty,
        barber.Rating,
        barber.ReviewCount,
        barber.ExperienceYears,
        barber.IsAvailable
    );
}

public class ServiceManagementService : IServiceManagementService
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceManagementService(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
    {
        var services = await _serviceRepository.GetActiveServicesAsync();
        return services.Select(MapToDto);
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesByCategoryAsync(string category)
    {
        var services = await _serviceRepository.GetByCategoryAsync(category);
        return services.Select(MapToDto);
    }

    public async Task<ServiceDto?> GetServiceByIdAsync(Guid id)
    {
        var service = await _serviceRepository.GetByIdAsync(id);
        return service == null ? null : MapToDto(service);
    }

    private static ServiceDto MapToDto(Domain.Entities.Service service) => new(
        service.Id,
        service.Name,
        service.Description,
        service.DurationMinutes,
        service.Price,
        service.Category
    );
}
