// ============================================
// INFRASTRUCTURE LAYER - Repository Implementations
// ============================================
// Implements the Repository Pattern with actual data access

import { v4 as uuidv4 } from 'uuid';
import { dataStore } from './dataStore';
import {
  IServiceRepository,
  IBarberRepository,
  IAppointmentRepository,
  IContactRepository,
} from '../domain/interfaces';
import { Service, Barber, Appointment, ContactMessage } from '../domain/entities';

// ============================================
// SERVICE REPOSITORY
// ============================================

export class ServiceRepository implements IServiceRepository {
  async getAll(): Promise<Service[]> {
    return dataStore.getServices();
  }

  async getById(id: string): Promise<Service | null> {
    const services = dataStore.getServices();
    return services.find(s => s.id === id) || null;
  }

  async create(service: Omit<Service, 'id' | 'createdAt'>): Promise<Service> {
    const services = dataStore.getServices();
    const newService: Service = {
      ...service,
      id: uuidv4(),
      createdAt: new Date().toISOString(),
    };
    services.push(newService);
    dataStore.saveServices(services);
    return newService;
  }

  async update(id: string, updates: Partial<Service>): Promise<Service | null> {
    const services = dataStore.getServices();
    const index = services.findIndex(s => s.id === id);
    if (index === -1) return null;
    
    services[index] = { ...services[index], ...updates };
    dataStore.saveServices(services);
    return services[index];
  }

  async delete(id: string): Promise<boolean> {
    const services = dataStore.getServices();
    const filtered = services.filter(s => s.id !== id);
    if (filtered.length === services.length) return false;
    dataStore.saveServices(filtered);
    return true;
  }

  async getByCategory(category: string): Promise<Service[]> {
    const services = dataStore.getServices();
    if (category === 'All') return services.filter(s => s.isActive);
    return services.filter(s => s.category === category && s.isActive);
  }

  async getActiveServices(): Promise<Service[]> {
    const services = dataStore.getServices();
    return services.filter(s => s.isActive);
  }
}

// ============================================
// BARBER REPOSITORY
// ============================================

export class BarberRepository implements IBarberRepository {
  async getAll(): Promise<Barber[]> {
    return dataStore.getBarbers();
  }

  async getById(id: string): Promise<Barber | null> {
    const barbers = dataStore.getBarbers();
    return barbers.find(b => b.id === id) || null;
  }

  async create(barber: Omit<Barber, 'id' | 'createdAt'>): Promise<Barber> {
    const barbers = dataStore.getBarbers();
    const newBarber: Barber = {
      ...barber,
      id: uuidv4(),
      createdAt: new Date().toISOString(),
    };
    barbers.push(newBarber);
    dataStore.saveBarbers(barbers);
    return newBarber;
  }

  async update(id: string, updates: Partial<Barber>): Promise<Barber | null> {
    const barbers = dataStore.getBarbers();
    const index = barbers.findIndex(b => b.id === id);
    if (index === -1) return null;
    
    barbers[index] = { ...barbers[index], ...updates };
    dataStore.saveBarbers(barbers);
    return barbers[index];
  }

  async delete(id: string): Promise<boolean> {
    const barbers = dataStore.getBarbers();
    const filtered = barbers.filter(b => b.id !== id);
    if (filtered.length === barbers.length) return false;
    dataStore.saveBarbers(filtered);
    return true;
  }

  async getAvailableBarbers(): Promise<Barber[]> {
    const barbers = dataStore.getBarbers();
    return barbers.filter(b => b.available);
  }
}

// ============================================
// APPOINTMENT REPOSITORY
// ============================================

export class AppointmentRepository implements IAppointmentRepository {
  async getAll(): Promise<Appointment[]> {
    return dataStore.getAppointments();
  }

  async getById(id: string): Promise<Appointment | null> {
    const appointments = dataStore.getAppointments();
    return appointments.find(a => a.id === id) || null;
  }

  async create(appointment: Omit<Appointment, 'id' | 'createdAt'>): Promise<Appointment> {
    const appointments = dataStore.getAppointments();
    const newAppointment: Appointment = {
      ...appointment,
      id: uuidv4(),
      createdAt: new Date().toISOString(),
    };
    appointments.push(newAppointment);
    dataStore.saveAppointments(appointments);
    return newAppointment;
  }

  async update(id: string, updates: Partial<Appointment>): Promise<Appointment | null> {
    const appointments = dataStore.getAppointments();
    const index = appointments.findIndex(a => a.id === id);
    if (index === -1) return null;
    
    appointments[index] = { ...appointments[index], ...updates };
    dataStore.saveAppointments(appointments);
    return appointments[index];
  }

  async delete(id: string): Promise<boolean> {
    const appointments = dataStore.getAppointments();
    const filtered = appointments.filter(a => a.id !== id);
    if (filtered.length === appointments.length) return false;
    dataStore.saveAppointments(filtered);
    return true;
  }

  async getByCustomerEmail(email: string): Promise<Appointment[]> {
    const appointments = dataStore.getAppointments();
    return appointments.filter(a => 
      a.customerEmail.toLowerCase() === email.toLowerCase()
    );
  }

  async getByBarberId(barberId: string): Promise<Appointment[]> {
    const appointments = dataStore.getAppointments();
    return appointments.filter(a => a.barberId === barberId);
  }

  async getByDate(date: string): Promise<Appointment[]> {
    const appointments = dataStore.getAppointments();
    return appointments.filter(a => a.date === date);
  }

  async hasConflict(barberId: string, date: string, time: string): Promise<boolean> {
    const appointments = dataStore.getAppointments();
    return appointments.some(a => 
      a.barberId === barberId &&
      a.date === date &&
      a.time === time &&
      a.status !== 'cancelled'
    );
  }
}

// ============================================
// CONTACT REPOSITORY
// ============================================

export class ContactRepository implements IContactRepository {
  async getAll(): Promise<ContactMessage[]> {
    return dataStore.getMessages();
  }

  async getById(id: string): Promise<ContactMessage | null> {
    const messages = dataStore.getMessages();
    return messages.find(m => m.id === id) || null;
  }

  async create(message: Omit<ContactMessage, 'id' | 'createdAt'>): Promise<ContactMessage> {
    const messages = dataStore.getMessages();
    const newMessage: ContactMessage = {
      ...message,
      id: uuidv4(),
      createdAt: new Date().toISOString(),
    };
    messages.push(newMessage);
    dataStore.saveMessages(messages);
    return newMessage;
  }

  async update(id: string, updates: Partial<ContactMessage>): Promise<ContactMessage | null> {
    const messages = dataStore.getMessages();
    const index = messages.findIndex(m => m.id === id);
    if (index === -1) return null;
    
    messages[index] = { ...messages[index], ...updates };
    dataStore.saveMessages(messages);
    return messages[index];
  }

  async delete(id: string): Promise<boolean> {
    const messages = dataStore.getMessages();
    const filtered = messages.filter(m => m.id !== id);
    if (filtered.length === messages.length) return false;
    dataStore.saveMessages(filtered);
    return true;
  }
}
