@echo off
REM ============================================
REM SharpCuts - Quick Start Script (Windows)
REM ============================================

echo.
echo 🚀 ========================================
echo 💈 SharpCuts - Full Stack Application
echo 🚀 ========================================
echo.

REM Check if node is installed
where node >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ Node.js is not installed. Please install Node.js 18+ first.
    pause
    exit /b 1
)

echo ✅ Node.js detected
echo.

REM Install frontend dependencies if needed
if not exist "node_modules" (
    echo 📦 Installing frontend dependencies...
    call npm install
    echo.
)

REM Install backend dependencies if needed
if not exist "server\node_modules" (
    echo 📦 Installing backend dependencies...
    cd server
    call npm install
    cd ..
    echo.
)

echo 🎯 Starting servers...
echo.
echo 📍 Frontend: http://localhost:5173
echo 📍 Backend:  http://localhost:5000
echo 📍 API:      http://localhost:5000/api
echo.
echo Press Ctrl+C to stop both servers
echo.
echo 🚀 ========================================
echo.

REM Start backend in new window
start "SharpCuts Backend" cmd /k "cd server && npm run dev"

REM Wait a moment for backend to start
timeout /t 2 /nobreak >nul

REM Start frontend in new window
start "SharpCuts Frontend" cmd /k "npm run dev"

echo.
echo ✅ Both servers starting in separate windows...
echo.
pause
