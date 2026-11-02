# 💈 SharpCuts - Clean Architecture & N-Tier Project

مشروع كامل لصالون حلاقة ببنية معمارية نظيفة (Clean Architecture) وبنية متعددة الطبقات (N-Tier Architecture) مع حقن التبعيات (Dependency Injection).

---

## 🏗️ البنية المعمارية

### Clean Architecture (البنية النظيفة)

```
┌─────────────────────────────────────────────────────────┐
│                    API Layer (Presentation)             │
│              Controllers, Middleware, Config            │
├─────────────────────────────────────────────────────────┤
│              Application Layer (Use Cases)              │
│         Services, DTOs, Interfaces, Validators          │
├─────────────────────────────────────────────────────────┤
│                 Domain Layer (Core)                     │
│         Entities, Enums, Value Objects, Rules           │
├─────────────────────────────────────────────────────────┤
│             Infrastructure Layer (Data)                 │
│      EF Core, Repositories, External Services           │
└─────────────────────────────────────────────────────────┘
```

### N-Tier Architecture (البنية متعددة الطبقات)

```
┌─────────────────────────────────────────────────────────┐
│              Presentation Tier (UI)                     │
│           React Frontend + ASP.NET API                  │
├─────────────────────────────────────────────────────────┤
│            Business Logic Tier                          │
│         Application Services + Domain Logic             │
├─────────────────────────────────────────────────────────┤
│             Data Access Tier                            │
│        Repositories + Entity Framework Core             │
├─────────────────────────────────────────────────────────┤
│               Database Tier                             │
│         SQL Server / InMemory Database                  │
└─────────────────────────────────────────────────────────┘
```

---

## 📂 هيكل المشروع

```
SharpCuts/
│
├── src/
│   │
│   ├── SharpCuts.Domain/              ← الطبقة الأساسية (Core)
│   │   ├── Common/
│   │   │   └── BaseEntity.cs          ← الكيان الأساسي
│   │   ├── Entities/
│   │   │   └── Entities.cs            ← User, Service, Barber, Appointment
│   │   ├── Enums/
│   │   │   └── Enums.cs               ← AppointmentStatus, UserRole, ServiceCategory
│   │   ├── Exceptions/
│   │   │   └── DomainExceptions.cs    ← استثناءات النطاق
│   │   └── SharpCuts.Domain.csproj
│   │
│   ├── SharpCuts.Application/         ← طبقة حالات الاستخدام
│   │   ├── DTOs/
│   │   │   └── DTOs.cs                ← كائنات نقل البيانات
│   │   ├── Interfaces/
│   │   │   ├── IRepositories.cs       ← واجهات المستودعات
│   │   │   └── IServices.cs           ← واجهات الخدمات
│   │   ├── Services/
│   │   │   ├── AuthService.cs         ← خدمة المصادقة
│   │   │   └── BusinessServices.cs    ← خدمات العمل
│   │   └── SharpCuts.Application.csproj
│   │
│   ├── SharpCuts.Infrastructure/      ← طبقة البنية التحتية
│   │   ├── Data/
│   │   │   └── AppDbContext.cs        ← سياق قاعدة البيانات
│   │   ├── Repositories/
│   │   │   └── Repositories.cs        ← تطبيقات المستودعات
│   │   ├── DependencyInjection.cs     ← تسجيل DI
│   │   └── SharpCuts.Infrastructure.csproj
│   │
│   └── SharpCuts.API/                 ← طبقة العرض (API)
│       ├── Controllers/
│       │   └── Controllers.cs         ← المتحكمات
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Program.cs                 ← نقطة الدخول
│       ├── appsettings.json           ← الإعدادات
│       └── SharpCuts.API.csproj
│
├── src/                               ← React Frontend
│   ├── components/
│   ├── pages/
│   ├── services/
│   ├── store/
│   └── App.tsx
│
└── SharpCuts.sln                      ← Solution File
```

---

## 🎯 مبادئ التصميم

### 1. Dependency Inversion Principle (DIP)

```csharp
// ❌ خطأ: الاعتماد على التطبيق
public class BookingService
{
    private readonly AppDbContext _context; // يعتمد على EF Core مباشرة
}

// ✅ صحيح: الاعتماد على التجريد
public class BookingService
{
    private readonly IAppointmentRepository _repository; // يعتمد على الواجهة
}
```

### 2. Single Responsibility Principle (SRP)

```csharp
// كل فئة لها مسؤولية واحدة
public class AuthService { /* المصادقة فقط */ }
public class BookingService { /* الحجوزات فقط */ }
public class UserRepository { /* الوصول للبيانات فقط */ }
```

### 3. Interface Segregation Principle (ISP)

```csharp
// واجهات صغيرة ومخصصة
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}
```

### 4. Dependency Injection (DI)

```csharp
// حقن التبعيات عبر Constructor
public class BookingService : IBookingService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IServiceRepository _serviceRepo;
    private readonly IBarberRepository _barberRepo;

    public BookingService(
        IAppointmentRepository appointmentRepo,
        IServiceRepository serviceRepo,
        IBarberRepository barberRepo)
    {
        _appointmentRepo = appointmentRepo;
        _serviceRepo = serviceRepo;
        _barberRepo = barberRepo;
    }
}
```

---

## 🔐 الأمان

### JWT Authentication

```csharp
// 1. تسجيل المستخدم
var user = new User
{
    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password) // تشفير
};

// 2. تسجيل الدخول
if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
{
    var token = GenerateJwtToken(user); // إنشاء JWT
}

// 3. التحقق من الرمز
[Authorize] // حماية النقاط
public async Task<IActionResult> CreateBooking() { }
```

### Role-Based Authorization

```csharp
[Authorize(Roles = "Admin")] // المديرين فقط
public async Task<IActionResult> GetDashboardStats() { }
```

---

## ⚡ الأداء

### 1. Response Caching

```csharp
[ResponseCache(Duration = 300)] // تخزين 5 دقائق
public async Task<IActionResult> GetAllServices() { }
```

### 2. AsNoTracking

```csharp
// أسرع للقراءة فقط
return await _dbSet.AsNoTracking().ToListAsync();
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
// فهرس مركب للتحقق السريع من التعارض
entity.HasIndex(e => new { e.BarberId, e.AppointmentDate, e.StartTime });
```

---

## 🚀 التشغيل

### 1. فتح في Visual Studio

```bash
# انقر مرتين على SharpCuts.sln
```

### 2. استعادة الحزم

```bash
dotnet restore
```

### 3. تشغيل Backend

```bash
cd src/SharpCuts.API
dotnet run
```

### 4. تشغيل Frontend

```bash
npm install
npm run dev
```

### 5. بيانات الدخول

**المدير:**
- Email: `admin@sharpcuts.com`
- Password: `Admin123!`

**العميل:**
- Email: `customer@sharpcuts.com`
- Password: `Customer123!`

---

## 📡 API Endpoints

### Public (بدون مصادقة)

```
POST /api/auth/register     - تسجيل جديد
POST /api/auth/login        - تسجيل دخول
GET  /api/services          - جميع الخدمات
GET  /api/barbers           - جميع الحلاقين
POST /api/contact           - إرسال رسالة
```

### Protected (يتطلب JWT)

```
POST /api/bookings          - إنشاء موعد
GET  /api/bookings/my       - مواعيدي
PUT  /api/bookings/{id}/cancel - إلغاء موعد
GET  /api/bookings/slots/{barberId} - الفترات المتاحة
```

### Admin Only (المديرين فقط)

```
GET /api/dashboard/stats    - الإحصائيات
GET /api/dashboard/today    - جدول اليوم
```

---

## 🎓 المفاهيم المتقدمة

### 1. Repository Pattern

```csharp
// الواجهة في Application Layer
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<bool> HasConflictAsync(Guid barberId, DateTime date, string startTime);
}

// التطبيق في Infrastructure Layer
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public async Task<bool> HasConflictAsync(Guid barberId, DateTime date, string startTime)
    {
        return await _dbSet.AnyAsync(a => 
            a.BarberId == barberId &&
            a.AppointmentDate.Date == date.Date &&
            a.StartTime == startTime);
    }
}
```

### 2. Unit of Work Pattern

```csharp
// DbContext يدير المعاملات
public async Task<Appointment> AddAsync(Appointment entity)
{
    await _dbSet.AddAsync(entity);
    await _context.SaveChangesAsync(); // حفظ جميع التغييرات
    return entity;
}
```

### 3. DTO Pattern

```csharp
// DTO للعرض (بدون بيانات حساسة)
public record ServiceDto(
    Guid Id,
    string Name,
    decimal Price,
    int DurationMinutes
);

// Entity للبيانات الكاملة
public class Service : BaseEntity
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; } // لا يُرسل للعميل
}
```

### 4. Domain-Driven Design (DDD)

```csharp
// قواعد النطاق في الكيان
public class Appointment : BaseEntity
{
    public bool CanBeCancelled() => 
        Status == AppointmentStatus.Pending || 
        Status == AppointmentStatus.Confirmed;

    public bool ConflictsWith(Appointment other)
    {
        if (BarberId != other.BarberId) return false;
        return StartTime == other.StartTime && 
               AppointmentDate.Date == other.AppointmentDate.Date;
    }
}
```

---

## ✅ قائمة التحقق

بعد التشغيل، تحقق من:

- [ ] Backend يعمل على المنفذ 5000
- [ ] Frontend يعمل على المنفذ 5173
- [ ] Swagger UI متاح
- [ ] يمكن تسجيل مستخدم جديد
- [ ] يمكن تسجيل الدخول
- [ ] JWT token يُرجع
- [ ] يمكن إنشاء موعد (محمي)
- [ ] يمكن عرض المواعيد
- [ ] المدير يمكنه الوصول للوحة التحكم
- [ ] العميل لا يمكنه الوصول للوحة التحكم (403)
- [ ] الكود موثق بشكل جيد

---

## 📚 الموارد التعليمية

### Clean Architecture
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft Architecture Guidelines](https://docs.microsoft.com/architecture/)

### Dependency Injection
- [DI in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [DI Patterns](https://martinfowler.com/articles/injection.html)

### N-Tier Architecture
- [N-Tier Architecture](https://docs.microsoft.com/azure/architecture/guide/architecture-styles/n-tier)

---

## 🎉 الملخص

لديك الآن مشروع كامل بـ:

✅ **Clean Architecture** - 4 طبقات نظيفة  
✅ **N-Tier Architecture** - فصل واضح للمسؤوليات  
✅ **Dependency Injection** - حقن التبعيات احترافي  
✅ **Repository Pattern** - وصول نظيف للبيانات  
✅ **JWT Authentication** - مصادقة آمنة  
✅ **Role-Based Authorization** - صلاحيات حسب الدور  
✅ **Entity Framework Core** - ORM احترافي  
✅ **Performance Optimized** - تخزين مؤقت، ترقيم صفحات  
✅ **Fully Documented** - تعليقات شاملة  

**افتح `SharpCuts.sln` في Visual Studio واضغط F5!** 🚀

---

**تم البناء بـ ❤️ باستخدام ASP.NET Core 8 و Clean Architecture**
