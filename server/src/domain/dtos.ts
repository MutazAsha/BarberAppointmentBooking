// ============================================
// DOMAIN LAYER - DTOs (Data Transfer Objects)
// ============================================

export interface CreateAppointmentDTO {
  serviceId: string;
  barberId: string;
  date: string;
  time: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  notes?: string;
}

export interface CreateServiceDTO {
  name: string;
  description: string;
  duration: number;
  price: number;
  category: string;
}

export interface CreateBarberDTO {
  name: string;
  avatar: string;
  specialty: string;
  rating: number;
  reviews: number;
  experience: number;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  message?: string;
}

export interface StatsDTO {
  totalAppointments: number;
  todayAppointments: number;
  pendingAppointments: number;
  confirmedAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
  totalRevenue: number;
}
