# 🚀 Open SharpCuts in Visual Studio

## ✅ Complete Visual Studio Solution Ready!

Your project now includes a **complete ASP.NET Core backend** with a Visual Studio solution file that you can open directly in Visual Studio 2022.

---

## 📂 Solution Structure

```
SharpCuts.sln (Visual Studio Solution)
├── 📁 backend/
│   ├── 📁 SharpCuts.API/              # ASP.NET Core Web API
│   │   ├── Controllers/
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── SharpCuts.API.csproj
│   │
│   ├── 📁 SharpCuts.Application/      # Business Logic Layer
│   │   ├── Services/
│   │   │   └── Services.cs
│   │   └── SharpCuts.Application.csproj
│   │
│   ├── 📁 SharpCuts.Domain/           # Domain Entities
│   │   ├── Entities/
│   │   │   └── Entities.cs
│   │   ├── Interfaces/
│   │   │   └── IRepositories.cs
│   │   └── SharpCuts.Domain.csproj
│   │
│   └── 📁 SharpCuts.Infrastructure/   # Data Access Layer
│       ├── Data/
│       │   └── AppDbContext.cs
│       ├── Repositories/
│       │   └── Repositories.cs
│       └── SharpCuts.Infrastructure.csproj
│
└── 📁 src/                            # React Frontend
    ├── components/
    ├── pages/
    ├── store/
    ├── services/
    ├── App.tsx
    └── main.tsx
```

---

## 🎯 How to Open in Visual Studio

### Method 1: Double-Click Solution File (Easiest)

1. **Navigate to project folder** in File Explorer
2. **Double-click** `SharpCuts.sln`
3. Visual Studio will open with all projects loaded

### Method 2: From Visual Studio

1. **Open Visual Studio 2022**
2. Click **File** → **Open** → **Project/Solution**
3. Navigate to your project folder
4. Select **SharpCuts.sln**
5. Click **Open**

### Method 3: From Command Line

```bash
# Windows
devenv SharpCuts.sln

# Or use Visual Studio's command line
"C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe" SharpCuts.sln
```

---

## 🔧 Prerequisites

### Required Software

1. **Visual Studio 2022** (Community, Professional, or Enterprise)
   - Download: https://visualstudio.microsoft.com/
   - During installation, select:
     - ✅ ASP.NET and web development
     - ✅ .NET desktop development
     - ✅ Azure development (optional)

2. **.NET 8 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation:
     ```bash
     dotnet --version
     ```

3. **Node.js 18+** (for React frontend)
   - Download: https://nodejs.org/
   - Verify installation:
     ```bash
     node --version
     npm --version
     ```

---

## 🚀 Running the Application

### Step 1: Restore NuGet Packages

In Visual Studio:
1. **Build** → **Restore NuGet Packages**
2. Wait for all packages to restore

Or from command line:
```bash
dotnet restore
```

### Step 2: Install Frontend Dependencies

Open terminal in Visual Studio:
1. **View** → **Terminal** (or press `Ctrl+` `)
2. Run:
   ```bash
   npm install
   ```

### Step 3: Set Startup Project

1. In **Solution Explorer**, right-click **SharpCuts.API**
2. Select **Set as Startup Project**
3. The project name will appear in **bold**

### Step 4: Run the Backend

1. Press **F5** or click the **green play button** (IIS Express/SharpCuts.API)
2. Backend will start on **http://localhost:5000**
3. Swagger UI will open automatically at **http://localhost:5000/swagger**

### Step 5: Run the Frontend (Separate Terminal)

Open a new terminal:
```bash
npm run dev
```

Frontend will start on **http://localhost:5173**

### Step 6: Open Browser

Visit: **http://localhost:5173**

---

## 🎨 Visual Studio Features

### Solution Explorer

You'll see:
- **SharpCuts.API** - Web API project (startup)
- **SharpCuts.Application** - Business logic
- **SharpCuts.Domain** - Domain models
- **SharpCuts.Infrastructure** - Data access
- **Frontend** - React files (solution folder)

### Debugging

1. **Set breakpoints** in C# code (click left margin)
2. **Press F5** to start debugging
3. **Hit breakpoints** when API is called
4. **Inspect variables** in Locals/Watch windows

### Swagger UI

When backend starts:
1. Navigate to **http://localhost:5000/swagger**
2. Test all API endpoints interactively
3. View request/response schemas

### IntelliSense

- ✅ C# code completion
- ✅ TypeScript support
- ✅ React/JSX support
- ✅ Tailwind CSS classes

---

## 📊 Project Configuration

### Backend (ASP.NET Core)

**Launch Settings** (`backend/SharpCuts.API/Properties/launchSettings.json`):
```json
{
  "applicationUrl": "http://localhost:5000",
  "launchUrl": "swagger"
}
```

**Database**: InMemory (for development)
- Data resets on restart
- Seed data loaded automatically
- Perfect for testing

### Frontend (React)

**Dev Server**: Port 5173
- Hot reload enabled
- Connects to backend API
- CORS configured

---

## 🔌 API Endpoints

### Test in Swagger

After starting backend, visit **http://localhost:5000/swagger**

**Available Endpoints:**

**Services:**
- `GET /api/services` - Get all services
- `GET /api/services/category/{category}` - Get by category
- `GET /api/services/{id}` - Get by ID

**Barbers:**
- `GET /api/barbers` - Get all barbers
- `GET /api/barbers/available` - Get available barbers
- `GET /api/barbers/{id}` - Get by ID
- `PUT /api/barbers/{id}/toggle` - Toggle availability

**Bookings:**
- `GET /api/bookings` - Get all appointments
- `GET /api/bookings/customer/{email}` - Get by customer
- `POST /api/bookings` - Create booking
- `PUT /api/bookings/{id}/cancel` - Cancel booking

**Contact:**
- `POST /api/contact` - Submit message
- `GET /api/contact` - Get all messages

**Dashboard:**
- `GET /api/dashboard/stats` - Get statistics

---

## 🐛 Troubleshooting

### Issue: Visual Studio won't open .sln file

**Solution:**
1. Ensure Visual Studio 2022 is installed
2. Install .NET 8 SDK
3. Right-click .sln → Open with → Visual Studio 2022

### Issue: NuGet packages won't restore

**Solution:**
```bash
# From command line
dotnet restore
```

Or in Visual Studio:
1. **Tools** → **NuGet Package Manager** → **Package Manager Settings**
2. Check package sources
3. Clear NuGet cache: **Tools** → **Options** → **NuGet Package Manager** → **Clear All Caches**

### Issue: Backend won't start

**Solution:**
1. Check **Output** window for errors
2. Verify .NET 8 SDK is installed
3. Clean solution: **Build** → **Clean Solution**
4. Rebuild: **Build** → **Rebuild Solution**

### Issue: Frontend can't connect to backend

**Solution:**
1. Ensure backend is running on port 5000
2. Check CORS settings in `Program.cs`
3. Verify `VITE_API_URL` in `.env` file
4. Restart both servers

### Issue: Database not seeding

**Solution:**
1. Check `AppDbContext.cs` has seed data
2. Verify `Program.cs` calls `dbContext.Database.EnsureCreated()`
3. Restart backend

---

## 🎯 Visual Studio Shortcuts

### General
- `Ctrl+Shift+B` - Build Solution
- `F5` - Start Debugging
- `Ctrl+F5` - Start Without Debugging
- `Shift+F5` - Stop Debugging
- `Ctrl+Shift+F` - Find in Files
- `Ctrl+` ` - Toggle Terminal

### Navigation
- `Ctrl+T` - Go to All
- `Ctrl+F12` - Go to Definition
- `Shift+F12` - Find All References
- `Ctrl+-` - Navigate Backward
- `Ctrl+Shift+-` - Navigate Forward

### Editing
- `Ctrl+K, Ctrl+C` - Comment Selection
- `Ctrl+K, Ctrl+U` - Uncomment Selection
- `Ctrl+K, Ctrl+F` - Format Selection
- `Ctrl+.` - Quick Actions

### Debugging
- `F9` - Toggle Breakpoint
- `F10` - Step Over
- `F11` - Step Into
- `Shift+F11` - Step Out

---

## 📝 Development Workflow

### 1. Make Backend Changes

1. Edit C# files in Visual Studio
2. **Build** → **Build Solution** (Ctrl+Shift+B)
3. **F5** to run and test
4. Check Swagger UI for API changes

### 2. Make Frontend Changes

1. Edit React files (TSX/TS)
2. Frontend auto-reloads (Vite)
3. Test in browser

### 3. Debug Both

1. Set breakpoints in C# code
2. Set breakpoints in TypeScript (Chrome DevTools)
3. Press F5 to start backend
4. Start frontend in separate terminal
5. Test full flow

---

## 🚀 Deployment from Visual Studio

### Publish Backend

1. Right-click **SharpCuts.API** project
2. Select **Publish**
3. Choose target:
   - **Folder** - For local deployment
   - **Azure App Service** - For cloud deployment
4. Click **Publish**

### Publish Frontend

From terminal:
```bash
npm run build
```

Deploy `dist/` folder to:
- Vercel
- Netlify
- Azure Static Web Apps

---

## ✅ Verification Checklist

After opening in Visual Studio:

- [ ] Solution opens without errors
- [ ] All 4 projects visible in Solution Explorer
- [ ] NuGet packages restored
- [ **] Can build solution (Ctrl+Shift+B)
- [ ] Can start backend (F5)
- [ ] Swagger UI opens at http://localhost:5000/swagger
- [ ] Can test API endpoints in Swagger
- [ ] Frontend starts on port 5173
- [ ] Frontend connects to backend
- [ ] Can create a booking
- [ ] Breakpoints work in C# code

---

## 🎓 Learning Resources

### ASP.NET Core
- [Microsoft Docs](https://docs.microsoft.com/aspnet/core)
- [ASP.NET Core Tutorial](https://docs.microsoft.com/learn/paths/aspnet-core-fundamentals/)

### Visual Studio
- [Visual Studio Docs](https://docs.microsoft.com/visualstudio/)
- [Visual Studio Tips](https://docs.microsoft.com/visualstudio/ide/productivity-features)

### Entity Framework Core
- [EF Core Docs](https://docs.microsoft.com/ef/core/)
- [InMemory Database](https://docs.microsoft.com/ef/core/providers/in-memory/)

---

## 🎉 You're Ready!

Your SharpCuts project is now a **complete Visual Studio solution** with:

✅ **ASP.NET Core Web API** - Professional backend
✅ **Layered Architecture** - Clean separation
✅ **Entity Framework Core** - Database ORM
✅ **Dependency Injection** - Built-in DI container
✅ **Swagger UI** - API documentation
✅ **React Frontend** - Modern UI
✅ **Full Integration** - Frontend calls backend
✅ **Debugging Support** - Breakpoints in C#
✅ **Seed Data** - Sample services & barbers

**Open the solution and start coding!** 🚀💈

---

## 📞 Quick Reference

### Open Solution
```bash
# Double-click SharpCuts.sln
# Or: devenv SharpCuts.sln
```

### Run Backend
```bash
# In Visual Studio: Press F5
# Or: dotnet run --project backend/SharpCuts.API
```

### Run Frontend
```bash
npm run dev
```

### Access
- Frontend: http://localhost:5173
- Backend: http://localhost:5000
- Swagger: http://localhost:5000/swagger

---

**Happy coding in Visual Studio!** 🎊✨
