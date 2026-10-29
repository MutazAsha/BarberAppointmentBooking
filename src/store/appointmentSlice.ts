import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Appointment } from '../types';

interface AppointmentState {
  appointments: Appointment[];
  loading: boolean;
  error: string | null;
}

const initialState: AppointmentState = {
  appointments: [
    {
      id: '1',
      serviceId: '1',
      serviceName: 'Classic Haircut',
      barberId: '1',
      barberName: 'James Wilson',
      date: '2026-01-15',
      time: '10:00',
      duration: 30,
      price: 25,
      status: 'completed',
      customerName: 'John Doe',
      customerPhone: '+1234567890',
      customerEmail: 'john@example.com',
      createdAt: '2026-01-14T10:00:00Z',
    },
    {
      id: '2',
      serviceId: '4',
      serviceName: 'Hair & Beard Combo',
      barberId: '2',
      barberName: 'Marcus Johnson',
      date: '2026-01-20',
      time: '14:30',
      duration: 50,
      price: 35,
      status: 'confirmed',
      customerName: 'John Doe',
      customerPhone: '+1234567890',
      customerEmail: 'john@example.com',
      createdAt: '2026-01-18T10:00:00Z',
    },
  ],
  loading: false,
  error: null,
};

const appointmentSlice = createSlice({
  name: 'appointments',
  initialState,
  reducers: {
    addAppointment: (state, action: PayloadAction<Appointment>) => {
      state.appointments.push(action.payload);
    },
    cancelAppointment: (state, action: PayloadAction<string>) => {
      const appointment = state.appointments.find((a) => a.id === action.payload);
      if (appointment) {
        appointment.status = 'cancelled';
      }
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
  },
});

export const { addAppointment, cancelAppointment, setLoading, setError } =
  appointmentSlice.actions;

export default appointmentSlice.reducer;
