// ============================================
// FRONTEND API SERVICE
// ============================================
// Connects to the backend API server

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// Helper function for API calls
async function apiCall<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.error || 'API request failed');
    }

    const data = await response.json();
    return data.data;
  } catch (error) {
    console.error(`API Error [${endpoint}]:`, error);
    throw error;
  }
}

// ============================================
// API SERVICE
// ============================================

export const api = {
  // Services
  async getServices() {
    return apiCall<any[]>('/services');
  },

  async getServicesByCategory(category: string) {
    return apiCall<any[]>(`/services/category/${category}`);
  },

  async getServiceById(id: string) {
    return apiCall<any>(`/services/${id}`);
  },

  // Barbers
  async getBarbers() {
    return apiCall<any[]>('/barbers');
  },

  async getAvailableBarbers() {
    return apiCall<any[]>('/barbers/available');
  },

  async getBarberById(id: string) {
    return apiCall<any>(`/barbers/${id}`);
  },

  async toggleBarberAvailability(id: string) {
    return apiCall<any>(`/barbers/${id}/toggle`, { method: 'PUT' });
  },

  // Time Slots
  async getTimeSlots(barberId: string, date: string) {
    return apiCall<any[]>(`/bookings/slots/${barberId}/${date}`);
  },

  // Appointments
  async getAppointments() {
    return apiCall<any[]>('/bookings');
  },

  async getAppointmentsByEmail(email: string) {
    return apiCall<any[]>(`/bookings/customer/${email}`);
  },

  async getAppointmentsByBarber(barberId: string) {
    return apiCall<any[]>(`/bookings/barber/${barberId}`);
  },

  async getAppointmentsByDate(date: string) {
    return apiCall<any[]>(`/bookings/date/${date}`);
  },

  async createAppointment(appointment: any) {
    return apiCall<any>('/bookings', {
      method: 'POST',
      body: JSON.stringify(appointment),
    });
  },

  async cancelAppointment(id: string) {
    return apiCall<any>(`/bookings/${id}/cancel`, { method: 'PUT' });
  },

  // Contact
  async submitContactForm(data: { name: string; email: string; subject: string; message: string }) {
    return apiCall<any>('/contact', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  },

  async getContactMessages() {
    return apiCall<any[]>('/contact');
  },

  // Dashboard
  async getStats() {
    return apiCall<any>('/dashboard/stats');
  },

  async getTodaySchedule() {
    return apiCall<any[]>('/dashboard/today');
  },

  // Health Check
  async healthCheck() {
    return apiCall<any>('/health');
  },
};
