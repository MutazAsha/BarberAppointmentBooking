using Microsoft.EntityFrameworkCore;
using SharpCuts.Application.Services;
using SharpCuts.Domain.Interfaces;
using SharpCuts.Infrastructure.Data;
using SharpCuts.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with InMemory database (for development)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("SharpCutsDB"));

// Register repositories
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IBarberRepository, BarberRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Register services
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<BarberService>();
builder.Services.AddScoped<ServiceManagementService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<DashboardService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
