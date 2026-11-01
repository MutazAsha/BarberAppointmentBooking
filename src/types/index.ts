export interface Service {
  id: string;
  name: string;
  description: string;
  duration: number; // minutes
  price: number;
  category: string;
  image?: string;
}

export interface Barber {
  id: string;
  name: string;
  avatar: string;
  specialty: string;
  rating: number;
  reviews: number;
  experience: number; // years
  available: boolean;
}

export interface TimeSlot {
  id: string;
  time: string;
  available: boolean;
}

export interface Appointment {
  id: string;
  serviceId: string;
  serviceName: string;
  barberId: string;
  barberName: string;
  date: string;
  time: string;
  duration: number;
  price: number;
  status: 'pending' | 'confirmed' | 'completed' | 'cancelled';
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  notes?: string;
  createdAt: string;
}

export interface BookingState {
  selectedService: Service | null;
  selectedBarber: Barber | null;
  selectedDate: string;
  selectedTime: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  notes: string;
  step: number;
}

export interface UIState {
  sidebarOpen: boolean;
  theme: 'light' | 'dark';
}
