#!/bin/bash

# ============================================
# SharpCuts - Quick Start Script
# ============================================
# This script starts both frontend and backend servers

echo "🚀 ========================================"
echo "💈 SharpCuts - Full Stack Application"
echo "🚀 ========================================"
echo ""

# Check if node is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js 18+ first."
    exit 1
fi

# Check if npm is installed
if ! command -v npm &> /dev/null; then
    echo "❌ npm is not installed. Please install npm first."
    exit 1
fi

echo "✅ Node.js and npm detected"
echo ""

# Install frontend dependencies if needed
if [ ! -d "node_modules" ]; then
    echo "📦 Installing frontend dependencies..."
    npm install
    echo ""
fi

# Install backend dependencies if needed
if [ ! -d "server/node_modules" ]; then
    echo "📦 Installing backend dependencies..."
    cd server
    npm install
    cd ..
    echo ""
fi

echo "🎯 Starting servers..."
echo ""
echo "📍 Frontend: http://localhost:5173"
echo "📍 Backend:  http://localhost:5000"
echo "📍 API:      http://localhost:5000/api"
echo ""
echo "Press Ctrl+C to stop both servers"
echo ""
echo "🚀 ========================================"
echo ""

# Start both servers
# Backend in background
cd server
npm run dev &
BACKEND_PID=$!
cd ..

# Frontend in foreground
npm run dev &
FRONTEND_PID=$!

# Wait for both processes
wait $BACKEND_PID $FRONTEND_PID
