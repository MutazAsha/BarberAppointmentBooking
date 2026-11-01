# 🚀 Open SharpCuts in VS Code or Visual Studio

## 📦 Quick Start

### Option 1: Open in VS Code (Recommended)

**Method 1: Using Workspace File**
```bash
# Open the workspace file
code sharpcuts.code-workspace
```

**Method 2: Open Folder**
```bash
# Open the entire project folder
code .
```

**Method 3: From VS Code**
1. Open VS Code
2. File → Open Workspace from File...
3. Select `sharpcuts.code-workspace`

---

### Option 2: Open in Visual Studio 2022

**For Backend (.NET) Only:**
```bash
# Open the solution file
devenv backend\SharpCuts.sln
```

**Or from Visual Studio:**
1. Open Visual Studio 2022
2. File → Open → Project/Solution
3. Navigate to `backend\SharpCuts.sln`
4. Click Open

---

## 🎯 Recommended VS Code Extensions

When you open the project, VS Code will suggest installing these extensions:

### Frontend (React)
- ✅ **ESLint** - Code linting
- ✅ **Prettier** - Code formatting
- ✅ **Tailwind CSS IntelliSense** - Tailwind autocomplete
- ✅ **ES7+ React/Redux/React-Native snippets** - React snippets
- ✅ **Auto Rename Tag** - Auto rename HTML/JSX tags
- ✅ **Path Intellisense** - File path autocomplete

### Backend (.NET)
- ✅ **C#** - C# language support
- ✅ **.NET Install Tool** - .NET runtime
- ✅ **NuGet Gallery** - Package management

### General
- ✅ **GitLens** - Git supercharged
- ✅ **Material Icon Theme** - Beautiful icons
- ✅ **One Dark Pro** - Popular theme

**Install All Recommended Extensions:**
```bash
# From command palette (Ctrl+Shift+P)
> Extensions: Show Recommended Extensions
# Click "Install" on each
```

---

## 🔧 Setup Instructions

### Prerequisites

**Required:**
- Node.js 18+ ([Download](https://nodejs.org/))
- npm 9+ (comes with Node.js)
- .NET 8 SDK ([Download](https://dotnet.microsoft.com/download))

**Optional:**
- Git ([Download](https://git-scm.com/))
- SQL Server or PostgreSQL (for backend database)

### Step 1: Install Frontend Dependencies

```bash
# From project root
npm install
```

### Step 2: Install Backend Dependencies

```bash
# Restore .NET packages
cd backend
dotnet restore
```

### Step 3: Start Development Servers

**Frontend Only:**
```bash
npm run dev
# Opens at http://localhost:5173
```

**Backend Only:**
```bash
cd backend/SharpCuts.API
dotnet run
# API at http://localhost:5000
```

**Both (using VS Code tasks):**
1. Open Command Palette (Ctrl+Shift+P)
2. Type: `Tasks: Run Task`
3. Select: `Start Frontend Dev Server`
4. Repeat and select: `Start Backend (.NET)`

---

## 🐛 Debugging

### Debug Frontend (React)

**Method 1: VS Code Debugger**
1. Press `F5` or go to Run → Start Debugging
2. Select: "Launch Frontend (Vite)"
3. Chrome will open with debugger attached
4. Set breakpoints in VS Code

**Method 2: Browser DevTools**
1. Start dev server: `npm run dev`
2. Open http://localhost:5173
3. Press F12 to open DevTools
4. Use Sources tab to set breakpoints

### Debug Backend (.NET)

**Method 1: VS Code**
1. Open Command Palette (Ctrl+Shift+P)
2. Select: `Debug: Start Debugging`
3. Select: "Launch .NET Backend"
4. Set breakpoints in C# files

**Method 2: Visual Studio**
1. Open `backend/SharpCuts.sln`
2. Set `SharpCuts.API` as startup project
3. Press `F5` to start debugging
4. Set breakpoints in controllers/services

---

## 📂 Project Structure in VS Code

```
sharpcuts/
├── 📁 .vscode/              # VS Code configuration
│   ├── extensions.json      # Recommended extensions
│   ├── launch.json          # Debug configurations
│   ├── settings.json        # Workspace settings
│   └── tasks.json           # Build tasks
│
├── 📁 src/                  # Frontend (React)
│   ├── 📁 components/       # Reusable components
│   ├── 📁 pages/            # Page components
│   ├── 📁 store/            # Redux store & slices
│   ├── 📁 services/         # API services
│   ├── 📁 types/            # TypeScript types
│   ├── App.tsx              # Main app component
│   └── main.tsx             # Entry point
│
├── 📁 backend/              # Backend (.NET)
│   ├── 📁 SharpCuts.API/    # Web API layer
│   ├── 📁 SharpCuts.Application/  # Business logic
│   ├── 📁 SharpCuts.Domain/       # Domain entities
│   ├── 📁 SharpCuts.Infrastructure/ # Data access
│   ├── 📁 SharpCuts.Tests/        # Unit tests
│   └── SharpCuts.sln        # Solution file
│
├── 📁 .github/              # GitHub Actions
│   └── workflows/           # CI/CD pipelines
│
├── sharpcuts.code-workspace # VS Code workspace
├── package.json             # Frontend dependencies
├── README.md                # Project documentation
└── DEPLOYMENT.md            # Deployment guide
```

---

## ⚡ Useful VS Code Shortcuts

### General
- `Ctrl+Shift+P` - Command Palette
- `Ctrl+P` - Quick Open File
- `Ctrl+Shift+F` - Global Search
- `Ctrl+` ` - Toggle Terminal
- `Ctrl+B` - Toggle Sidebar

### Editing
- `Ctrl+D` - Select next occurrence
- `Alt+↑/↓` - Move line up/down
- `Shift+Alt+↑/↓` - Copy line up/down
- `Ctrl+/` - Toggle comment
- `Ctrl+Shift+K` - Delete line

### Navigation
- `Ctrl+G` - Go to line
- `F12` - Go to definition
- `Alt+←` - Go back
- `Ctrl+Tab` - Switch between files

### Debugging
- `F5` - Start debugging
- `F9` - Toggle breakpoint
- `F10` - Step over
- `F11` - Step into

---

## 🎨 VS Code Tasks

Access via: `Terminal → Run Task...` or `Ctrl+Shift+P → Tasks: Run Task`

### Available Tasks:

**Frontend:**
- ✅ `Install Dependencies` - npm install
- ✅ `Start Frontend Dev Server` - npm run dev
- ✅ `Build Frontend` - npm run build
- ✅ `Type Check` - TypeScript validation

**Backend:**
- ✅ `Restore .NET Packages` - dotnet restore
- ✅ `Start Backend (.NET)` - dotnet run
- ✅ `build-backend` - Build .NET project
- ✅ `Run .NET Tests` - Execute unit tests

**Maintenance:**
- ✅ `Clean All` - Remove node_modules, dist, bin, obj

---

## 🔌 API Testing

### Using REST Client Extension

Create a file `requests.http`:

```http
@baseUrl = http://localhost:5000/api

### Get all services
GET {{baseUrl}}/services

### Get available barbers
GET {{baseUrl}}/barbers/available

### Create booking
POST {{baseUrl}}/bookings
Content-Type: application/json

{
  "serviceId": "00000000-0000-0000-0000-000000000001",
  "barberId": "00000000-0000-0000-0000-000000000001",
  "date": "2026-01-20",
  "time": "14:30",
  "customerName": "John Doe",
  "customerPhone": "+1234567890",
  "customerEmail": "john@example.com"
}

### Get appointments by email
GET {{baseUrl}}/bookings/customer/john@example.com
```

Click "Send Request" above each request to execute!

---

## 🐛 Troubleshooting

### Issue: Extensions not installing
**Solution:**
```bash
# Install manually
code --install-extension dbaeumer.vscode-eslint
code --install-extension esbenp.prettier-vscode
code --install-extension bradlc.vscode-tailwindcss
```

### Issue: TypeScript errors
**Solution:**
```bash
# Reinstall TypeScript
npm install typescript --save-dev
# Restart VS Code
```

### Issue: .NET not recognized
**Solution:**
1. Install .NET 8 SDK
2. Restart VS Code
3. Install C# extension

### Issue: Port already in use
**Solution:**
```bash
# Kill process on port 5173 (frontend)
npx kill-port 5173

# Kill process on port 5000 (backend)
npx kill-port 5000
```

---

## 📚 Additional Resources

- [VS Code Documentation](https://code.visualstudio.com/docs)
- [VS Code React Tutorial](https://code.visualstudio.com/docs/nodejs/react-tutorial)
- [VS Code .NET Tutorial](https://code.visualstudio.com/docs/languages/dotnet)
- [Tailwind CSS in VS Code](https://tailwindcss.com/docs/editor-setup#vs-code)

---

## ✅ Verification Checklist

After opening in VS Code, verify:

- [ ] All recommended extensions installed
- [ ] No TypeScript errors in Problems panel
- [ ] Frontend dev server runs (`npm run dev`)
- [ ] Backend builds successfully (`dotnet build`)
- [ ] Debug configurations work (F5)
- [ ] Tasks execute properly
- [ ] Git integration working

---

## 🎉 You're All Set!

Your SharpCuts project is now configured for VS Code/Visual Studio with:

✅ IntelliSense for React, TypeScript, Tailwind, C#
✅ Debug configurations for frontend and backend
✅ Build tasks for common operations
✅ Recommended extensions
✅ Workspace settings optimized for the project

**Happy coding!** 🚀
