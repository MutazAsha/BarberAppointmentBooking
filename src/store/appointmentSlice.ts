import { createSlice, PayloadAction, createAsyncThunk } from '@reduxjs/toolkit';
import { Appointment } from '../types';
import { api } from '../services/api';

interface AppointmentState {
  appointments: Appointment[];
  loading: boolean;
  error: string | null;
}

const initialState: AppointmentState = {
  appointments: [],
  loading: false,
  error: null,
};

// Async Thunks
export const fetchAppointments = createAsyncThunk(
  'appointments/fetchAll',
  async () => {
    return await api.getAppointments();
  }
);

export const fetchAppointmentsByEmail = createAsyncThunk(
  'appointments/fetchByEmail',
  async (email: string) => {
    return await api.getAppointmentsByEmail(email);
  }
);

export const createAppointment = createAsyncThunk(
  'appointments/create',
  async (appointment: Omit<Appointment, 'id' | 'createdAt'>) => {
    return await api.createAppointment(appointment);
  }
);

export const cancelAppointmentAsync = createAsyncThunk(
  'appointments/cancel',
  async (id: string) => {
    await api.cancelAppointment(id);
    return id;
  }
);

const appointmentSlice = createSlice({
  name: 'appointments',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all
      .addCase(fetchAppointments.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchAppointments.fulfilled, (state, action) => {
        state.loading = false;
        state.appointments = action.payload;
      })
      .addCase(fetchAppointments.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to fetch appointments';
      })
      // Fetch by email
      .addCase(fetchAppointmentsByEmail.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchAppointmentsByEmail.fulfilled, (state, action) => {
        state.loading = false;
        state.appointments = action.payload;
      })
      // Create
      .addCase(createAppointment.pending, (state) => {
        state.loading = true;
      })
      .addCase(createAppointment.fulfilled, (state, action) => {
        state.loading = false;
        state.appointments.push(action.payload);
      })
      .addCase(createAppointment.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Failed to create appointment';
      })
      // Cancel
      .addCase(cancelAppointmentAsync.fulfilled, (state, action) => {
        const appointment = state.appointments.find(a => a.id === action.payload);
        if (appointment) {
          appointment.status = 'cancelled';
        }
      });
  },
});

export const { clearError } = appointmentSlice.actions;

export default appointmentSlice.reducer;
