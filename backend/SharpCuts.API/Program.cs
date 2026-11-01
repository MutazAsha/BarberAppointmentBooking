// ============================================
// API LAYER - Application Entry Point
// ============================================
// This is where the ASP.NET Core application is configured.
// It sets up:
// 1. Services (DI container)
// 2. Middleware pipeline
// 3. Authentication & Authorization
// 4. Database initialization
//
// IMPORTANT: Order matters! Middleware runs in the order it's added.

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpCuts.Infrastructure;
using SharpCuts.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. SERVICE REGISTRATION (Dependency Injection)
// ============================================

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Use camelCase for JSON (standard for JavaScript/React)
        options.JsonSerializerOptions.PropertyNamingPolicy = 
            System.Text.Json.JsonNamingPolicy.CamelCase;
        
        // Convert enums to strings (more readable in API)
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SharpCuts API",
        Version = "v1",
        Description = "Barber Shop Appointment Booking API with JWT Authentication"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        { "Bearer", Array.Empty<string>() }
    });
});

// ============================================
// 2. AUTHENTICATION (JWT Bearer)
// ============================================

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate the signing key
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            
            // Validate issuer and audience (prevents token spoofing)
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            
            // Validate expiration
            ValidateLifetime = true,
            
            // No clock skew (token expires exactly when it says)
            ClockSkew = TimeSpan.Zero
        };
    });

// Add authorization (role-based)
builder.Services.AddAuthorization();

// ============================================
// 3. CORS (Cross-Origin Resource Sharing)
// ============================================

// Allow React frontend to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",  // Vite dev server
                "http://localhost:3000"   // Create React App
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Required for JWT cookies
    });
});

// ============================================
// 4. RESPONSE CACHING (Performance)
// ============================================

builder.Services.AddResponseCaching();

// ============================================
// 5. INFRASTRUCTURE SERVICES
// ============================================

// Register all repositories and services (from Infrastructure layer)
builder.Services.AddInfrastructure(builder.Configuration);

// ============================================
// BUILD THE APPLICATION
// ============================================

var app = builder.Build();

// ============================================
// 6. MIDDLEWARE PIPELINE
// ============================================
// Order matters! Middleware runs in the order added.

// Development-only middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection (security)
app.UseHttpsRedirection();

// CORS (must be before authorization)
app.UseCors("AllowFrontend");

// Response caching (performance)
app.UseResponseCaching();

// Authentication (validates JWT tokens)
app.UseAuthentication();

// Authorization (checks roles/permissions)
app.UseAuthorization();

// Map controllers to routes
app.MapControllers();

// ============================================
// 7. DATABASE INITIALIZATION
// ============================================

// Create database and seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // EnsureCreated() creates the database if it doesn't exist
    // and applies seed data from OnModelCreating()
    dbContext.Database.EnsureCreated();
    
    Console.WriteLine("✅ Database initialized with seed data");
}

// ============================================
// 8. START THE APPLICATION
// ============================================

Console.WriteLine("\n🚀 ========================================");
Console.WriteLine("💈 SharpCuts API Server");
Console.WriteLine("🚀 ========================================");
Console.WriteLine($"📍 Server running on: http://localhost:5000");
Console.WriteLine($"📚 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine($"🔐 Login: POST /api/auth/login");
Console.WriteLine("🚀 ========================================\n");

app.Run();
