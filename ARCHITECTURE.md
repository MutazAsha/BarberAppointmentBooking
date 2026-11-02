# 🏗️ دليل البنية المعمارية - SharpCuts

دليل شامل يشرح البنية المعمارية للمشروع (Clean Architecture + N-Tier + Dependency Injection)

---

## 📊 نظرة عامة على البنية

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                        │
│                    (SharpCuts.API + React)                       │
│  Controllers │ Middleware │ JWT Auth │ Swagger │ React UI        │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                       APPLICATION LAYER                          │
│                   (SharpCuts.Application)                        │
│  Use Cases │ Services │ DTOs │ Interfaces │ Validators          │
└─────────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────────┐
│                         DOMAIN LAYER                             │
│                     (SharpCuts.Domain)                           │
│  Entities │ Enums │ Value Objects │ Domain Rules │ Exceptions   │
└─────────────────────────────────────────────────────────────────┘
                              ↑
┌─────────────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                          │
│                  (SharpCuts.Infrastructure)                      │
│  EF Core │ Repositories │ DI Registration │ External Services   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎯 المبادئ الأساسية

### 1. Dependency Rule (قاعدة الاعتماد)

```
الطبقات الداخلية لا تعرف شيئاً عن الطبقات الخارجية
الطبقات الخارجية تعتمد على الطبقات الداخلية

API → Application → Domain ← Infrastructure
 ↑         ↑          ↑            ↑
يعتمد    يعتمد     لا يعتمد     يعتمد على
على      على       على شيء    Application
Application Domain              و Domain
```

### 2. Dependency Injection (حقن التبعيات)

```csharp
// ❌ طريقة قديمة: إنشاء الاعتمادات داخل الفئة
public class BookingService
{
    private AppDbContext _context = new AppDbContext(); // متصل بـ EF Core
}

// ✅ طريقة نظيفة: حقن الاعتمادات
public class BookingService
{
    private readonly IAppointmentRepository _repo;
    
    public BookingService(IAppointmentRepository repo)
    {
        _repo = repo; // يعتمد على الواجهة فقط
    }
}
```

### 3. Inversion of Control (عكس التحكم)

```csharp
// التسجيل في Program.cs
services.AddScoped<IBookingService, BookingService>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// الاستخدام في Controller
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;
    
    public BookingsController(IBookingService service)
    {
        _service = service; // DI يوفر التطبيق
    }
}
```

---

## 📁 تفاصيل الطبقات

### 1. Domain Layer (الطبقة الأساسية)

**المسؤولية:**
- تعريف الكيانات الأساسية (Entities)
- تعريف قواعد العمل (Business Rules)
- تعريف الأنواع (Enums)
- تعريف الاستثناءات (Exceptions)

**الاعتمادات:** لا يعتمد على أي طبقة أخرى (Pure C#)

**المحتوى:**
```
SharpCuts.Domain/
├── Common/
│   └── BaseEntity.cs          ← الكيان الأساسي
├── Entities/
│   ├── User.cs                ← كيان المستخدم
│   ├── Service.cs             ← كيان الخدمة
│   ├── Barber.cs              ← كيان الحلاق
│   ├── Appointment.cs         ← كيان الموعد
│   └── ContactMessage.cs      ← كيان الرسالة
├── Enums/
│   ├── AppointmentStatus.cs   ← حالات الموعد
│   ├── UserRole.cs            ← أدوار المستخدمين
│   └── ServiceCategory.cs     ← فئات الخدمات
└── Exceptions/
    ├── NotFoundException.cs   ← استثناء عدم الوجود
    ├── BusinessRuleException.cs ← استثناء انتهاك قاعدة
    └── ConflictException.cs   ← استثناء التعارض
```

**مثال على قاعدة نطاق:**
```csharp
public class Appointment : BaseEntity
{
    // قاعدة: يمكن إلغاء المواعيد المعلقة أو المؤكدة فقط
    public bool CanBeCancelled() => 
        Status == AppointmentStatus.Pending || 
        Status == AppointmentStatus.Confirmed;
    
    // قاعدة: التحقق من التعارض
    public bool ConflictsWith(Appointment other)
    {
        if (BarberId != other.BarberId) return false;
        return StartTime == other.StartTime && 
               AppointmentDate.Date == other.AppointmentDate.Date;
    }
}
```

---

### 2. Application Layer (طبقة حالات الاستخدام)

**المسؤولية:**
- تنفيذ حالات الاستخدام (Use Cases)
- تعريف واجهات المستودعات (Repository Interfaces)
- تعريف DTOs (كائنات نقل البيانات)
- تنفيذ خدمات التطبيق

**الاعتمادات:** يعتمد على Domain Layer فقط

**المحتوى:**
```
SharpCuts.Application/
├── DTOs/
│   ├── AuthDTOs.cs            ← DTOs المصادقة
│   ├── ServiceDTOs.cs         ← DTOs الخدمات
│   ├── AppointmentDTOs.cs     ← DTOs المواعيد
│   └── CommonDTOs.cs          ← DTOs عامة
├── Interfaces/
│   ├── IRepositories.cs       ← واجهات المستودعات
│   └── IServices.cs           ← واجهات الخدمات
└── Services/
    ├── AuthService.cs         ← خدمة المصادقة
    ├── BookingService.cs      ← خدمة الحجز
    ├── DashboardService.cs    ← خدمة لوحة التحكم
    └── ContactService.cs      ← خدمة الاتصال
```

**مثال على Use Case:**
```csharp
public class BookingService : IBookingService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly IBarberRepository _barberRepo;

    public async Task<AppointmentDto> CreateBookingAsync(CreateBookingRequest request)
    {
        // 1. التحقق من وجود الخدمة
        var service = await _serviceRepo.GetByIdAsync(request.ServiceId)
            ?? throw new NotFoundException("الخدمة غير موجودة");

        // 2. التحقق من وجود الحلاق
        var barber = await _barberRepo.GetByIdAsync(request.BarberId)
            ?? throw new NotFoundException("الحلاق غير موجود");

        // 3. التحقق من التعارض
        var hasConflict = await _appointmentRepo.HasConflictAsync(
            request.BarberId, request.Date, request.Time);
        
        if (hasConflict)
            throw new ConflictException("هذا الوقت محجوز مسبقاً");

        // 4. إنشاء الموعد
        var appointment = new Appointment
        {
            ServiceId = request.ServiceId,
            BarberId = request.BarberId,
            CustomerId = request.CustomerId,
            AppointmentDate = request.Date,
            StartTime = request.Time,
            Status = AppointmentStatus.Confirmed
        };

        await _appointmentRepo.AddAsync(appointment);

        // 5. إرجاع DTO
        return MapToDto(appointment);
    }
}
```

---

### 3. Infrastructure Layer (طبقة البنية التحتية)

**المسؤولية:**
- تنفيذ واجهات المستودعات (Repository Implementations)
- إعداد Entity Framework Core
- تسجيل خدمات DI
- التعامل مع الخدمات الخارجية

**الاعتمادات:** يعتمد على Application و Domain

**المحتوى:**
```
SharpCuts.Infrastructure/
├── Data/
│   └── AppDbContext.cs        ← سياق قاعدة البيانات
├── Repositories/
│   ├── Repository.cs          ← المستودع العام
│   ├── UserRepository.cs      ← مستودع المستخدمين
│   ├── AppointmentRepository.cs ← مستودع المواعيد
│   └── ...
└── DependencyInjection.cs     ← تسجيل DI
```

**مثال على Repository Implementation:**
```csharp
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    // تنفيذ الواجهة المعرفة في Application Layer
    public async Task<bool> HasConflictAsync(Guid barberId, DateTime date, string startTime)
    {
        return await _context.Appointments.AnyAsync(a => 
            a.BarberId == barberId &&
            a.AppointmentDate.Date == date.Date &&
            a.StartTime == startTime &&
            a.Status != AppointmentStatus.Cancelled);
    }

    // استخدام AsNoTracking للأداء
    public async Task<IReadOnlyList<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .ToListAsync();
    }
}
```

---

### 4. API Layer (طبقة العرض)

**المسؤولية:**
- تعريف نقاط النهاية HTTP (Endpoints)
- معالجة الطلبات والاستجابات
- المصادقة والتفويض
- Swagger documentation

**الاعتمادات:** يعتمد على Application و Infrastructure

**المحتوى:**
```
SharpCuts.API/
├── Controllers/
│   ├── AuthController.cs      ← متحكم المصادقة
│   ├── ServicesController.cs  ← متحكم الخدمات
│   ├── BookingsController.cs  ← متحكم الحجوزات
│   └── DashboardController.cs ← متحكم لوحة التحكم
├── Middleware/
│   └── ExceptionMiddleware.cs ← معالجة الاستثناءات
├── Program.cs                 ← نقطة الدخول
└── appsettings.json           ← الإعدادات
```

**مثال على Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // حماية جميع النقاط
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    // حقن التبعيات عبر Constructor
    public BookingsController(IBookingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            // الحصول على معرّف المستخدم من JWT
            var customerId = GetCurrentUserId();
            request.CustomerId = customerId;

            // استدعاء خدمة التطبيق
            var appointment = await _service.CreateBookingAsync(request);

            return Ok(new ApiResponse<AppointmentDto>(
                Success: true,
                Data: appointment,
                Message: "تم إنشاء الموعد بنجاح"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiResponse<object>(
                Success: false,
                Error: ex.Message
            ));
        }
        catch (ConflictException ex)
        {
            return Conflict(new ApiResponse<object>(
                Success: false,
                Error: ex.Message
            ));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.Parse(userIdClaim?.Value ?? throw new UnauthorizedAccessException());
    }
}
```

---

## 🔄 تدفق البيانات

### مثال: إنشاء موعد جديد

```
1. Client (React)
   ↓ POST /api/bookings
   ↓ { serviceId, barberId, date, time }

2. API Layer (Controller)
   ↓ التحقق من JWT Token
   ↓ استخراج CustomerId من Token
   ↓ استدعاء BookingService.CreateBookingAsync()

3. Application Layer (Service)
   ↓ التحقق من وجود الخدمة (IServiceRepository)
   ↓ التحقق من وجود الحلاق (IBarberRepository)
   ↓ التحقق من التعارض (IAppointmentRepository)
   ↓ إنشاء كيان Appointment
   ↓ حفظ الموعد (IAppointmentRepository.AddAsync)
   ↓ إرجاع AppointmentDto

4. Infrastructure Layer (Repository)
   ↓ تنفيذ IServiceRepository.GetByIdAsync()
   ↓ تنفيذ IBarberRepository.GetByIdAsync()
   ↓ تنفيذ IAppointmentRepository.HasConflictAsync()
   ↓ تنفيذ IAppointmentRepository.AddAsync()
   ↓ Entity Framework Core يحفظ في قاعدة البيانات

5. Database
   ↓ INSERT INTO Appointments ...

6. Response
   ↓ إرجاع 200 OK مع AppointmentDto
   ↓ Client يعرض رسالة نجاح
```

---

## 🎨 أنماط التصميم المستخدمة

### 1. Repository Pattern

```csharp
// الواجهة في Application Layer
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Appointment>> GetAllAsync();
    Task<Appointment> AddAsync(Appointment entity);
    Task<bool> HasConflictAsync(Guid barberId, DateTime date, string time);
}

// التطبيق في Infrastructure Layer
public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;
    
    public async Task<bool> HasConflictAsync(Guid barberId, DateTime date, string time)
    {
        return await _context.Appointments.AnyAsync(...);
    }
}
```

**الفوائد:**
- فصل منطق العمل عن الوصول للبيانات
- سهولة الاختبار (يمكن Mock الواجهة)
- إمكانية تغيير قاعدة البيانات دون تغيير الكود

### 2. Unit of Work Pattern

```csharp
// DbContext يدير المعاملات
public class AppDbContext : DbContext
{
    public async Task<int> SaveChangesAsync()
    {
        // حفظ جميع التغييرات في معاملة واحدة
        return await base.SaveChangesAsync();
    }
}
```

**الفوائد:**
- ضمان الاتساق (Atomic Operations)
- تقليل عدد الاتصالات بقاعدة البيانات
- التراجع عن التغييرات عند الفشل

### 3. DTO Pattern

```csharp
// DTO للعرض (بدون بيانات حساسة)
public record AppointmentDto(
    Guid Id,
    string ServiceName,
    string BarberName,
    DateTime Date,
    string Time,
    string Status
);

// Entity للبيانات الكاملة
public class Appointment : BaseEntity
{
    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }
    public Guid BarberId { get; set; }
    public Barber? Barber { get; set; }
    public Guid CustomerId { get; set; }
    public User? Customer { get; set; }
    // ... بيانات إضافية
}

// Mapper
public static AppointmentDto MapToDto(Appointment entity)
{
    return new AppointmentDto(
        Id: entity.Id,
        ServiceName: entity.Service?.Name ?? "",
        BarberName: entity.Barber?.FullName ?? "",
        Date: entity.AppointmentDate,
        Time: entity.StartTime,
        Status: entity.Status.ToString()
    );
}
```

**الفوائد:**
- التحكم في البيانات المعروضة
- منع تسريب البيانات الحساسة
- فصل API Contract عن Domain Model

### 4. Dependency Injection Pattern

```csharp
// التسجيل
services.AddScoped<IBookingService, BookingService>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// الاستخدام
public class BookingsController
{
    private readonly IBookingService _service;
    
    public BookingsController(IBookingService service)
    {
        _service = service; // DI يوفر التطبيق
    }
}
```

**الفوائد:**
- فصل الاعتمادات
- سهولة الاختبار
- مرونة في تغيير التطبيقات

---

## 🔐 الأمان

### JWT Authentication Flow

```
1. تسجيل الدخول
   Client → POST /api/auth/login
   ↓
   API → التحقق من البريد وكلمة المرور
   ↓
   API → إنشاء JWT Token
   ↓
   API → إرجاع Token للعميل

2. الطلبات المحمية
   Client → GET /api/bookings/my
   ↓
   Client → Authorization: Bearer <token>
   ↓
   API → التحقق من صحة Token
   ↓
   API → استخراج CustomerId من Token
   ↓
   API → إرجاع مواعيد العميل فقط
```

### Role-Based Authorization

```csharp
// المديرين فقط
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetDashboardStats() { }

// العملاء والحلاقين
[Authorize(Roles = "Customer,Barber")]
public async Task<IActionResult> CreateBooking() { }

// أي مستخدم مسجل
[Authorize]
public async Task<IActionResult> GetMyAppointments() { }
```

---

## ⚡ الأداء

### 1. Response Caching

```csharp
[ResponseCache(Duration = 300)] // 5 دقائق
public async Task<IActionResult> GetAllServices() { }
```

### 2. AsNoTracking

```csharp
// أسرع بـ 30-50% للقراءة فقط
return await _context.Services.AsNoTracking().ToListAsync();
```

### 3. Pagination

```csharp
var items = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 4. Indexed Queries

```csharp
// فهرس مركب للتحقق السريع
entity.HasIndex(e => new { e.BarberId, e.AppointmentDate, e.StartTime });
```

### 5. Eager Loading

```csharp
// تحميل العلاقات دفعة واحدة
return await _context.Appointments
    .Include(a => a.Service)
    .Include(a => a.Barber)
    .Include(a => a.Customer)
    .ToListAsync();
```

---

## 🧪 الاختبار

### Unit Testing مع Moq

```csharp
[Fact]
public async Task CreateBooking_WithConflict_ThrowsConflictException()
{
    // Arrange
    var mockRepo = new Mock<IAppointmentRepository>();
    mockRepo.Setup(r => r.HasConflictAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<string>()))
        .ReturnsAsync(true);

    var service = new BookingService(mockRepo.Object, ...);

    // Act & Assert
    await Assert.ThrowsAsync<ConflictException>(() => 
        service.CreateBookingAsync(new CreateBookingRequest(...)));
}
```

### Integration Testing

```csharp
[Fact]
public async Task CreateBooking_ValidRequest_ReturnsAppointment()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = await GetAuthTokenAsync();
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);

    // Act
    var response = await client.PostAsJsonAsync("/api/bookings", new { ... });

    // Assert
    response.EnsureSuccessStatusCode();
    var appointment = await response.Content.ReadAsAsync<AppointmentDto>();
    Assert.NotNull(appointment);
}
```

---

## 📚 الموارد التعليمية

### Clean Architecture
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Understanding Clean Architecture](https://www.c-sharpcorner.com/article/clean-architecture/)

### Dependency Injection
- [DI in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [Dependency Injection Principles](https://martinfowler.com/articles/injection.html)

### Design Patterns
- [Repository Pattern](https://docs.microsoft.com/azure/architecture/patterns/repository)
- [Unit of Work Pattern](https://docs.microsoft.com/azure/architecture/patterns/unit-of-work)

---

## 🎉 الخلاصة

هذا المشروع يطبق أفضل الممارسات في:

✅ **Clean Architecture** - فصل واضح للطبقات  
✅ **N-Tier Architecture** - تنظيم منطقي  
✅ **Dependency Injection** - حقن التبعيات  
✅ **Repository Pattern** - وصول نظيف للبيانات  
✅ **Unit of Work** - إدارة المعاملات  
✅ **DTO Pattern** - فصل البيانات  
✅ **Domain-Driven Design** - قواعد النطاق  
✅ **SOLID Principles** - مبادئ التصميم  
✅ **Security Best Practices** - أمان متقدم  
✅ **Performance Optimization** - أداء عالي  

**افتح `SharpCuts.sln` في Visual Studio واستمتع بالبرمجة!** 🚀

---

**تم البناء بـ ❤️ باستخدام Clean Architecture و Best Practices**
