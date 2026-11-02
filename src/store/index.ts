import { configureStore } from '@reduxjs/toolkit';
import bookingReducer from './bookingSlice';
import appointmentReducer from './appointmentSlice';
import uiReducer from './uiSlice';

export const store = configureStore({
  reducer: {
    booking: bookingReducer,
    appointments: appointmentReducer,
    ui: uiReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
