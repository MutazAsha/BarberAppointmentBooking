// ============================================
// APPLICATION LAYER - Authentication Service
// ============================================
// Handles user registration, login, and JWT token generation.
// 
// SECURITY FEATURES:
// - Passwords are hashed with BCrypt (never stored in plain text)
// - JWT tokens are signed with a secret key
// - Tokens expire after a configurable duration
// - Role-based claims for authorization

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SharpCuts.Application.DTOs;
using SharpCuts.Application.Interfaces;
using SharpCuts.Domain.Entities;
using SharpCuts.Domain.Enums;

namespace SharpCuts.Application.Services;

/// <summary>
/// Implements authentication and JWT token management.
/// 
/// HOW IT WORKS:
/// 1. User registers → password is hashed with BCrypt → stored in DB
/// 2. User logs in → password is verified against hash → JWT token is issued
/// 3. User sends token in Authorization header → token is validated → access granted
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    // JWT token expiration (configurable via appsettings.json)
    private readonly int _tokenExpirationHours;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
        
        // Read token expiration from config (default: 24 hours)
        _tokenExpirationHours = _configuration.GetValue<int>("JwtSettings:ExpirationHours", 24);
    }

    /// <summary>
    /// Registers a new user account.
    /// 
    /// SECURITY STEPS:
    /// 1. Validate email is unique
    /// 2. Hash password with BCrypt (expensive by design - prevents brute force)
    /// 3. Create user record
    /// 4. Generate JWT token for immediate login
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        // Step 1: Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
            throw new InvalidOperationException("Email is already registered");
        }

        // Step 2: Create user entity
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email.ToLowerInvariant(), // Normalize email
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // HASH the password!
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Customer, // New users are customers by default
            IsActive = true
        };

        // Step 3: Save to database
        await _userRepository.AddAsync(user);
        _logger.LogInformation("User registered successfully: {UserId}", user.Id);

        // Step 4: Generate JWT token for immediate login
        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// Authenticates a user with email and password.
    /// 
    /// SECURITY STEPS:
    /// 1. Find user by email
    /// 2. Verify password hash matches (BCrypt handles this securely)
    /// 3. Check account is active
    /// 4. Generate JWT token with user claims
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Step 1: Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
        
        if (user == null)
        {
            // SECURITY: Don't reveal whether email exists (prevents user enumeration)
            _logger.LogWarning("Login failed - invalid credentials for: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Step 2: Verify password
        // BCrypt.Verify is designed to be slow (prevents brute force attacks)
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed - wrong password for: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Step 3: Check if account is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed - account disabled: {Email}", request.Email);
            throw new UnauthorizedAccessException("Account is disabled");
        }

        // Step 4: Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

        // Step 5: Generate JWT token
        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// Validates a JWT token.
    /// Used by middleware to check if incoming requests are authenticated.
    /// </summary>
    public Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        try
        {
            // Validate the token signature and expiration
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
            }, out _);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Generates a JWT token and auth response for a user.
    /// 
    /// JWT CONTAINS:
    /// - User ID (sub claim)
    /// - Email
    /// - Role (for authorization)
    /// - Expiration time
    /// - Signature (prevents tampering)
    /// </summary>
    private AuthResponse GenerateAuthResponse(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(_tokenExpirationHours);

        // Create JWT claims (data embedded in the token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
        };

        // Get signing key from configuration
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create the JWT token
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        // Serialize token to string
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogDebug("Generated JWT token for user: {UserId}, expires: {ExpiresAt}", 
            user.Id, expiresAt);

        return new AuthResponse(
            Token: tokenString,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role.ToString(),
            ExpiresAt: expiresAt
        );
    }
}
