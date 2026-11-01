// ============================================
// DOMAIN LAYER - Repository Interfaces
// ============================================
// These interfaces define contracts for data access
// Following the Repository Pattern from .NET

import { Service, Barber, Appointment, ContactMessage } from '../entities';

// Generic Repository Interface
export interface IRepository<T> {
  getAll(): Promise<T[]>;
  getById(id: string): Promise<T | null>;
  create(entity: T): Promise<T>;
  update(id: string, entity: Partial<T>): Promise<T | null>;
  delete(id: string): Promise<boolean>;
}

// Specific Repository Interfaces
export interface IServiceRepository extends IRepository<Service> {
  getByCategory(category: string): Promise<Service[]>;
  getActiveServices(): Promise<Service[]>;
}

export interface IBarberRepository extends IRepository<Barber> {
  getAvailableBarbers(): Promise<Barber[]>;
}

export interface IAppointmentRepository extends IRepository<Appointment> {
  getByCustomerEmail(email: string): Promise<Appointment[]>;
  getByBarberId(barberId: string): Promise<Appointment[]>;
  getByDate(date: string): Promise<Appointment[]>;
  hasConflict(barberId: string, date: string, time: string): Promise<boolean>;
}

export interface IContactRepository extends IRepository<ContactMessage> {
  getAll(): Promise<ContactMessage[]>;
}
