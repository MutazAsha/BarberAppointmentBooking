namespace SharpCuts.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using SharpCuts.Domain.Interfaces;

/// <summary>
/// Bookings Controller - Handles appointment booking operations
/// Uses constructor injection for IBookingService
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    // Dependency Injection via Constructor
    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/bookings - Create a new appointment booking
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bookingService.CreateBookingAsync(request);

        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(
            nameof(GetBooking),
            new { id = result.AppointmentId },
            new { id = result.AppointmentId, message = "Booking created successfully" });
    }

    /// <summary>
    /// GET /api/bookings/{id} - Get booking details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        // Implementation would fetch specific booking
        return Ok(new { id, message = "Booking details" });
    }

    /// <summary>
    /// PUT /api/bookings/{id}/cancel - Cancel a booking
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var success = await _bookingService.CancelBookingAsync(id);
        if (!success)
            return NotFound(new { message = "Booking not found" });

        return Ok(new { message = "Booking cancelled successfully" });
    }

    /// <summary>
    /// GET /api/bookings/customer/{email} - Get customer appointments
    /// </summary>
    [HttpGet("customer/{email}")]
    public async Task<IActionResult> GetCustomerAppointments(string email)
    {
        var appointments = await _bookingService.GetCustomerAppointmentsAsync(email);
        return Ok(appointments);
    }
}

/// <summary>
/// Services Controller - Handles service catalog operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceManagementService _serviceService;

    public ServicesController(IServiceManagementService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServices()
    {
        var services = await _serviceService.GetAllServices();
        return Ok(services);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var services = await _serviceService.GetServicesByCategoryAsync(category);
        return Ok(services);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var service = await _serviceService.GetServiceByIdAsync(id);
        if (service == null) return NotFound();
        return Ok(service);
    }
}

/// <summary>
/// Barbers Controller - Handles barber operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly IBarberService _barberService;

    public BarbersController(IBarberService barberService)
    {
        _barberService = barberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBarbers()
    {
        var barbers = await _barberService.GetAllBarbersAsync();
        return Ok(barbers);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBarbers()
    {
        var barbers = await _barberService.GetAvailableBarbersAsync();
        return Ok(barbers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBarberById(Guid id)
    {
        var barber = await _barberService.GetBarberByIdAsync(id);
        if (barber == null) return NotFound();
        return Ok(barber);
    }
}
