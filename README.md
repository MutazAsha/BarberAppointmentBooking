# SharpCuts - Barber Shop Appointment Booking System

## 🏗️ Architecture Overview

This project follows a **layered architecture** pattern with **Dependency Injection** throughout.

### Frontend (React + Redux + Tailwind)
- **React 18** with TypeScript
- **Redux Toolkit** for state management
- **Tailwind CSS** for responsive styling
- **React Router** for navigation

### Backend (.NET 8 Web API)
- **Layered Architecture**: API → Application → Domain → Infrastructure
- **Dependency Injection** for all services
- **Repository Pattern** for data access
- **Entity Framework Core** for ORM

---

## 📁 Project Structure

```
sharpcuts/
├── src/                          # Frontend (React)
│   ├── components/               # Reusable UI components
│   ├── pages/                    # Page components
│   ├── store/                    # Redux store & slices
│   ├── types/                    # TypeScript types
│   ├── data/                     # Mock data / API services
│   ├── App.tsx                   # Main app component
│   └── main.tsx                  # Entry point
│
├── backend/                      # Backend (.NET)
│   ├── SharpCuts.API/            # Web API Layer
│   ├── SharpCuts.Application/    # Application/Business Logic
│   ├── SharpCuts.Domain/         # Domain Entities
│   └── SharpCuts.Infrastructure/ # Data Access & External Services
│
└── README.md
```

---

## 🚀 Getting Started

### Frontend
```bash
npm install
npm run dev
```

### Backend
```bash
cd backend
dotnet restore
dotnet run --project SharpCuts.API
```

---

## 📋 Features

- ✅ Browse barber services with categories
- ✅ Select preferred barber
- ✅ Multi-step booking process (Service → Barber → Date/Time → Details)
- ✅ View & manage appointments
- ✅ Cancel appointments
- ✅ Responsive design (mobile, tablet, desktop)
- ✅ Form validation
- ✅ Contact form

---

## 🌐 Deployment

### GitHub Pages (Frontend)
```bash
npm run build
# Push dist/ folder to gh-pages branch
```

### Azure/AWS (Backend)
```bash
dotnet publish -c Release -o ./publish
# Deploy to Azure App Service or AWS Elastic Beanstalk
```

---

## 🔧 Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend UI | React 18, TypeScript |
| State Management | Redux Toolkit |
| Styling | Tailwind CSS |
| Routing | React Router v6 |
| Backend API | .NET 8 Web API |
| Database | SQL Server / PostgreSQL |
| ORM | Entity Framework Core |
| Auth | JWT Bearer Tokens |
