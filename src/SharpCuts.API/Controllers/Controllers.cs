// ============================================
// API LAYER - Controllers
// ============================================
// المتحكمات - نقاط النهاية HTTP

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;
using System.Security.Claims;

namespace SharpCuts.API.Controllers;

// ============================================
// AUTH CONTROLLER (Public)
// ============================================

/// <summary>
/// متحكم المصادقة - تسجيل الدخول والتسجيل
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// تسجيل مستخدم جديد
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(new ApiResponse<AuthResponse>(true, result, "تم التسجيل بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, Error: ex.Message));
        }
    }

    /// <summary>
    /// تسجيل الدخول
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(new ApiResponse<AuthResponse>(true, result, "تم تسجيل الدخول بنجاح"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>(false, Error: ex.Message));
        }
    }
}

// ============================================
// SERVICES CONTROLLER (Public)
// ============================================

/// <summary>
/// متحكم الخدمات - عام (بدون مصادقة)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _service;

    public ServicesController(IServiceService service) => _service = service;

    /// <summary>
    /// الحصول على جميع الخدمات
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 300)] // تخزين مؤقت 5 دقائق
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ServiceDto>>>> GetAll()
    {
        var services = await _service.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<ServiceDto>>(true, services));
    }

    /// <summary>
    /// الحصول على الخدمات حسب الفئة
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ServiceDto>>>> GetByCategory(string category)
    {
        var services = await _service.GetByCategoryAsync(category);
        return Ok(new ApiResponse<IReadOnlyList<ServiceDto>>(true, services));
    }

    /// <summary>
    /// الحصول على خدمة بالمعرف
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ServiceDto>>> GetById(Guid id)
    {
        var service = await _service.GetByIdAsync(id);
        if (service == null)
            return NotFound(new ApiResponse<ServiceDto>(false, Error: "الخدمة غير موجودة"));
        
        return Ok(new ApiResponse<ServiceDto>(true, service));
    }
}

// ============================================
// BARBERS CONTROLLER (Public)
// ============================================

/// <summary>
/// متحكم الحلاقين - عام
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BarbersController : ControllerBase
{
    private readonly IBarberService _service;

    public BarbersController(IBarberService service) => _service = service;

    [HttpGet]
    [ResponseCache(Duration = 300)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BarberDto>>>> GetAll()
    {
        var barbers = await _service.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<BarberDto>>(true, barbers));
    }

    [HttpGet("available")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BarberDto>>>> GetAvailable()
    {
        var barbers = await _service.GetAvailableAsync();
        return Ok(new ApiResponse<IReadOnlyList<BarberDto>>(true, barbers));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BarberDto>>> GetById(Guid id)
    {
        var barber = await _service.GetByIdAsync(id);
        if (barber == null)
            return NotFound(new ApiResponse<BarberDto>(false, Error: "الحلاق غير موجود"));
        
        return Ok(new ApiResponse<BarberDto>(true, barber));
    }
}

// ============================================
// BOOKINGS CONTROLLER (Protected)
// ============================================

/// <summary>
/// متحكم الحجوزات - محمي (يتطلب مصادقة)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // جميع النقاط المحمية تتطلب JWT
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
    /// الحصول على معرّف المستخدم الحالي من JWT
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("معرّف المستخدم غير موجود");
        return Guid.Parse(userIdClaim.Value);
    }

    /// <summary>
    /// إنشاء موعد جديد
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        try
        {
            var customerId = GetCurrentUserId();
            var appointment = await _service.CreateAsync(customerId, request);
            return CreatedAtAction(nameof(GetMyAppointments), 
                new ApiResponse<AppointmentDto>(true, appointment, "تم إنشاء الموعد بنجاح"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, Error: ex.Message));
        }
    }

    /// <summary>
    /// الحصول على مواعيدي
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentDto>>>> GetMyAppointments()
    {
        var customerId = GetCurrentUserId();
        var appointments = await _service.GetByCustomerAsync(customerId);
        return Ok(new ApiResponse<IReadOnlyList<AppointmentDto>>(true, appointments));
    }

    /// <summary>
    /// إلغاء موعد
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var customerId = GetCurrentUserId();
            await _service.CancelAsync(id, customerId);
            return Ok(new ApiResponse<bool>(true, true, "تم إلغاء الموعد"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(false, Error: ex.Message));
        }
    }

    /// <summary>
    /// الحصول على الفترات الزمنية المتاحة
    /// </summary>
    [HttpGet("slots/{barberId}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TimeSlotDto>>>> GetAvailableSlots(
        Guid barberId, [FromQuery] DateTime date)
    {
        var slots = await _service.GetAvailableSlotsAsync(barberId, date);
        return Ok(new ApiResponse<IReadOnlyList<TimeSlotDto>>(true, slots));
    }
}

// ============================================
// DASHBOARD CONTROLLER (Admin Only)
// ============================================

/// <summary>
/// متحكم لوحة التحكم - للمديرين فقط
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // المديرين فقط
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        var stats = await _service.GetStatsAsync();
        return Ok(new ApiResponse<DashboardStatsDto>(true, stats));
    }

    [HttpGet("today")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentDto>>>> GetTodaySchedule()
    {
        var schedule = await _service.GetTodayScheduleAsync();
        return Ok(new ApiResponse<IReadOnlyList<AppointmentDto>>(true, schedule));
    }
}

// ============================================
// CONTACT CONTROLLER (Public)
// ============================================

/// <summary>
/// متحكم الاتصال - عام
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _service;

    public ContactController(IContactService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] ContactRequest request)
    {
        await _service.SubmitAsync(request);
        return Ok(new ApiResponse<bool>(true, true, "تم إرسال الرسالة بنجاح"));
    }
}
