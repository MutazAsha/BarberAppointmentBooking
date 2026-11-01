# 💈 SharpCuts - Full Stack Barber Shop Booking System

A complete **full-stack** barber shop appointment booking system with a modern React frontend and Node.js/Express backend.

![SharpCuts](https://img.shields.io/badge/SharpCuts-v1.0-amber)
![React](https://img.shields.io/badge/React-18-blue)
![Node.js](https://img.shields.io/badge/Node.js-18+-green)
![TypeScript](https://img.shields.io/badge/TypeScript-5-blue)
![License](https://img.shields.io/badge/License-MIT-yellow)

---

## 🌟 Features

### Frontend
- ✅ **Dynamic Booking System** - Real-time availability & conflict detection
- ✅ **Admin Dashboard** - Live analytics, revenue tracking, performance metrics
- ✅ **Responsive Design** - Mobile, tablet, and desktop optimized
- ✅ **Toast Notifications** - Real-time user feedback
- ✅ **Form Validation** - Client-side validation with error handling
- ✅ **Redux State Management** - Centralized state with async thunks

### Backend
- ✅ **RESTful API** - Complete CRUD operations
- ✅ **Layered Architecture** - Clean separation of concerns
- ✅ **Dependency Injection** - Professional DI container
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **JSON Storage** - Persistent data without database setup
- ✅ **CORS Enabled** - Secure cross-origin requests

---

## 🚀 Quick Start

### Prerequisites

- Node.js 18+ ([Download](https://nodejs.org/))
- npm 9+ (comes with Node.js)
- Git ([Download](https://git-scm.com/))

### Installation

```bash
# Clone the repository
git clone https://github.com/yourusername/sharpcuts.git
cd sharpcuts

# Install frontend dependencies
npm install

# Install backend dependencies
cd server
npm install
cd ..
```

### Running the Application

**Option 1: Separate Terminals (Recommended)**

Terminal 1 - Backend:
```bash
cd server
npm run dev
```

Terminal 2 - Frontend:
```bash
npm run dev
```

**Option 2: Combined (if you add concurrently)**

```bash
npm install -D concurrently
npm run dev:all
```

### Access the Application

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5000
- **API Health**: http://localhost:5000/api/health

---

## 📁 Project Structure

```
sharpcuts/
├── 📁 src/                          # Frontend (React)
│   ├── 📁 components/               # Reusable UI components
│   │   ├── Navbar.tsx
│   │   ├── Footer.tsx
│   │   ├── Toast.tsx
│   │   └── LoadingSpinner.tsx
│   ├── 📁 pages/                    # Page components
│   │   ├── HomePage.tsx
│   │   ├── ServicesPage.tsx
│   │   ├── BookingPage.tsx
│   │   ├── AppointmentsPage.tsx
│   │   ├── DashboardPage.tsx
│   │   └── ContactPage.tsx
│   ├── 📁 store/                    # Redux store
│   │   ├── index.ts
│   │   ├── bookingSlice.ts
│   │   ├── appointmentSlice.ts
│   │   └── uiSlice.ts
│   ├── 📁 services/                 # API service
│   │   └── api.ts
│   ├── 📁 types/                    # TypeScript types
│   │   └── index.ts
│   ├── App.tsx
│   └── main.tsx
│
├── 📁 server/                       # Backend (Node.js/Express)
│   ├── 📁 src/
│   │   ├── 📁 api/                  # Routes/Controllers
│   │   │   └── routes.ts
│   │   ├── 📁 application/          # Business logic
│   │   │   └── services.ts
│   │   ├── 📁 domain/               # Entities & interfaces
│   │   │   ├── entities.ts
│   │   │   ├── interfaces.ts
│   │   │   └── dtos.ts
│   │   ├── 📁 infrastructure/       # Data access & DI
│   │   │   ├── container.ts
│   │   │   ├── repositories.ts
│   │   │   └── dataStore.ts
│   │   └── index.ts                 # Server entry
│   ├── 📁 data/                     # JSON storage (auto-created)
│   ├── package.json
│   └── tsconfig.json
│
├── 📁 backend/                      # .NET Backend (Reference)
│   └── SharpCuts.sln
│
├── .vscode/                         # VS Code config
├── .env                             # Environment variables
├── package.json                     # Frontend dependencies
└── README.md                        # This file
```

---

## 🎯 Features in Detail

### 1. Home Page
- Hero section with live barber availability
- Dynamic service cards
- Barber profiles with ratings
- Call-to-action sections

### 2. Services Page
- Search functionality
- Category filtering (Hair, Beard, Shave, Combo, Treatment)
- Sort by name/price/duration
- Book buttons for each service

### 3. Booking System (4-Step Wizard)
**Step 1**: Select Service
**Step 2**: Choose Barber
**Step 3**: Pick Date & Time (real-time availability)
**Step 4**: Enter Details & Confirm

Features:
- Real-time slot availability
- Conflict detection (prevents double-booking)
- Form validation
- Success/error notifications

### 4. Appointments Page
- View all appointments
- Filter by status (All/Confirmed/Pending/Completed/Cancelled)
- Cancel appointments with confirmation
- Sort by date
- "Upcoming" indicators

### 5. Admin Dashboard
**Overview Tab:**
- Today's bookings count
- Total revenue
- Confirmed/Pending/Completed/Cancelled counts
- Service popularity chart
- Revenue breakdown
- Today's schedule timeline

**Appointments Tab:**
- Full appointments table
- Customer details
- Status badges

**Barbers Tab:**
- Barber performance cards
- Appointment counts
- Revenue per barber

### 6. Contact Page
- Contact form with validation
- Contact information cards
- Business hours
- Map placeholder

---

## 🌐 API Documentation

### Base URL
```
http://localhost:5000/api
```

### Endpoints

#### Services
```http
GET    /api/services                    # Get all services
GET    /api/services/category/:category # Get by category
GET    /api/services/:id                # Get by ID
```

#### Barbers
```http
GET    /api/barbers                     # Get all barbers
GET    /api/barbers/available           # Get available barbers
GET    /api/barbers/:id                 # Get by ID
PUT    /api/barbers/:id/toggle          # Toggle availability
```

#### Bookings
```http
GET    /api/bookings                    # Get all appointments
GET    /api/bookings/customer/:email    # Get by customer
GET    /api/bookings/barber/:id         # Get by barber
GET    /api/bookings/date/:date         # Get by date
GET    /api/bookings/slots/:barberId/:date  # Get time slots
POST   /api/bookings                    # Create booking
PUT    /api/bookings/:id/cancel         # Cancel booking
```

#### Contact
```http
POST   /api/contact                     # Submit message
GET    /api/contact                     # Get all messages
```

#### Dashboard
```http
GET    /api/dashboard/stats             # Get statistics
GET    /api/dashboard/today             # Get today's schedule
```

#### Health
```http
GET    /api/health                      # Health check
```

### Example Requests

**Create Booking:**
```bash
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
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "abc123",
    "serviceId": "1",
    "serviceName": "Classic Haircut",
    "barberId": "1",
    "barberName": "James Wilson",
    "date": "2026-01-20",
    "time": "14:30",
    "status": "confirmed",
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "price": 25,
    "duration": 30
  },
  "message": "Booking created successfully"
}
```

---

## 🏛️ Architecture

### Frontend Architecture
```
Components → Redux Store → API Service → Backend
```

- **React 18** with TypeScript
- **Redux Toolkit** for state management
- **React Router v6** for navigation
- **Tailwind CSS** for styling
- **Lucide React** for icons

### Backend Architecture
```
API Routes → Services → Repositories → Data Store
```

**Layers:**
1. **API Layer** - HTTP endpoints (routes.ts)
2. **Application Layer** - Business logic (services.ts)
3. **Domain Layer** - Entities, interfaces, DTOs
4. **Infrastructure Layer** - Repositories, DI container, data access

**Patterns:**
- **Dependency Injection** - Container manages all dependencies
- **Repository Pattern** - Abstracts data access
- **Service Layer** - Encapsulates business logic
- **DTO Pattern** - Separates API contracts from domain models

---

## 🗄️ Data Storage

The backend uses **JSON file storage** in `server/data/`:

- `services.json` - Service catalog (8 services)
- `barbers.json` - Barber profiles (5 barbers)
- `appointments.json` - All appointments
- `messages.json` - Contact messages

**Seed Data:**
- 8 services across 5 categories
- 5 barbers with different specialties
- Sample appointments for testing

**To Reset Data:**
```bash
cd server
rm -rf data/
npm run dev  # Recreates with seed data
```

---

## 🎨 Tech Stack

### Frontend
| Technology | Version | Purpose |
|------------|---------|---------|
| React | 18.2.0 | UI library |
| TypeScript | 5.x | Type safety |
| Redux Toolkit | 2.x | State management |
| React Router | 6.x | Navigation |
| Tailwind CSS | 4.x | Styling |
| Lucide React | 0.294 | Icons |
| Vite | 6.x | Build tool |

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| Node.js | 18+ | Runtime |
| Express | 4.18 | Web framework |
| TypeScript | 5.x | Type safety |
| UUID | 9.x | ID generation |
| CORS | 2.8 | Cross-origin |
| ts-node-dev | 2.x | Dev server |

---

## 🚀 Deployment

### Frontend (Vercel/Netlify)

```bash
npm run build
# Deploy dist/ folder
```

**Environment Variables:**
```
VITE_API_URL=https://your-api-url.com/api
```

### Backend (Railway/Render/Heroku)

```bash
cd server
npm run build
npm start
```

**Environment Variables:**
```
PORT=5000
NODE_ENV=production
```

### GitHub Pages (Frontend Only)

```bash
npm install -D gh-pages
npm run build
npx gh-pages -d dist
```

---

## 🧪 Testing

### Manual Testing

1. **Start both servers**
2. **Test booking flow:**
   - Select service
   - Choose barber
   - Pick date & time
   - Fill details
   - Confirm booking
3. **Check dashboard:**
   - Verify stats update
   - Check today's schedule
4. **Test cancellation:**
   - Cancel appointment
   - Verify status changes

### API Testing

Use Postman or REST Client:
```http
GET http://localhost:5000/api/services
POST http://localhost:5000/api/bookings
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

### Frontend can't connect
1. Check backend is running on port 5000
2. Verify `VITE_API_URL` in `.env`
3. Check CORS settings in `server/src/index.ts`

### Port already in use
```bash
npx kill-port 5000  # Backend
npx kill-port 5173  # Frontend
```

### Data not persisting
- Check `server/data/` directory exists
- Verify file permissions
- Restart backend server

---

## 📚 Documentation

- **Full Stack Setup**: See `FULLSTACK_SETUP.md`
- **VS Code Setup**: See `OPEN_IN_VSCODE.md`
- **Dynamic Features**: See `DYNAMIC_FEATURES.md`
- **Deployment**: See `DEPLOYMENT.md`
- **.NET Backend**: See `backend/` folder

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

---

## 📝 License

MIT License - feel free to use this project for learning or commercial purposes.

---

## 🎉 Credits

Built with ❤️ using:
- React + TypeScript
- Node.js + Express
- Tailwind CSS
- Redux Toolkit

---

## 📞 Support

For issues and questions:
- Open an issue on GitHub
- Check the documentation files
- Review the code comments

---

## ✅ Checklist

After setup, verify:

- [ ] Backend starts on port 5000
- [ ] Frontend starts on port 5173
- [ ] Can view services
- [ ] Can create a booking
- [ ] Can view appointments
- [ ] Dashboard shows stats
- [ ] Can cancel appointments
- [ ] Contact form works
- [ ] Data persists across refresh

---

**Enjoy building with SharpCuts!** 💈✨
