// ============================================
// APPLICATION LAYER - Authentication Service
// ============================================
// خدمة المصادقة - تسجيل الدخول وإدارة JWT

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
using SharpCuts.Domain.Exceptions;

namespace SharpCuts.Application.Services;

/// <summary>
/// خدمة المصادقة - تتعامل مع تسجيل المستخدمين وJWT
/// 
/// خطوات الأمان:
/// 1. كلمات المرور مشفرة بـ BCrypt
/// 2. JWT موقّع بمفتاح سري
/// 3. الرموز تنتهي بعد مدة محددة
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly int _tokenExpirationHours;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
        _tokenExpirationHours = _configuration.GetValue<int>("JwtSettings:ExpirationHours", 24);
    }

    /// <summary>
    /// تسجيل مستخدم جديد
    /// </summary>
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("محاولة تسجيل: {Email}", request.Email);

        // التحقق من عدم وجود البريد مسبقاً
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new BusinessRuleException("البريد الإلكتروني مسجل مسبقاً");
        }

        // إنشاء المستخدم
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Customer,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        _logger.LogInformation("تم تسجيل المستخدم بنجاح: {UserId}", user.Id);

        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// تسجيل الدخول
    /// </summary>
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("محاولة تسجيل دخول: {Email}", request.Email);

        // البحث عن المستخدم
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غير صحيحة");
        }

        // التحقق من كلمة المرور
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غير صحيحة");
        }

        // التحقق من تفعيل الحساب
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("الحساب معطل");
        }

        // تحديث آخر تسجيل دخول
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("تم تسجيل الدخول بنجاح: {UserId}", user.Id);

        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// التحقق من صحة JWT
    /// </summary>
    public Task<bool> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// إنشاء JWT واستجابة المصادقة
    /// </summary>
    private AuthResponse GenerateAuthResponse(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(_tokenExpirationHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse(
            Token: tokenString,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role.ToString(),
            ExpiresAt: expiresAt
        );
    }
}
