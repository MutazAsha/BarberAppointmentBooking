import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { BookingState, Service, Barber } from '../types';

const initialState: BookingState = {
  selectedService: null,
  selectedBarber: null,
  selectedDate: '',
  selectedTime: '',
  customerName: '',
  customerPhone: '',
  customerEmail: '',
  notes: '',
  step: 1,
};

const bookingSlice = createSlice({
  name: 'booking',
  initialState,
  reducers: {
    setSelectedService: (state, action: PayloadAction<Service>) => {
      state.selectedService = action.payload;
    },
    setSelectedBarber: (state, action: PayloadAction<Barber>) => {
      state.selectedBarber = action.payload;
    },
    setSelectedDate: (state, action: PayloadAction<string>) => {
      state.selectedDate = action.payload;
      state.selectedTime = ''; // Reset time when date changes
    },
    setSelectedTime: (state, action: PayloadAction<string>) => {
      state.selectedTime = action.payload;
    },
    setCustomerName: (state, action: PayloadAction<string>) => {
      state.customerName = action.payload;
    },
    setCustomerPhone: (state, action: PayloadAction<string>) => {
      state.customerPhone = action.payload;
    },
    setCustomerEmail: (state, action: PayloadAction<string>) => {
      state.customerEmail = action.payload;
    },
    setNotes: (state, action: PayloadAction<string>) => {
      state.notes = action.payload;
    },
    setStep: (state, action: PayloadAction<number>) => {
      state.step = action.payload;
    },
    nextStep: (state) => {
      state.step += 1;
    },
    prevStep: (state) => {
      state.step -= 1;
    },
    resetBooking: (state) => {
      Object.assign(state, initialState);
    },
  },
});

export const {
  setSelectedService,
  setSelectedBarber,
  setSelectedDate,
  setSelectedTime,
  setCustomerName,
  setCustomerPhone,
  setCustomerEmail,
  setNotes,
  setStep,
  nextStep,
  prevStep,
  resetBooking,
} = bookingSlice.actions;

export default bookingSlice.reducer;
