// ============================================
// INFRASTRUCTURE LAYER - Dependency Injection Container
// ============================================
// Implements DI pattern similar to .NET's IServiceCollection

import {
  ServiceRepository,
  BarberRepository,
  AppointmentRepository,
  ContactRepository,
} from './repositories';
import {
  IServiceRepository,
  IBarberRepository,
  IAppointmentRepository,
  IContactRepository,
} from '../domain/interfaces';
import { BookingService, BarberService, ServiceManagementService, ContactService, DashboardService } from '../application/services';

// ============================================
// DI CONTAINER
// ============================================

export class Container {
  private static instance: Container;
  
  // Repositories (Singletons)
  private _serviceRepository: IServiceRepository;
  private _barberRepository: IBarberRepository;
  private _appointmentRepository: IAppointmentRepository;
  private _contactRepository: IContactRepository;

  // Services (Singletons)
  private _bookingService: BookingService;
  private _barberService: BarberService;
  private _serviceManagementService: ServiceManagementService;
  private _contactService: ContactService;
  private _dashboardService: DashboardService;

  private constructor() {
    // Register Repositories
    this._serviceRepository = new ServiceRepository();
    this._barberRepository = new BarberRepository();
    this._appointmentRepository = new AppointmentRepository();
    this._contactRepository = new ContactRepository();

    // Register Services (injecting repositories)
    this._bookingService = new BookingService(
      this._appointmentRepository,
      this._serviceRepository,
      this._barberRepository
    );
    
    this._barberService = new BarberService(this._barberRepository);
    
    this._serviceManagementService = new ServiceManagementService(this._serviceRepository);
    
    this._contactService = new ContactService(this._contactRepository);
    
    this._dashboardService = new DashboardService(
      this._appointmentRepository,
      this._serviceRepository,
      this._barberRepository
    );

    console.log('✅ Dependency Injection Container initialized');
  }

  static getInstance(): Container {
    if (!Container.instance) {
      Container.instance = new Container();
    }
    return Container.instance;
  }

  // Repository getters
  get serviceRepository(): IServiceRepository {
    return this._serviceRepository;
  }

  get barberRepository(): IBarberRepository {
    return this._barberRepository;
  }

  get appointmentRepository(): IAppointmentRepository {
    return this._appointmentRepository;
  }

  get contactRepository(): IContactRepository {
    return this._contactRepository;
  }

  // Service getters
  get bookingService(): BookingService {
    return this._bookingService;
  }

  get barberService(): BarberService {
    return this._barberService;
  }

  get serviceManagementService(): ServiceManagementService {
    return this._serviceManagementService;
  }

  get contactService(): ContactService {
    return this._contactService;
  }

  get dashboardService(): DashboardService {
    return this._dashboardService;
  }
}

// Export singleton instance
export const container = Container.getInstance();
