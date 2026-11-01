# 🚀 SharpCuts - Full Stack Setup Guide

## 📋 Overview

SharpCuts is a **full-stack** barber shop appointment booking system with:

- **Frontend**: React + Redux + Tailwind CSS (TypeScript)
- **Backend**: Node.js + Express (TypeScript)
- **Architecture**: Layered (API → Services → Repositories → Data)
- **Pattern**: Dependency Injection + Repository Pattern

---

## 🏗️ Project Structure

```
sharpcuts/
├── 📁 src/                    # Frontend (React)
│   ├── components/            # UI components
│   ├── pages/                 # Page components
│   ├── store/                 # Redux store
│   ├── services/              # API service
│   └── types/                 # TypeScript types
│
├── 📁 server/                 # Backend (Node.js/Express)
│   ├── src/
│   │   ├── api/              # Routes/Controllers
│   │   ├── application/      # Business logic services
│   │   ├── domain/           # Entities, interfaces, DTOs
│   │   ├── infrastructure/   # Repositories, DI container, data
│   │   └── index.ts          # Server entry point
│   ├── data/                 # JSON data storage
│   └── package.json
│
├── 📁 backend/                # .NET Backend (Reference)
│   └── SharpCuts.sln         # .NET solution
│
└── package.json               # Frontend dependencies
```

---

## ⚡ Quick Start

### 1. Install Frontend Dependencies

```bash
npm install
```

### 2. Install Backend Dependencies

```bash
cd server
npm install
cd ..
```

### 3. Start Backend Server

```bash
cd server
npm run dev
```

Backend will start at: **http://localhost:5000**

### 4. Start Frontend (New Terminal)

```bash
npm run dev
```

Frontend will start at: **http://localhost:5173**

### 5. Open in Browser

Visit: **http://localhost:5173**

---

## 🔧 Running Both Servers

### Option 1: Separate Terminals

**Terminal 1 - Backend:**
```bash
cd server
npm run dev
```

**Terminal 2 - Frontend:**
```bash
npm run dev
```

### Option 2: VS Code Tasks

1. Open Command Palette (`Ctrl+Shift+P`)
2. Run Task: `Start Backend (.NET)` or create custom task
3. Run Task: `Start Frontend Dev Server`

### Option 3: Combined Script (Recommended)

Add to root `package.json`:
```json
{
  "scripts": {
    "dev:all": "concurrently \"npm run dev\" \"cd server && npm run dev\""
  }
}
```

Install concurrently:
```bash
npm install -D concurrently
```

Then run:
```bash
npm run dev:all
```

---

## 🌐 API Endpoints

### Services
- `GET /api/services` - Get all services
- `GET /api/services/category/:category` - Get by category
- `GET /api/services/:id` - Get by ID

### Barbers
- `GET /api/barbers` - Get all barbers
- `GET /api/barbers/available` - Get available barbers
- `GET /api/barbers/:id` - Get by ID
- `PUT /api/barbers/:id/toggle` - Toggle availability

### Bookings
- `GET /api/bookings` - Get all appointments
- `GET /api/bookings/customer/:email` - Get by customer
- `GET /api/bookings/barber/:id` - Get by barber
- `GET /api/bookings/date/:date` - Get by date
- `GET /api/bookings/slots/:barberId/:date` - Get time slots
- `POST /api/bookings` - Create booking
- `PUT /api/bookings/:id/cancel` - Cancel booking

### Contact
- `POST /api/contact` - Submit message
- `GET /api/contact` - Get all messages

### Dashboard
- `GET /api/dashboard/stats` - Get statistics
- `GET /api/dashboard/today` - Get today's schedule

### Health
- `GET /api/health` - Health check

---

## 🧪 Testing the API

### Using cURL

```bash
# Get all services
curl http://localhost:5000/api/services

# Create a booking
curl -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "serviceId": "1",
    "barberId": "1",
    "date": "2026-01-20",
    "time": "14:30",
    "customerName": "John Doe",
    "customerPhone": "+1234567890",
    "customerEmail": "john@example.com"
  }'

# Get appointments
curl http://localhost:5000/api/bookings
```

### Using Postman

Import the API endpoints and test interactively.

### Using REST Client (VS Code Extension)

Create `requests.http`:
```http
@baseUrl = http://localhost:5000/api

### Get services
GET {{baseUrl}}/services

### Create booking
POST {{baseUrl}}/bookings
Content-Type: application/json

{
  "serviceId": "1",
  "barberId": "1",
  "date": "2026-01-20",
  "time": "14:30",
  "customerName": "John Doe",
  "customerPhone": "+1234567890",
  "customerEmail": "john@example.com"
}
```

---

## 🗄️ Data Storage

The backend uses **JSON file storage** in `server/data/`:

- `services.json` - Service catalog
- `barbers.json` - Barber profiles
- `appointments.json` - All appointments
- `messages.json` - Contact messages

**To reset data:**
```bash
cd server
rm -rf data/
npm run dev  # Will recreate with seed data
```

---

## 🏛️ Architecture Details

### Layered Architecture

```
┌─────────────────────────────────────┐
│         API Layer (Routes)          │  ← HTTP endpoints
├─────────────────────────────────────┤
│    Application Layer (Services)     │  ← Business logic
├─────────────────────────────────────┤
│      Domain Layer (Entities)        │  ← Core models
├─────────────────────────────────────┤
│  Infrastructure Layer (Repos/DI)    │  ← Data access
└─────────────────────────────────────┘
```

### Dependency Injection

```typescript
// Container registers all dependencies
const container = Container.getInstance();

// Services receive repositories via constructor
class BookingService {
  constructor(
    private appointmentRepo: IAppointmentRepository,
    private serviceRepo: IServiceRepository,
    private barberRepo: IBarberRepository
  ) {}
}
```

### Repository Pattern

```typescript
// Interface defines contract
interface IAppointmentRepository {
  getAll(): Promise<Appointment[]>;
  getById(id: string): Promise<Appointment | null>;
  // ...
}

// Implementation handles data access
class AppointmentRepository implements IAppointmentRepository {
  async getAll(): Promise<Appointment[]> {
    return dataStore.getAppointments();
  }
}
```

---

## 🚀 Deployment

### Frontend (Vercel/Netlify)

```bash
npm run build
# Deploy dist/ folder
```

### Backend (Railway/Render/Heroku)

```bash
cd server
npm run build
npm start
```

### Environment Variables

**Frontend (.env):**
```
VITE_API_URL=https://your-api-url.com/api
```

**Backend (.env):**
```
PORT=5000
NODE_ENV=production
```

---

## 🐛 Troubleshooting

### Backend won't start
```bash
cd server
rm -rf node_modules dist
npm install
npm run dev
```

### Frontend can't connect to backend
1. Check backend is running on port 5000
2. Check CORS settings in `server/src/index.ts`
3. Check `VITE_API_URL` in `.env`

### Data not persisting
- Check `server/data/` directory exists
- Check file permissions
- Restart backend server

### Port already in use
```bash
# Kill process on port 5000
npx kill-port 5000

# Kill process on port 5173
npx kill-port 5173
```

---

## 📚 Development Workflow

### 1. Make Changes to Backend

```bash
# Backend auto-reloads with ts-node-dev
cd server
npm run dev
```

### 2. Make Changes to Frontend

```bash
# Frontend auto-reloads with Vite
npm run dev
```

### 3. Test API Changes

Use Postman or REST Client to test endpoints.

### 4. Test Frontend Integration

Open browser and test the UI.

---

## 🎯 Next Steps

### Add Authentication
- JWT tokens
- User registration/login
- Protected routes

### Add Database
- Replace JSON with PostgreSQL
- Use Prisma or TypeORM

### Add Real-time Features
- WebSocket for live updates
- Push notifications

### Add Payment Integration
- Stripe for payments
- Subscription management

---

## 📖 Documentation

- **Frontend**: See `README.md`
- **Backend**: See `server/README.md` (create if needed)
- **API**: See endpoints list above
- **Architecture**: See comments in code

---

## ✅ Verification Checklist

After setup, verify:

- [ ] Backend starts on port 5000
- [ ] Frontend starts on port 5173
- [ ] API health check works: `http://localhost:5000/api/health`
- [ ] Frontend can fetch services
- [ ] Can create a booking
- [ ] Data persists in `server/data/`
- [ ] Dashboard shows stats
- [ ] Contact form submits

---

## 🎉 You're Ready!

Your full-stack SharpCuts application is now running with:

✅ Real backend API (Node.js/Express)
✅ Layered architecture with DI
✅ Repository pattern
✅ JSON file storage
✅ Frontend connected to backend
✅ All features working end-to-end

**Happy coding!** 🚀
