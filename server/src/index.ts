// ============================================
// SERVER ENTRY POINT
// ============================================
// Express server with middleware and routes

import express from 'express';
import cors from 'cors';
import routes from './api/routes';
import { container } from './infrastructure/container';

const app = express();
const PORT = process.env.PORT || 5000;

// ============================================
// MIDDLEWARE
// ============================================

// CORS - Allow frontend to access API
app.use(cors({
  origin: ['http://localhost:3000', 'http://localhost:5173', 'http://localhost:5174'],
  credentials: true,
}));

// Parse JSON bodies
app.use(express.json());

// Parse URL-encoded bodies
app.use(express.urlencoded({ extended: true }));

// Request logging middleware
app.use((req, res, next) => {
  console.log(`${new Date().toISOString()} - ${req.method} ${req.path}`);
  next();
});

// ============================================
// ROUTES
// ============================================

// API routes
app.use('/api', routes);

// Root endpoint
app.get('/', (req, res) => {
  res.json({
    success: true,
    message: 'Welcome to SharpCuts API',
    version: '1.0.0',
    endpoints: {
      services: '/api/services',
      barbers: '/api/barbers',
      bookings: '/api/bookings',
      contact: '/api/contact',
      dashboard: '/api/dashboard',
      health: '/api/health',
    },
  });
});

// 404 handler
app.use((req, res) => {
  res.status(404).json({
    success: false,
    error: 'Endpoint not found',
    path: req.path,
  });
});

// Error handler
app.use((err: any, req: express.Request, res: express.Response, next: express.NextFunction) => {
  console.error('Error:', err);
  res.status(500).json({
    success: false,
    error: err.message || 'Internal server error',
  });
});

// ============================================
// START SERVER
// ============================================

app.listen(PORT, () => {
  console.log('\n🚀 ========================================');
  console.log('💈 SharpCuts API Server');
  console.log('🚀 ========================================');
  console.log(`📍 Server running on: http://localhost:${PORT}`);
  console.log(`🔌 API endpoints: http://localhost:${PORT}/api`);
  console.log(`✅ Health check: http://localhost:${PORT}/api/health`);
  console.log('🚀 ========================================\n');
});

export default app;
