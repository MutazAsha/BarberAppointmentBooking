// ============================================
// APPLICATION LAYER - Business Logic Services
// ============================================
// These services contain the core business logic
// They depend on repository interfaces (not implementations)

import {
  IServiceRepository,
  IBarberRepository,
  IAppointmentRepository,
  IContactRepository,
} from '../domain/interfaces';
import { Appointment, Service, Barber, ContactMessage } from '../domain/entities';
import { CreateAppointmentDTO, StatsDTO } from '../domain/dtos';

// ============================================
// BOOKING SERVICE
// ============================================

export class BookingService {
  constructor(
    private appointmentRepo: IAppointmentRepository,
    private serviceRepo: IServiceRepository,
    private barberRepo: IBarberRepository
  ) {}

  async createBooking(dto: CreateAppointmentDTO): Promise<Appointment> {
    // Validate service exists
    const service = await this.serviceRepo.getById(dto.serviceId);
    if (!service) {
      throw new Error('Service not found');
    }

    // Validate barber exists and is available
    const barber = await this.barberRepo.getById(dto.barberId);
    if (!barber) {
      throw new Error('Barber not found');
    }
    if (!barber.available) {
      throw new Error('Barber is not available');
    }

    // Check for scheduling conflicts
    const hasConflict = await this.appointmentRepo.hasConflict(
      dto.barberId,
      dto.date,
      dto.time
    );
    if (hasConflict) {
      throw new Error('This time slot is already booked');
    }

    // Create appointment
    const appointment = await this.appointmentRepo.create({
      serviceId: dto.serviceId,
      serviceName: service.name,
      barberId: dto.barberId,
      barberName: barber.name,
      date: dto.date,
      time: dto.time,
      duration: service.duration,
      price: service.price,
      status: 'confirmed',
      customerName: dto.customerName,
      customerPhone: dto.customerPhone,
      customerEmail: dto.customerEmail,
      notes: dto.notes,
    });

    return appointment;
  }

  async cancelBooking(id: string): Promise<boolean> {
    const appointment = await this.appointmentRepo.getById(id);
    if (!appointment) {
      throw new Error('Appointment not found');
    }

    await this.appointmentRepo.update(id, { status: 'cancelled' });
    return true;
  }

  async getAllAppointments(): Promise<Appointment[]> {
    return this.appointmentRepo.getAll();
  }

  async getAppointmentsByEmail(email: string): Promise<Appointment[]> {
    return this.appointmentRepo.getByCustomerEmail(email);
  }

  async getAppointmentsByBarber(barberId: string): Promise<Appointment[]> {
    return this.appointmentRepo.getByBarberId(barberId);
  }

  async getAppointmentsByDate(date: string): Promise<Appointment[]> {
    return this.appointmentRepo.getByDate(date);
  }

  async getAvailableTimeSlots(barberId: string, date: string): Promise<{ time: string; available: boolean }[]> {
    const appointments = await this.appointmentRepo.getAll();
    const slots: { time: string; available: boolean }[] = [];
    
    const hours = [9, 10, 11, 12, 13, 14, 15, 16, 17];
    const minutes = ['00', '30'];

    for (const hour of hours) {
      for (const min of minutes) {
        const timeStr = `${hour.toString().padStart(2, '0')}:${min}`;
        
        // Check if slot is booked
        const isBooked = appointments.some(a =>
          a.barberId === barberId &&
          a.date === date &&
          a.time === timeStr &&
          a.status !== 'cancelled'
        );

        // Check if time has passed
        const slotDateTime = new Date(`${date}T${timeStr}`);
        const isPast = slotDateTime < new Date();

        slots.push({
          time: timeStr,
          available: !isBooked && !isPast,
        });
      }
    }

    return slots;
  }
}

// ============================================
// BARBER SERVICE
// ============================================

export class BarberService {
  constructor(private barberRepo: IBarberRepository) {}

  async getAllBarbers(): Promise<Barber[]> {
    return this.barberRepo.getAll();
  }

  async getAvailableBarbers(): Promise<Barber[]> {
    return this.barberRepo.getAvailableBarbers();
  }

  async getBarberById(id: string): Promise<Barber | null> {
    return this.barberRepo.getById(id);
  }

  async toggleAvailability(id: string): Promise<Barber | null> {
    const barber = await this.barberRepo.getById(id);
    if (!barber) return null;
    
    return this.barberRepo.update(id, { available: !barber.available });
  }
}

// ============================================
// SERVICE MANAGEMENT SERVICE
// ============================================

export class ServiceManagementService {
  constructor(private serviceRepo: IServiceRepository) {}

  async getAllServices(): Promise<Service[]> {
    return this.serviceRepo.getActiveServices();
  }

  async getServicesByCategory(category: string): Promise<Service[]> {
    return this.serviceRepo.getByCategory(category);
  }

  async getServiceById(id: string): Promise<Service | null> {
    return this.serviceRepo.getById(id);
  }

  async createService(data: Omit<Service, 'id' | 'createdAt' | 'isActive'>): Promise<Service> {
    return this.serviceRepo.create({
      ...data,
      isActive: true,
    });
  }

  async deleteService(id: string): Promise<boolean> {
    return this.serviceRepo.delete(id);
  }
}

// ============================================
// CONTACT SERVICE
// ============================================

export class ContactService {
  constructor(private contactRepo: IContactRepository) {}

  async submitMessage(data: Omit<ContactMessage, 'id' | 'createdAt'>): Promise<ContactMessage> {
    return this.contactRepo.create(data);
  }

  async getAllMessages(): Promise<ContactMessage[]> {
    return this.contactRepo.getAll();
  }
}

// ============================================
// DASHBOARD SERVICE
// ============================================

export class DashboardService {
  constructor(
    private appointmentRepo: IAppointmentRepository,
    private serviceRepo: IServiceRepository,
    private barberRepo: IBarberRepository
  ) {}

  async getStats(): Promise<StatsDTO> {
    const appointments = await this.appointmentRepo.getAll();
    const today = new Date().toISOString().split('T')[0];

    return {
      totalAppointments: appointments.length,
      todayAppointments: appointments.filter(a => 
        a.date === today && a.status !== 'cancelled'
      ).length,
      pendingAppointments: appointments.filter(a => a.status === 'pending').length,
      confirmedAppointments: appointments.filter(a => a.status === 'confirmed').length,
      completedAppointments: appointments.filter(a => a.status === 'completed').length,
      cancelledAppointments: appointments.filter(a => a.status === 'cancelled').length,
      totalRevenue: appointments
        .filter(a => a.status !== 'cancelled')
        .reduce((sum, a) => sum + a.price, 0),
    };
  }

  async getTodaySchedule(): Promise<Appointment[]> {
    const today = new Date().toISOString().split('T')[0];
    const appointments = await this.appointmentRepo.getByDate(today);
    return appointments
      .filter(a => a.status !== 'cancelled')
      .sort((a, b) => a.time.localeCompare(b.time));
  }
}
