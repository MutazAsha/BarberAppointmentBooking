// ============================================
// API LAYER - Protected Controllers
// ============================================
// These controllers require JWT authentication.
// The [Authorize] attribute ensures only logged-in users can access.

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;
using SharpCuts.Domain.Enums;

namespace SharpCuts.API.Controllers;

/// <summary>
/// Booking/Appointment endpoints.
/// PROTECTED - Requires authentication.
/// 
/// SECURITY:
/// - All endpoints require valid JWT token
/// - Users can only manage their own appointments
/// - Admins can view all appointments
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints in this controller require authentication
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService service, ILogger<BookingsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Helper to get the current user's ID from JWT claims.
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("User ID not found in token");
        return Guid.Parse(userIdClaim.Value);
    }

    /// <summary>
    /// Create a new appointment.
    /// The customer ID is taken from the JWT token (not from request body).
    /// This prevents users from booking for other customers.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            _logger.LogInformation("Customer {CustomerId} creating appointment", customerId);

            var appointment = await _service.CreateAsync(customerId, request);

            return CreatedAtAction(
                nameof(GetMyAppointments),
                new ApiResponse<AppointmentDto>(
                    Success: true,
                    Data: appointment,
                    Message: "Appointment created successfully"
                ));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>(false, Error: ex.Message));
        }
    }

    /// <summary>
    /// Get current user's appointments.
    /// SECURITY: Only returns appointments for the authenticated user.
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentDto>>>> GetMyAppointments()
    {
        var customerId = GetCurrentUserId();
        var appointments = await _service.GetByCustomerAsync(customerId);
        return Ok(new ApiResponse<IReadOnlyList<AppointmentDto>>(true, appointments));
    }

    /// <summary>
    /// Cancel an appointment.
    /// SECURITY: Users can only cancel their own appointments.
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var customerId = GetCurrentUserId();
            await _service.CancelAsync(id, customerId);
            return Ok(new ApiResponse<bool>(true, true, "Appointment cancelled"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>(false, Error: ex.Message));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // 403 - User tried to cancel someone else's appointment
        }
    }

    /// <summary>
    /// Get available time slots for a barber on a specific date.
    /// PUBLIC within authenticated context (any logged-in user can check).
    /// </summary>
    [HttpGet("slots/{barberId}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TimeSlotDto>>>> GetAvailableSlots(
        Guid barberId, [FromQuery] DateTime date)
    {
        var slots = await _service.GetAvailableSlotsAsync(barberId, date);
        return Ok(new ApiResponse<IReadOnlyList<TimeSlotDto>>(true, slots));
    }
}

/// <summary>
/// Dashboard endpoints.
/// PROTECTED - Admin only.
/// 
/// SECURITY: Uses role-based authorization.
/// Only users with Admin role can access these endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Only admins can access
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    /// <summary>
    /// Get aggregated statistics for the dashboard.
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        var stats = await _service.GetStatsAsync();
        return Ok(new ApiResponse<DashboardStatsDto>(true, stats));
    }

    /// <summary>
    /// Get today's schedule (all appointments for today).
    /// </summary>
    [HttpGet("today")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentDto>>>> GetTodaySchedule()
    {
        var schedule = await _service.GetTodayScheduleAsync();
        return Ok(new ApiResponse<IReadOnlyList<AppointmentDto>>(true, schedule));
    }
}
