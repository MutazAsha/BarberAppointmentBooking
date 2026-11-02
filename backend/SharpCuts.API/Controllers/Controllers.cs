using Microsoft.AspNetCore.Mvc;
using SharpCuts.Application.Services;

namespace SharpCuts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly ServiceManagementService _service;
    public ServicesController(ServiceManagementService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _service.GetAllServicesAsync() });

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category) => Ok(new { success = true, data = await _service.GetServicesByCategoryAsync(category) });

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var service = await _service.GetServiceByIdAsync(id);
        return service == null ? NotFound(new { success = false, error = "Service not found" }) : Ok(new { success = true, data = service });
    }
}

[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly BarberService _service;
    public BarbersController(BarberService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _service.GetAllBarbersAsync() });

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable() => Ok(new { success = true, data = await _service.GetAvailableBarbersAsync() });

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var barber = await _service.GetBarberByIdAsync(id);
        return barber == null ? NotFound(new { success = false, error = "Barber not found" }) : Ok(new { success = true, data = barber });
    }

    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> ToggleAvailability(Guid id)
    {
        var barber = await _service.ToggleAvailabilityAsync(id);
        return barber == null ? NotFound(new { success = false, error = "Barber not found" }) : Ok(new { success = true, data = barber });
    }
}

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _service;
    public BookingsController(BookingService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _service.GetAllAppointmentsAsync() });

    [HttpGet("customer/{email}")]
    public async Task<IActionResult> GetByCustomer(string email) => Ok(new { success = true, data = await _service.GetAppointmentsByEmailAsync(email) });

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        try
        {
            var appointment = await _service.CreateBookingAsync(request.ServiceId, request.BarberId, request.Date, request.Time, request.CustomerName, request.CustomerPhone, request.CustomerEmail, request.Notes);
            return CreatedAtAction(nameof(GetAll), new { success = true, data = appointment, message = "Booking created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            await _service.CancelBookingAsync(id);
            return Ok(new { success = true, message = "Booking cancelled successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactService _service;
    public ContactController(ContactService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ContactRequest request)
    {
        var message = await _service.SubmitMessageAsync(request.Name, request.Email, request.Subject, request.Message);
        return CreatedAtAction(nameof(GetAll), new { success = true, data = message, message = "Message sent successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _service.GetAllMessagesAsync() });
}

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;
    public DashboardController(DashboardService service) => _service = service;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats() => Ok(new { success = true, data = await _service.GetStatsAsync() });
}

public record CreateBookingRequest(Guid ServiceId, Guid BarberId, string Date, string Time, string CustomerName, string CustomerPhone, string CustomerEmail, string? Notes);
public record ContactRequest(string Name, string Email, string Subject, string Message);
