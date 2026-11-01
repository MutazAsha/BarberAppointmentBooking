# 💈 SharpCuts - Full Stack Barber Shop Booking System

A **production-ready** full-stack application built with **React + TypeScript** frontend and **ASP.NET Core 8** backend, featuring **JWT authentication**, **layered architecture**, and **enterprise-grade security**.

![SharpCuts](https://img.shields.io/badge/SharpCuts-v2.0-amber?style=for-the-badge)
![React](https://img.shields.io/badge/React-18-blue?style=for-the-badge&logo=react)
![ASP.NET](https://img.shields.io/badge/ASP.NET-8-purple?style=for-the-badge&logo=.net)
![TypeScript](https://img.shields.io/badge/TypeScript-5-blue?style=for-the-badge&logo=typescript)
![JWT](https://img.shields.io/badge/Auth-JWT-red?style=for-the-badge)

---

## 🎯 Project Overview

SharpCuts is a complete barber shop appointment booking system with:

✅ **React Frontend** - Modern, responsive UI with TypeScript  
✅ **ASP.NET Core Backend** - Enterprise-grade API with .NET 8  
✅ **JWT Authentication** - Secure token-based auth  
✅ **Layered Architecture** - Clean separation of concerns  
✅ **Dependency Injection** - Professional DI patterns  
✅ **Entity Framework Core** - ORM with InMemory/SQL Server  
✅ **Role-Based Authorization** - Admin and Customer roles  
✅ **Performance Optimized** - Caching, async/await, pagination  
✅ **Fully Documented** - Extensive code comments  

---

## 🏗️ Architecture

### Backend (ASP.NET Core 8)

```
┌─────────────────────────────────────────┐
│         API Layer (Controllers)         │  ← HTTP endpoints, JWT validation
├─────────────────────────────────────────┤
│     Application Layer (Services)        │  ← Business logic, DTOs
├─────────────────────────────────────────┤
│       Domain Layer (Entities)           │  ← Core business objects
├─────────────────────────────────────────┤
│   Infrastructure Layer (EF Core)        │  ← Data access, repositories
└─────────────────────────────────────────┘
```

**Layers:**
1. **SharpCuts.API** - Web API controllers, middleware, authentication
2. **SharpCuts.Application** - Business services, DTOs, interfaces
3. **SharpCuts.Domain** - Entities, enums, business rules
4. **SharpCuts.Infrastructure** - EF Core, repositories, DI registration

### Frontend (React 18)

```
┌─────────────────────────────────────────┐
│           Pages (Routes)                │  ← Login, Booking, Dashboard
├─────────────────────────────────────────┤
│         Components (UI)                 │  ← Navbar, Toast, Forms
├─────────────────────────────────────────┤
│      Redux Store (State)                │  ← Global state management
├─────────────────────────────────────────┤
│      Services (API Calls)               │  ← Auth, HTTP requests
└─────────────────────────────────────────┘
```

---

## 🔐 Security Features

### JWT Authentication
- **BCrypt Password Hashing** - Passwords never stored in plain text
- **JWT Tokens** - Signed with HMAC-SHA256
- **Token Expiration** - Configurable (default: 24 hours)
- **Role-Based Access** - Customer and Admin roles
- **Protected Routes** - Frontend and backend validation

### Authorization
- `[Authorize]` attribute on protected endpoints
- `[Authorize(Roles = "Admin")]` for admin-only routes
- Users can only access their own data
- JWT claims contain user ID, email, and role

### Security Best Practices
- CORS configured for specific origins
- HTTPS redirection enabled
- Input validation on all endpoints
- SQL injection prevention (EF Core parameterized queries)
- XSS protection (React escapes output by default)

---

## 🚀 Quick Start

### Prerequisites

- **Visual Studio 2022** (or VS Code)
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **SQL Server** (optional - uses InMemory by default)

### 1. Open in Visual Studio

```bash
# Double-click SharpCuts.sln
# Or from command line:
devenv SharpCuts.sln
```

### 2. Restore Dependencies

**Backend:**
```bash
dotnet restore
```

**Frontend:**
```bash
npm install
```

### 3. Run Backend

In Visual Studio:
1. Set **SharpCuts.API** as startup project
2. Press **F5** to run
3. Backend starts on **http://localhost:5000**
4. Swagger opens at **http://localhost:5000/swagger**

Or from command line:
```bash
cd backend/SharpCuts.API
dotnet run
```

### 4. Run Frontend

```bash
npm run dev
```

Frontend starts on **http://localhost:5173**

### 5. Test the Application

**Demo Credentials:**
- **Customer:** customer@sharpcuts.com / Customer123!
- **Admin:** admin@sharpcuts.com / Admin123!

---

## 📡 API Endpoints

### Authentication (Public)
```
POST /api/auth/register    - Create account
POST /api/auth/login       - Login, get JWT token
```

### Services (Public)
```
GET  /api/services         - Get all services
GET  /api/services/{id}    - Get service by ID
GET  /api/services/category/{category} - Filter by category
```

### Barbers (Public)
```
GET  /api/barbers          - Get all barbers
GET  /api/barbers/available - Get available barbers
GET  /api/barbers/{id}     - Get barber by ID
```

### Bookings (Protected - Requires JWT)
```
POST /api/bookings         - Create appointment
GET  /api/bookings/my      - Get my appointments
PUT  /api/bookings/{id}/cancel - Cancel appointment
GET  /api/bookings/slots/{barberId}?date=YYYY-MM-DD - Get available slots
```

### Dashboard (Admin Only)
```
GET  /api/dashboard/stats  - Get statistics
GET  /api/dashboard/today  - Get today's schedule
```

### Contact (Public)
```
POST /api/contact          - Submit contact form
```

---

## 🧪 Testing the API

### Using Swagger UI

1. Navigate to **http://localhost:5000/swagger**
2. Click **"Authorize"** button
3. Enter: `Bearer YOUR_JWT_TOKEN`
4. Test protected endpoints

### Using cURL

**Login:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "customer@sharpcuts.com",
    "password": "Customer123!"
  }'
```

**Create Booking (with JWT):**
```bash
curl -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{
    "serviceId": "11111111-1111-1111-1111-111111111111",
    "barberId": "22222222-2222-2222-2222-222222222221",
    "appointmentDate": "2026-01-20",
    "startTime": "14:30"
  }'
```

---

## 📂 Project Structure

```
SharpCuts/
├── SharpCuts.sln                    # Visual Studio solution
│
├── backend/
│   ├── SharpCuts.API/               # Web API layer
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs    # Login/Register
│   │   │   ├── ResourceControllers.cs # Services, Barbers, Contact
│   │   │   └── ProtectedControllers.cs # Bookings, Dashboard
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── Program.cs               # Entry point, DI setup
│   │   ├── appsettings.json         # Configuration
│   │   └── SharpCuts.API.csproj
│   │
│   ├── SharpCuts.Application/       # Business logic layer
│   │   ├── DTOs/
│   │   │   └── DTOs.cs              # Data transfer objects
│   │   ├── Interfaces/
│   │   │   ├── IRepositories.cs     # Repository contracts
│   │   │   └── IServices.cs         # Service contracts
│   │   ├── Services/
│   │   │   ├── AuthService.cs       # JWT authentication
│   │   │   └── BusinessServices.cs  # Booking, Dashboard, etc.
│   │   └── SharpCuts.Application.csproj
│   │
│   ├── SharpCuts.Domain/            # Domain layer
│   │   ├── Entities/
│   │   │   └── Entities.cs          # User, Service, Barber, Appointment
│   │   ├── Enums/
│   │   │   └── Enums.cs             # AppointmentStatus, UserRole
│   │   └── SharpCuts.Domain.csproj
│   │
│   └── SharpCuts.Infrastructure/    # Data access layer
│       ├── Data/
│       │   └── AppDbContext.cs      # EF Core context, seed data
│       ├── Repositories/
│       │   └── Repositories.cs      # Repository implementations
│       ├── DependencyInjection.cs   # Service registration
│       └── SharpCuts.Infrastructure.csproj
│
├── src/                             # React frontend
│   ├── components/
│   │   ├── Navbar.tsx               # Navigation with auth
│   │   ├── Footer.tsx
│   │   ├── Toast.tsx                # Notification system
│   │   └── LoadingSpinner.tsx
│   ├── pages/
│   │   ├── HomePage.tsx
│   │   ├── ServicesPage.tsx
│   │   ├── BookingPage.tsx          # Protected
│   │   ├── AppointmentsPage.tsx     # Protected
│   │   ├── DashboardPage.tsx        # Admin only
│   │   ├── ContactPage.tsx
│   │   ├── LoginPage.tsx
│   │   └── RegisterPage.tsx
│   ├── services/
│   │   ├── api.ts                   # Public API calls
│   │   └── authApi.ts               # Auth + protected API calls
│   ├── store/
│   │   ├── index.ts                 # Redux store
│   │   ├── bookingSlice.ts
│   │   ├── appointmentSlice.ts
│   │   └── uiSlice.ts
│   ├── types/
│   │   └── index.ts
│   ├── App.tsx                      # Routes, protected routes
│   └── main.tsx
│
├── .vscode/                         # VS Code configuration
├── .env                             # Environment variables
├── package.json                     # Frontend dependencies
└── README.md                        # This file
```

---

## 🔑 Authentication Flow

### 1. Registration
```
User → POST /api/auth/register
     → Password hashed with BCrypt
     → User saved to database
     → JWT token generated
     → Token returned to client
```

### 2. Login
```
User → POST /api/auth/login
     → Password verified against hash
     → JWT token generated with claims (userId, email, role)
     → Token returned to client
     → Client stores token in localStorage
```

### 3. Protected Request
```
Client → GET /api/bookings/my
       → Authorization: Bearer <token>
       → Backend validates token signature
       → Backend checks token expiration
       → Backend extracts userId from claims
       → Returns user's appointments
```

### 4. Token Expiration
```
Token expires after 24 hours (configurable)
Client receives 401 Unauthorized
Client redirects to login page
User must login again
```

---

## ⚡ Performance Optimizations

### Backend
- **Response Caching** - `[ResponseCache(Duration = 300)]` on public endpoints
- **Async/Await** - All database operations are non-blocking
- **AsNoTracking()** - Read-only queries skip change tracking
- **Pagination** - Large datasets use Skip/Take
- **Indexed Queries** - Composite indexes for conflict checking
- **Eager Loading** - `.Include()` prevents N+1 queries

### Frontend
- **Code Splitting** - Routes load on demand
- **Memoization** - `useMemo` and `useCallback` prevent re-renders
- **Debouncing** - Search inputs debounce API calls
- **Optimistic Updates** - UI updates before API confirms

---

## 🗄️ Database

### Default: InMemory Database
- No setup required
- Data resets on restart
- Perfect for development and testing

### Production: SQL Server
Change in `Program.cs`:
```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
```

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=SharpCutsDb;..."
  }
}
```

### Seed Data
The database automatically seeds:
- 2 users (admin + customer)
- 8 services
- 5 barbers

---

## 🎨 Frontend Features

### Authentication UI
- Login page with form validation
- Register page with password confirmation
- Protected routes redirect to login
- Navbar shows user info and logout button
- Admin-only routes check role

### Booking Flow
1. Select service (from API)
2. Choose barber (from API)
3. Pick date and time (real-time availability)
4. Enter details and confirm
5. Appointment saved to database

### Dashboard (Admin)
- Total appointments count
- Today's schedule
- Revenue tracking
- Service popularity
- Barber performance

---

## 📚 Code Quality

### Comments
Every file has:
- File-level documentation
- Class/method XML comments
- Inline explanations for complex logic
- Security notes for auth code

### Clean Code Principles
- Single Responsibility Principle
- Dependency Injection throughout
- Interface segregation
- DRY (Don't Repeat Yourself)
- Meaningful naming

### Error Handling
- Global exception handling
- User-friendly error messages
- Logging with ILogger
- Try-catch blocks with proper context

---

## 🚀 Deployment

### Backend (Azure App Service)
```bash
cd backend/SharpCuts.API
dotnet publish -c Release -o ./publish
# Upload to Azure App Service
```

### Frontend (Vercel/Netlify)
```bash
npm run build
# Deploy dist/ folder
```

### Environment Variables

**Backend (appsettings.Production.json):**
```json
{
  "JwtSettings": {
    "SecretKey": "YourProductionSecretKeyAtLeast32Chars!",
    "Issuer": "SharpCutsAPI",
    "Audience": "SharpCutsClient",
    "ExpirationHours": 24
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;..."
  }
}
```

**Frontend (.env.production):**
```
VITE_API_URL=https://your-api.azurewebsites.net/api
```

---

## 🧰 Development Tools

### Visual Studio 2022
- Open `SharpCuts.sln`
- F5 to debug backend
- Swagger UI for API testing
- Breakpoints in C# code

### VS Code
- Open folder in VS Code
- Install recommended extensions
- Run tasks from Command Palette
- Debug with launch.json

### Swagger UI
- Interactive API documentation
- Test endpoints directly
- Authenticate with JWT tokens
- View request/response schemas

---

## 📖 Learning Resources

### ASP.NET Core
- [Microsoft Docs](https://docs.microsoft.com/aspnet/core)
- [JWT Authentication](https://docs.microsoft.com/aspnet/core/security/authentication/jwt-authentication)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

### React
- [React Docs](https://react.dev/)
- [Redux Toolkit](https://redux-toolkit.js.org/)
- [React Router](https://reactrouter.com/)

### Security
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)
- [BCrypt](https://github.com/BcryptNet/bcrypt.net)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

---

## ✅ Checklist

After setup, verify:

- [ ] Backend runs on port 5000
- [ ] Frontend runs on port 5173
- [ ] Swagger UI accessible
- [ ] Can register new user
- [ ] Can login with demo credentials
- [ ] JWT token received
- [ ] Can create booking (authenticated)
- [ ] Can view my appointments
- [ ] Admin can access dashboard
- [ ] Customer cannot access dashboard (403)
- [ ] Token expires after 24 hours
- [ ] CORS works for frontend

---

## 🎓 Key Concepts Demonstrated

### Backend
✅ **Layered Architecture** - Clean separation  
✅ **Dependency Injection** - Constructor injection  
✅ **Repository Pattern** - Data access abstraction  
✅ **Unit of Work** - DbContext manages transactions  
✅ **DTO Pattern** - Separate API contracts  
✅ **JWT Authentication** - Token-based security  
✅ **Role-Based Authorization** - Admin vs Customer  
✅ **BCrypt Hashing** - Secure password storage  
✅ **Entity Framework Core** - ORM with migrations  
✅ **Async/Await** - Non-blocking I/O  
✅ **Response Caching** - Performance optimization  
✅ **Global Exception Handling** - Consistent errors  

### Frontend
✅ **React Hooks** - useState, useEffect, useMemo  
✅ **Redux Toolkit** - Global state management  
✅ **React Router** - Client-side routing  
✅ **Protected Routes** - Auth-based navigation  
✅ **JWT Token Management** - localStorage + axios  
✅ **TypeScript** - Type safety  
✅ **Tailwind CSS** - Utility-first styling  
✅ **Responsive Design** - Mobile-first  
✅ **Toast Notifications** - User feedback  
✅ **Form Validation** - Client-side checks  

---

## 🤝 Contributing

This is a production-ready template. Feel free to:
- Add features
- Fix bugs
- Improve documentation
- Submit pull requests

---

## 📝 License

MIT License - Free to use for personal and commercial projects.

---

## 🎉 Summary

You now have a **complete, production-ready** full-stack application with:

✅ **ASP.NET Core 8 Backend** - Enterprise-grade API  
✅ **React 18 Frontend** - Modern, responsive UI  
✅ **JWT Authentication** - Secure token-based auth  
✅ **Layered Architecture** - Professional patterns  
✅ **Entity Framework Core** - Database ORM  
✅ **Role-Based Authorization** - Admin and Customer  
✅ **Performance Optimized** - Caching, async, pagination  
✅ **Fully Documented** - Extensive comments  
✅ **Visual Studio Ready** - Open and run  

**Open `SharpCuts.sln` in Visual Studio and press F5!** 🚀

---

**Built with ❤️ using ASP.NET Core, React, and TypeScript**
