import { Service, Barber, TimeSlot, Appointment } from '../types';

// Simulated API delay
const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

const STORAGE_KEYS = {
  SERVICES: 'sharpcuts_services',
  BARBERS: 'sharpcuts_barbers',
  APPOINTMENTS: 'sharpcuts_appointments',
  BOOKINGS: 'sharpcuts_bookings',
};

// Initial seed data
const defaultServices: Service[] = [
  { id: '1', name: 'Classic Haircut', description: 'Traditional haircut with precision cutting and styling', duration: 30, price: 25, category: 'Hair' },
  { id: '2', name: 'Beard Trim & Shape', description: 'Expert beard trimming, shaping, and conditioning', duration: 20, price: 15, category: 'Beard' },
  { id: '3', name: 'Hot Towel Shave', description: 'Luxurious hot towel straight razor shave experience', duration: 45, price: 35, category: 'Shave' },
  { id: '4', name: 'Hair & Beard Combo', description: 'Complete haircut and beard grooming package', duration: 50, price: 35, category: 'Combo' },
  { id: '5', name: 'Kids Haircut', description: 'Fun and friendly haircut for children under 12', duration: 20, price: 18, category: 'Hair' },
  { id: '6', name: 'Hair Coloring', description: 'Professional hair coloring with premium products', duration: 60, price: 55, category: 'Hair' },
  { id: '7', name: 'Scalp Treatment', description: 'Relaxing scalp massage and treatment for healthy hair', duration: 30, price: 30, category: 'Treatment' },
  { id: '8', name: 'Premium Package', description: 'Haircut, beard trim, hot towel shave, and scalp massage', duration: 90, price: 75, category: 'Combo' },
];

const defaultBarbers: Barber[] = [
  { id: '1', name: 'James Wilson', avatar: '👨‍🦱', specialty: 'Classic Cuts & Fades', rating: 4.9, reviews: 234, experience: 8, available: true },
  { id: '2', name: 'Marcus Johnson', avatar: '👨‍🦲', specialty: 'Beard Styling & Design', rating: 4.8, reviews: 189, experience: 6, available: true },
  { id: '3', name: 'David Chen', avatar: '👨', specialty: 'Modern Styles & Color', rating: 4.7, reviews: 156, experience: 5, available: true },
  { id: '4', name: 'Alex Rivera', avatar: '🧔', specialty: 'Hot Towel Shaves', rating: 4.9, reviews: 312, experience: 12, available: true },
  { id: '5', name: 'Ryan Thompson', avatar: '👱‍♂️', specialty: 'Kids & Teen Cuts', rating: 4.6, reviews: 98, experience: 3, available: true },
];

// Initialize storage with defaults if empty
const initStorage = () => {
  if (!localStorage.getItem(STORAGE_KEYS.SERVICES)) {
    localStorage.setItem(STORAGE_KEYS.SERVICES, JSON.stringify(defaultServices));
  }
  if (!localStorage.getItem(STORAGE_KEYS.BARBERS)) {
    localStorage.setItem(STORAGE_KEYS.BARBERS, JSON.stringify(defaultBarbers));
  }
  if (!localStorage.getItem(STORAGE_KEYS.APPOINTMENTS)) {
    // Seed some sample appointments
    const sampleAppointments: Appointment[] = [
      {
        id: 'seed-1',
        serviceId: '1',
        serviceName: 'Classic Haircut',
        barberId: '1',
        barberName: 'James Wilson',
        date: getTodayString(),
        time: '10:00',
        duration: 30,
        price: 25,
        status: 'confirmed',
        customerName: 'John Doe',
        customerPhone: '+1234567890',
        customerEmail: 'john@example.com',
        createdAt: new Date().toISOString(),
      },
      {
        id: 'seed-2',
        serviceId: '4',
        serviceName: 'Hair & Beard Combo',
        barberId: '2',
        barberName: 'Marcus Johnson',
        date: getTomorrowString(),
        time: '14:30',
        duration: 50,
        price: 35,
        status: 'pending',
        customerName: 'Jane Smith',
        customerPhone: '+1987654321',
        customerEmail: 'jane@example.com',
        createdAt: new Date().toISOString(),
      },
      {
        id: 'seed-3',
        serviceId: '3',
        serviceName: 'Hot Towel Shave',
        barberId: '4',
        barberName: 'Alex Rivera',
        date: getTodayString(),
        time: '11:00',
        duration: 45,
        price: 35,
        status: 'completed',
        customerName: 'Mike Brown',
        customerPhone: '+1555555555',
        customerEmail: 'mike@example.com',
        createdAt: new Date(Date.now() - 86400000).toISOString(),
      },
    ];
    localStorage.setItem(STORAGE_KEYS.APPOINTMENTS, JSON.stringify(sampleAppointments));
  }
};

function getTodayString(): string {
  return new Date().toISOString().split('T')[0];
}

function getTomorrowString(): string {
  const d = new Date();
  d.setDate(d.getDate() + 1);
  return d.toISOString().split('T')[0];
}

// ============ API SERVICE ============

export const api = {
  // Services
  async getServices(): Promise<Service[]> {
    await delay(300);
    initStorage();
    const data = localStorage.getItem(STORAGE_KEYS.SERVICES);
    return data ? JSON.parse(data) : [];
  },

  async getServicesByCategory(category: string): Promise<Service[]> {
    await delay(200);
    const services = await this.getServices();
    if (category === 'All') return services;
    return services.filter(s => s.category === category);
  },

  async addService(service: Omit<Service, 'id'>): Promise<Service> {
    await delay(400);
    const services = await this.getServices();
    const newService = { ...service, id: Date.now().toString() };
    services.push(newService);
    localStorage.setItem(STORAGE_KEYS.SERVICES, JSON.stringify(services));
    return newService;
  },

  async deleteService(id: string): Promise<void> {
    await delay(300);
    const services = await this.getServices();
    const filtered = services.filter(s => s.id !== id);
    localStorage.setItem(STORAGE_KEYS.SERVICES, JSON.stringify(filtered));
  },

  // Barbers
  async getBarbers(): Promise<Barber[]> {
    await delay(300);
    initStorage();
    const data = localStorage.getItem(STORAGE_KEYS.BARBERS);
    return data ? JSON.parse(data) : [];
  },

  async getAvailableBarbers(): Promise<Barber[]> {
    await delay(200);
    const barbers = await this.getBarbers();
    return barbers.filter(b => b.available);
  },

  async toggleBarberAvailability(id: string): Promise<Barber> {
    await delay(300);
    const barbers = await this.getBarbers();
    const barber = barbers.find(b => b.id === id);
    if (barber) {
      barber.available = !barber.available;
      localStorage.setItem(STORAGE_KEYS.BARBERS, JSON.stringify(barbers));
      return barber;
    }
    throw new Error('Barber not found');
  },

  // Time Slots - Dynamic based on actual bookings
  async getTimeSlots(barberId: string, date: string): Promise<TimeSlot[]> {
    await delay(300);
    const appointments = await this.getAppointments();
    const slots: TimeSlot[] = [];
    const hours = [9, 10, 11, 12, 13, 14, 15, 16, 17];
    const minutes = ['00', '30'];

    hours.forEach(hour => {
      minutes.forEach(min => {
        const timeStr = `${hour.toString().padStart(2, '0')}:${min}`;
        // Check if this slot is already booked
        const isBooked = appointments.some(
          a => a.barberId === barberId &&
               a.date === date &&
               a.time === timeStr &&
               a.status !== 'cancelled'
        );
        // Also check if time has passed for today
        const now = new Date();
        const slotDate = new Date(`${date}T${timeStr}`);
        const isPast = slotDate < now;

        slots.push({
          id: `${barberId}-${date}-${timeStr}`,
          time: timeStr,
          available: !isBooked && !isPast,
        });
      });
    });

    return slots;
  },

  // Appointments
  async getAppointments(): Promise<Appointment[]> {
    await delay(200);
    initStorage();
    const data = localStorage.getItem(STORAGE_KEYS.APPOINTMENTS);
    return data ? JSON.parse(data) : [];
  },

  async getAppointmentsByEmail(email: string): Promise<Appointment[]> {
    await delay(200);
    const appointments = await this.getAppointments();
    return appointments.filter(a => a.customerEmail.toLowerCase() === email.toLowerCase());
  },

  async getAppointmentsByDate(date: string): Promise<Appointment[]> {
    await delay(200);
    const appointments = await this.getAppointments();
    return appointments.filter(a => a.date === date);
  },

  async getAppointmentsByBarber(barberId: string): Promise<Appointment[]> {
    await delay(200);
    const appointments = await this.getAppointments();
    return appointments.filter(a => a.barberId === barberId);
  },

  async createAppointment(appointment: Omit<Appointment, 'id' | 'createdAt'>): Promise<Appointment> {
    await delay(500);
    const appointments = await this.getAppointments();
    
    // Check for conflicts
    const hasConflict = appointments.some(
      a => a.barberId === appointment.barberId &&
           a.date === appointment.date &&
           a.time === appointment.time &&
           a.status !== 'cancelled'
    );

    if (hasConflict) {
      throw new Error('This time slot is already booked. Please choose another.');
    }

    const newAppointment: Appointment = {
      ...appointment,
      id: Date.now().toString(),
      createdAt: new Date().toISOString(),
    };

    appointments.push(newAppointment);
    localStorage.setItem(STORAGE_KEYS.APPOINTMENTS, JSON.stringify(appointments));
    return newAppointment;
  },

  async cancelAppointment(id: string): Promise<void> {
    await delay(400);
    const appointments = await this.getAppointments();
    const appointment = appointments.find(a => a.id === id);
    if (appointment) {
      appointment.status = 'cancelled';
      localStorage.setItem(STORAGE_KEYS.APPOINTMENTS, JSON.stringify(appointments));
    }
  },

  async updateAppointmentStatus(id: string, status: Appointment['status']): Promise<void> {
    await delay(300);
    const appointments = await this.getAppointments();
    const appointment = appointments.find(a => a.id === id);
    if (appointment) {
      appointment.status = status;
      localStorage.setItem(STORAGE_KEYS.APPOINTMENTS, JSON.stringify(appointments));
    }
  },

  // Stats
  async getStats() {
    await delay(200);
    const appointments = await this.getAppointments();
    const today = getTodayString();
    
    return {
      totalAppointments: appointments.length,
      todayAppointments: appointments.filter(a => a.date === today && a.status !== 'cancelled').length,
      pendingAppointments: appointments.filter(a => a.status === 'pending').length,
      confirmedAppointments: appointments.filter(a => a.status === 'confirmed').length,
      completedAppointments: appointments.filter(a => a.status === 'completed').length,
      cancelledAppointments: appointments.filter(a => a.status === 'cancelled').length,
      totalRevenue: appointments
        .filter(a => a.status !== 'cancelled')
        .reduce((sum, a) => sum + a.price, 0),
    };
  },

  // Contact Form
  async submitContactForm(data: { name: string; email: string; subject: string; message: string }): Promise<{ success: boolean }> {
    await delay(800);
    // Simulate sending
    console.log('Contact form submitted:', data);
    return { success: true };
  },
};
