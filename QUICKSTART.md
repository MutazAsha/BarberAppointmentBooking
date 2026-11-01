# 🚀 Quick Start Guide - Run SharpCuts with Backend

## ⚡ Fastest Way to Run

### Option 1: Using Start Script (Easiest)

**Mac/Linux:**
```bash
chmod +x start.sh
./start.sh
```

**Windows:**
```bash
start.bat
```

This will automatically:
- Install all dependencies
- Start backend on port 5000
- Start frontend on port 5173
- Open both in separate terminals

---

### Option 2: Manual Start (Recommended for Development)

#### Step 1: Install Dependencies

```bash
# Frontend
npm install

# Backend
cd server
npm install
cd ..
```

#### Step 2: Start Backend (Terminal 1)

```bash
cd server
npm run dev
```

You should see:
```
🚀 ========================================
💈 SharpCuts API Server
🚀 ========================================
📍 Server running on: http://localhost:5000
🔌 API endpoints: http://localhost:5000/api
✅ Health check: http://localhost:5000/api/health
🚀 ========================================
```

#### Step 3: Start Frontend (Terminal 2)

```bash
npm run dev
```

You should see:
```
  VITE v6.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  Network: use --host to expose
```

#### Step 4: Open Browser

Visit: **http://localhost:5173**

---

## 🎯 VS Code Setup

### Open in VS Code

```bash
code sharpcuts.code-workspace
```

Or open the folder:
```bash
code .
```

### Install Recommended Extensions

When prompted, install all recommended extensions:
- ESLint
- Prettier
- Tailwind CSS IntelliSense
- C# (for .NET reference)
- ES7+ React snippets

### Run Using VS Code Tasks

1. Press `Ctrl+Shift+P` (Command Palette)
2. Type: `Tasks: Run Task`
3. Select: `Start Frontend Dev Server`
4. Repeat and select: `Start Backend (.NET)` (or create custom task)

---

## 🧪 Testing the Application

### 1. Test Home Page
- Visit http://localhost:5173
- Verify services load dynamically
- Check barber availability shows

### 2. Test Booking Flow
1. Click "Book Appointment"
2. Select a service
3. Choose a barber
4. Pick a date and time
5. Fill in your details
6. Confirm booking
7. Check success toast appears

### 3. Test Appointments
1. Go to "My Appointments"
2. Verify your booking appears
3. Try canceling an appointment
4. Check status updates

### 4. Test Dashboard
1. Go to "Dashboard"
2. Check stats update
3. View today's schedule
4. Check service popularity

### 5. Test API Directly

Open browser and visit:
```
http://localhost:5000/api/health
http://localhost:5000/api/services
http://localhost:5000/api/barbers
http://localhost:5000/api/bookings
```

---

## 🔧 Common Issues

### Issue: Backend won't start

**Solution:**
```bash
cd server
rm -rf node_modules dist data
npm install
npm run dev
```

### Issue: Frontend can't connect to backend

**Solution:**
1. Check backend is running on port 5000
2. Open http://localhost:5000/api/health in browser
3. Check `.env` file has `VITE_API_URL=http://localhost:5000/api`
4. Restart frontend after changing `.env`

### Issue: Port already in use

**Solution:**
```bash
# Kill process on port 5000
npx kill-port 5000

# Kill process on port 5173
npx kill-port 5173
```

### Issue: Data not persisting

**Solution:**
- Check `server/data/` directory exists
- Verify you have write permissions
- Restart backend server

---

## 📊 What's Running

### Backend (Port 5000)
- Express server with TypeScript
- RESTful API endpoints
- JSON file storage
- Dependency injection container
- CORS enabled for frontend

### Frontend (Port 5173)
- React 18 with TypeScript
- Redux state management
- Tailwind CSS styling
- Connected to backend API
- Hot reload enabled

---

## 🎨 Development Workflow

### Making Backend Changes

1. Edit files in `server/src/`
2. Server auto-reloads (ts-node-dev)
3. Test API with browser or Postman
4. Check terminal for errors

### Making Frontend Changes

1. Edit files in `src/`
2. Vite auto-reloads
3. Browser updates instantly
4. Check console for errors

### Testing API Changes

**Using cURL:**
```bash
# Get services
curl http://localhost:5000/api/services

# Create booking
curl -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "serviceId": "1",
    "barberId": "1",
    "date": "2026-01-20",
    "time": "14:30",
    "customerName": "Test User",
    "customerPhone": "+1234567890",
    "customerEmail": "test@example.com"
  }'
```

**Using Postman:**
- Import endpoints from documentation
- Test each endpoint
- Verify responses

---

## 📁 Data Management

### View Data Files

Data is stored in `server/data/`:
```bash
ls server/data/
# appointments.json  barbers.json  messages.json  services.json
```

### Reset Data

```bash
cd server
rm -rf data/
npm run dev
# Recreates with seed data
```

### Backup Data

```bash
cp -r server/data server/data-backup
```

---

## 🚀 Next Steps

### 1. Explore the Code
- Read `server/src/index.ts` - Server setup
- Read `server/src/api/routes.ts` - API endpoints
- Read `server/src/application/services.ts` - Business logic
- Read `src/services/api.ts` - Frontend API calls

### 2. Customize
- Add new services in `server/data/services.json`
- Add new barbers in `server/data/barbers.json`
- Modify UI in `src/pages/`
- Add new API endpoints in `server/src/api/routes.ts`

### 3. Deploy
See `DEPLOYMENT.md` for:
- Frontend deployment (Vercel/Netlify)
- Backend deployment (Railway/Render)
- Database migration (PostgreSQL)

---

## ✅ Verification Checklist

After starting both servers, verify:

- [ ] Backend running on http://localhost:5000
- [ ] Frontend running on http://localhost:5173
- [ ] API health check works
- [ ] Frontend loads services from API
- [ ] Can create a booking
- [ ] Booking appears in appointments
- [ ] Dashboard shows stats
- [ ] Can cancel appointments
- [ ] Contact form submits
- [ ] Data persists in server/data/

---

## 📚 Documentation

- **Full Stack Setup**: `FULLSTACK_SETUP.md`
- **API Documentation**: See README.md API section
- **Architecture**: See code comments
- **VS Code Setup**: `OPEN_IN_VSCODE.md`

---

## 🎉 You're Ready!

Your full-stack SharpCuts application is now running with:

✅ **Real Backend API** - Node.js/Express with TypeScript
✅ **Layered Architecture** - Professional patterns
✅ **Dependency Injection** - Clean code structure
✅ **Repository Pattern** - Data access abstraction
✅ **Connected Frontend** - React calls real API
✅ **Persistent Storage** - JSON file database
✅ **Hot Reload** - Both servers auto-reload

**Happy coding!** 🚀💈
