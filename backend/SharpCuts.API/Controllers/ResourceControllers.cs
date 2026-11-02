// ============================================
// API LAYER - Resource Controllers
// ============================================
// Controllers for Services, Barbers, and Contact.
// These are mostly PUBLIC endpoints (no auth required).

using Microsoft.AspNetCore.Mvc;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;

namespace SharpCuts.API.Controllers;

/// <summary>
/// Service catalog endpoints.
/// PUBLIC - No authentication required.
/// 
/// PERFORMANCE: Responses are cached for 5 minutes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _service;

    public ServicesController(IServiceService service) => _service = service;

    /// <summary>Get all active services</summary>
    [HttpGet]
    [ResponseCache(Duration = 300)] // Cache for 5 minutes (performance)
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ServiceDto>>>> GetAll()
    {
        var services = await _service.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<ServiceDto>>(true, services));
    }

    /// <summary>Get services by category</summary>
    [HttpGet("category/{category}")]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "category" })]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ServiceDto>>>> GetByCategory(string category)
    {
        var services = await _service.GetByCategoryAsync(category);
        return Ok(new ApiResponse<IReadOnlyList<ServiceDto>>(true, services));
    }

    /// <summary>Get service by ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> GetById(Guid id)
    {
        var service = await _service.GetByIdAsync(id);
        if (service == null)
            return NotFound(new ApiResponse<ServiceDto>(false, Error: "Service not found"));
        
        return Ok(new ApiResponse<ServiceDto>(true, service));
    }
}

/// <summary>
/// Barber endpoints.
/// PUBLIC - No authentication required.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly IBarberService _service;

    public BarbersController(IBarberService service) => _service = service;

    /// <summary>Get all barbers</summary>
    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BarberDto>>>> GetAll()
    {
        var barbers = await _service.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<BarberDto>>(true, barbers));
    }

    /// <summary>Get available barbers only</summary>
    [HttpGet("available")]
    [ResponseCache(Duration = 60)] // Shorter cache (availability changes)
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BarberDto>>>> GetAvailable()
    {
        var barbers = await _service.GetAvailableAsync();
        return Ok(new ApiResponse<IReadOnlyList<BarberDto>>(true, barbers));
    }

    /// <summary>Get barber by ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BarberDto>>> GetById(Guid id)
    {
        var barber = await _service.GetByIdAsync(id);
        if (barber == null)
            return NotFound(new ApiResponse<BarberDto>(false, Error: "Barber not found"));
        
        return Ok(new ApiResponse<BarberDto>(true, barber));
    }
}

/// <summary>
/// Contact form endpoints.
/// PUBLIC - No authentication required.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _service;

    public ContactController(IContactService service) => _service = service;

    /// <summary>Submit contact form</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<bool>>> Submit([FromBody] ContactRequest request)
    {
        await _service.SubmitAsync(request);
        return Ok(new ApiResponse<bool>(true, true, "Message sent successfully"));
    }
}
