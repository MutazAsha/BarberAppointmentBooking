// ============================================
// API LAYER - Program.cs
// ============================================
// نقطة الدخول - إعداد التطبيق

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpCuts.Infrastructure;
using SharpCuts.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. تسجيل الخدمات (Dependency Injection)
// ============================================

// إضافة المتحكمات
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = 
            System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// إضافة Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SharpCuts API",
        Version = "v1",
        Description = "API لحجز مواعيد صالون الحلاقة مع مصادقة JWT"
    });

    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "أدخل 'Bearer' ثم المسافة ثم الرمز المميز",
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
// 2. المصادقة (JWT)
// ============================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ============================================
// 3. CORS
// ============================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ============================================
// 4. التخزين المؤقت
// ============================================

builder.Services.AddResponseCaching();

// ============================================
// 5. خدمات البنية التحتية
// ============================================

builder.Services.AddInfrastructure(builder.Configuration);

// ============================================
// بناء التطبيق
// ============================================

var app = builder.Build();

// ============================================
// 6. خط البرامج الوسيطة
// ============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ============================================
// 7. تهيئة قاعدة البيانات
// ============================================

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    Console.WriteLine("✅ تم تهيئة قاعدة البيانات");
}

// ============================================
// 8. بدء التطبيق
// ============================================

Console.WriteLine("\n🚀 ========================================");
Console.WriteLine("💈 SharpCuts API Server");
Console.WriteLine("🚀 ========================================");
Console.WriteLine($"📍 الخادم يعمل على: http://localhost:5000");
Console.WriteLine($"📚 Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("🚀 ========================================\n");

app.Run();
