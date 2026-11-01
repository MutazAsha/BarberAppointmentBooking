// ============================================
// API LAYER - Authentication Controller
// ============================================
// Handles user registration and login.
// These endpoints are PUBLIC (no authentication required).

using Microsoft.AspNetCore.Mvc;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;

namespace SharpCuts.API.Controllers;

/// <summary>
/// Authentication endpoints.
/// 
/// ENDPOINTS:
/// POST /api/auth/register - Create new account
/// POST /api/auth/login    - Authenticate and get JWT token
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
    /// Registers a new user account.
    /// </summary>
    /// <response code="200">Registration successful, returns JWT token</response>
    /// <response code="400">Invalid data or email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registration request for: {Email}", request.Email);
            
            var result = await _authService.RegisterAsync(request);
            
            return Ok(new ApiResponse<AuthResponse>(
                Success: true,
                Data: result,
                Message: "Registration successful"
            ));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<object>(
                Success: false,
                Error: ex.Message
            ));
        }
    }

    /// <summary>
    /// Authenticates user and returns JWT token.
    /// </summary>
    /// <response code="200">Login successful, returns JWT token</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login request for: {Email}", request.Email);
            
            var result = await _authService.LoginAsync(request);
            
            return Ok(new ApiResponse<AuthResponse>(
                Success: true,
                Data: result,
                Message: "Login successful"
            ));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>(
                Success: false,
                Error: ex.Message
            ));
        }
    }
}
