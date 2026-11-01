// ============================================
// API LAYER - Express Routes (Controllers)
// ============================================
// These are the HTTP endpoints that the frontend calls
// They use the services from the DI container

import { Router, Request, Response } from 'express';
import { container } from '../infrastructure/container';

const router = Router();

// ============================================
// SERVICES ENDPOINTS
// ============================================

// GET /api/services - Get all services
router.get('/services', async (req: Request, res: Response) => {
  try {
    const services = await container.serviceManagementService.getAllServices();
    res.json({ success: true, data: services });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/services/category/:category - Get services by category
router.get('/services/category/:category', async (req: Request, res: Response) => {
  try {
    const { category } = req.params;
    const services = await container.serviceManagementService.getServicesByCategory(category);
    res.json({ success: true, data: services });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/services/:id - Get service by ID
router.get('/services/:id', async (req: Request, res: Response) => {
  try {
    const { id } = req.params;
    const service = await container.serviceManagementService.getServiceById(id);
    if (!service) {
      return res.status(404).json({ success: false, error: 'Service not found' });
    }
    res.json({ success: true, data: service });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ============================================
// BARBERS ENDPOINTS
// ============================================

// GET /api/barbers - Get all barbers
router.get('/barbers', async (req: Request, res: Response) => {
  try {
    const barbers = await container.barberService.getAllBarbers();
    res.json({ success: true, data: barbers });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/barbers/available - Get available barbers
router.get('/barbers/available', async (req: Request, res: Response) => {
  try {
    const barbers = await container.barberService.getAvailableBarbers();
    res.json({ success: true, data: barbers });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/barbers/:id - Get barber by ID
router.get('/barbers/:id', async (req: Request, res: Response) => {
  try {
    const { id } = req.params;
    const barber = await container.barberService.getBarberById(id);
    if (!barber) {
      return res.status(404).json({ success: false, error: 'Barber not found' });
    }
    res.json({ success: true, data: barber });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// PUT /api/barbers/:id/toggle - Toggle barber availability
router.put('/barbers/:id/toggle', async (req: Request, res: Response) => {
  try {
    const { id } = req.params;
    const barber = await container.barberService.toggleAvailability(id);
    if (!barber) {
      return res.status(404).json({ success: false, error: 'Barber not found' });
    }
    res.json({ success: true, data: barber });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ============================================
// BOOKINGS/APPOINTMENTS ENDPOINTS
// ============================================

// GET /api/bookings - Get all appointments
router.get('/bookings', async (req: Request, res: Response) => {
  try {
    const appointments = await container.bookingService.getAllAppointments();
    res.json({ success: true, data: appointments });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/bookings/customer/:email - Get appointments by customer email
router.get('/bookings/customer/:email', async (req: Request, res: Response) => {
  try {
    const { email } = req.params;
    const appointments = await container.bookingService.getAppointmentsByEmail(email);
    res.json({ success: true, data: appointments });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/bookings/barber/:id - Get appointments by barber
router.get('/bookings/barber/:id', async (req: Request, res: Response) => {
  try {
    const { id } = req.params;
    const appointments = await container.bookingService.getAppointmentsByBarber(id);
    res.json({ success: true, data: appointments });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/bookings/date/:date - Get appointments by date
router.get('/bookings/date/:date', async (req: Request, res: Response) => {
  try {
    const { date } = req.params;
    const appointments = await container.bookingService.getAppointmentsByDate(date);
    res.json({ success: true, data: appointments });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/bookings/slots/:barberId/:date - Get available time slots
router.get('/bookings/slots/:barberId/:date', async (req: Request, res: Response) => {
  try {
    const { barberId, date } = req.params;
    const slots = await container.bookingService.getAvailableTimeSlots(barberId, date);
    res.json({ success: true, data: slots });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// POST /api/bookings - Create new booking
router.post('/bookings', async (req: Request, res: Response) => {
  try {
    const appointment = await container.bookingService.createBooking(req.body);
    res.status(201).json({ 
      success: true, 
      data: appointment,
      message: 'Booking created successfully' 
    });
  } catch (error: any) {
    res.status(400).json({ success: false, error: error.message });
  }
});

// PUT /api/bookings/:id/cancel - Cancel booking
router.put('/bookings/:id/cancel', async (req: Request, res: Response) => {
  try {
    const { id } = req.params;
    await container.bookingService.cancelBooking(id);
    res.json({ success: true, message: 'Booking cancelled successfully' });
  } catch (error: any) {
    res.status(400).json({ success: false, error: error.message });
  }
});

// ============================================
// CONTACT ENDPOINTS
// ============================================

// POST /api/contact - Submit contact form
router.post('/contact', async (req: Request, res: Response) => {
  try {
    const message = await container.contactService.submitMessage(req.body);
    res.status(201).json({ 
      success: true, 
      data: message,
      message: 'Message sent successfully' 
    });
  } catch (error: any) {
    res.status(400).json({ success: false, error: error.message });
  }
});

// GET /api/contact - Get all messages (admin)
router.get('/contact', async (req: Request, res: Response) => {
  try {
    const messages = await container.contactService.getAllMessages();
    res.json({ success: true, data: messages });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ============================================
// DASHBOARD ENDPOINTS
// ============================================

// GET /api/dashboard/stats - Get dashboard statistics
router.get('/dashboard/stats', async (req: Request, res: Response) => {
  try {
    const stats = await container.dashboardService.getStats();
    res.json({ success: true, data: stats });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// GET /api/dashboard/today - Get today's schedule
router.get('/dashboard/today', async (req: Request, res: Response) => {
  try {
    const schedule = await container.dashboardService.getTodaySchedule();
    res.json({ success: true, data: schedule });
  } catch (error: any) {
    res.status(500).json({ success: false, error: error.message });
  }
});

// ============================================
// HEALTH CHECK
// ============================================

router.get('/health', (req: Request, res: Response) => {
  res.json({ 
    success: true, 
    message: 'SharpCuts API is running',
    timestamp: new Date().toISOString()
  });
});

export default router;
