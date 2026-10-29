import React, { useState, useMemo } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { RootState } from '../store';
import {
  setSelectedService,
  setSelectedBarber,
  setSelectedDate,
  setSelectedTime,
  setCustomerName,
  setCustomerPhone,
  setCustomerEmail,
  setNotes,
  nextStep,
  prevStep,
  resetBooking,
} from '../store/bookingSlice';
import { addAppointment } from '../store/appointmentSlice';
import { services, barbers, generateTimeSlots } from '../data/mockData';
import {
  Scissors,
  User,
  Calendar,
  Clock,
  CheckCircle,
  ChevronRight,
  ChevronLeft,
  Star,
  AlertCircle,
} from 'lucide-react';
import { Appointment } from '../types';

const BookingPage: React.FC = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const booking = useSelector((state: RootState) => state.booking);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const dates = useMemo(() => {
    const result = [];
    const today = new Date();
    for (let i = 1; i <= 14; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() + i);
      if (date.getDay() !== 0) {
        // Exclude Sundays
        result.push({
          value: date.toISOString().split('T')[0],
          day: date.toLocaleDateString('en-US', { weekday: 'short' }),
          date: date.getDate(),
          month: date.toLocaleDateString('en-US', { month: 'short' }),
        });
      }
    }
    return result;
  }, []);

  const timeSlots = useMemo(() => {
    if (booking.selectedDate) {
      return generateTimeSlots(booking.selectedDate);
    }
    return [];
  }, [booking.selectedDate]);

  const validateStep = (step: number): boolean => {
    const newErrors: Record<string, string> = {};

    switch (step) {
      case 1:
        if (!booking.selectedService) newErrors.service = 'Please select a service';
        break;
      case 2:
        if (!booking.selectedBarber) newErrors.barber = 'Please select a barber';
        break;
      case 3:
        if (!booking.selectedDate) newErrors.date = 'Please select a date';
        if (!booking.selectedTime) newErrors.time = 'Please select a time';
        break;
      case 4:
        if (!booking.customerName.trim()) newErrors.name = 'Name is required';
        if (!booking.customerPhone.trim()) newErrors.phone = 'Phone number is required';
        if (!booking.customerEmail.trim()) newErrors.email = 'Email is required';
        else if (!/\S+@\S+\.\S+/.test(booking.customerEmail))
          newErrors.email = 'Please enter a valid email';
        break;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (validateStep(booking.step)) {
      dispatch(nextStep());
    }
  };

  const handlePrev = () => {
    setErrors({});
    dispatch(prevStep());
  };

  const handleConfirm = () => {
    if (validateStep(4)) {
      const appointment: Appointment = {
        id: Date.now().toString(),
        serviceId: booking.selectedService!.id,
        serviceName: booking.selectedService!.name,
        barberId: booking.selectedBarber!.id,
        barberName: booking.selectedBarber!.name,
        date: booking.selectedDate,
        time: booking.selectedTime,
        duration: booking.selectedService!.duration,
        price: booking.selectedService!.price,
        status: 'confirmed',
        customerName: booking.customerName,
        customerPhone: booking.customerPhone,
        customerEmail: booking.customerEmail,
        notes: booking.notes,
        createdAt: new Date().toISOString(),
      };

      dispatch(addAppointment(appointment));
      dispatch(resetBooking());
      navigate('/appointments');
    }
  };

  const steps = [
    { number: 1, label: 'Service', icon: Scissors },
    { number: 2, label: 'Barber', icon: User },
    { number: 3, label: 'Date & Time', icon: Calendar },
    { number: 4, label: 'Details', icon: CheckCircle },
  ];

  return (
    <div className="min-h-screen bg-gray-50 pt-20">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Progress Steps */}
        <div className="mb-8">
          <div className="flex items-center justify-between">
            {steps.map((step, index) => {
              const Icon = step.icon;
              const isCompleted = booking.step > step.number;
              const isCurrent = booking.step === step.number;

              return (
                <React.Fragment key={step.number}>
                  <div className="flex flex-col items-center">
                    <div
                      className={`w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center transition-all ${
                        isCompleted
                          ? 'bg-green-500 text-white'
                          : isCurrent
                          ? 'bg-amber-500 text-white shadow-lg shadow-amber-200'
                          : 'bg-gray-200 text-gray-500'
                      }`}
                    >
                      {isCompleted ? (
                        <CheckCircle className="w-5 h-5 sm:w-6 sm:h-6" />
                      ) : (
                        <Icon className="w-5 h-5 sm:w-6 sm:h-6" />
                      )}
                    </div>
                    <span
                      className={`mt-2 text-xs sm:text-sm font-medium ${
                        isCurrent ? 'text-amber-600' : isCompleted ? 'text-green-600' : 'text-gray-400'
                      }`}
                    >
                      {step.label}
                    </span>
                  </div>
                  {index < steps.length - 1 && (
                    <div
                      className={`flex-1 h-1 mx-2 rounded-full transition-all ${
                        isCompleted ? 'bg-green-500' : 'bg-gray-200'
                      }`}
                    />
                  )}
                </React.Fragment>
              );
            })}
          </div>
        </div>

        {/* Step Content */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 sm:p-8">
          {/* Step 1: Select Service */}
          {booking.step === 1 && (
            <div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Select a Service</h2>
              <p className="text-gray-500 mb-6">Choose the service you'd like to book</p>
              {errors.service && (
                <div className="flex items-center gap-2 text-red-500 text-sm mb-4">
                  <AlertCircle className="w-4 h-4" />
                  {errors.service}
                </div>
              )}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                {services.map((service) => (
                  <button
                    key={service.id}
                    onClick={() => dispatch(setSelectedService(service))}
                    className={`p-4 rounded-xl border-2 text-left transition-all ${
                      booking.selectedService?.id === service.id
                        ? 'border-amber-500 bg-amber-50 shadow-md'
                        : 'border-gray-200 hover:border-amber-300 hover:bg-gray-50'
                    }`}
                  >
                    <div className="flex items-start justify-between">
                      <div>
                        <h3 className="font-semibold text-gray-900">{service.name}</h3>
                        <p className="text-sm text-gray-500 mt-1">{service.description}</p>
                        <div className="flex items-center gap-3 mt-3">
                          <span className="text-sm text-amber-600 font-bold">${service.price}</span>
                          <span className="text-sm text-gray-400 flex items-center gap-1">
                            <Clock className="w-3 h-3" />
                            {service.duration} min
                          </span>
                        </div>
                      </div>
                      <span className="text-2xl">
                        {service.category === 'Hair'
                          ? '✂️'
                          : service.category === 'Beard'
                          ? '🧔'
                          : service.category === 'Shave'
                          ? '🪒'
                          : service.category === 'Combo'
                          ? '💈'
                          : '💆'}
                      </span>
                    </div>
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Step 2: Select Barber */}
          {booking.step === 2 && (
            <div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Choose Your Barber</h2>
              <p className="text-gray-500 mb-6">Select your preferred barber for this appointment</p>
              {errors.barber && (
                <div className="flex items-center gap-2 text-red-500 text-sm mb-4">
                  <AlertCircle className="w-4 h-4" />
                  {errors.barber}
                </div>
              )}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                {barbers.map((barber) => (
                  <button
                    key={barber.id}
                    onClick={() => barber.available && dispatch(setSelectedBarber(barber))}
                    disabled={!barber.available}
                    className={`p-5 rounded-xl border-2 text-left transition-all ${
                      !barber.available
                        ? 'border-gray-100 bg-gray-50 opacity-50 cursor-not-allowed'
                        : booking.selectedBarber?.id === barber.id
                        ? 'border-amber-500 bg-amber-50 shadow-md'
                        : 'border-gray-200 hover:border-amber-300 hover:bg-gray-50'
                    }`}
                  >
                    <div className="flex items-center gap-4">
                      <div className="text-4xl">{barber.avatar}</div>
                      <div className="flex-1">
                        <h3 className="font-semibold text-gray-900">{barber.name}</h3>
                        <p className="text-sm text-amber-600">{barber.specialty}</p>
                        <div className="flex items-center gap-2 mt-1">
                          <Star className="w-3 h-3 text-amber-400 fill-amber-400" />
                          <span className="text-xs text-gray-600">
                            {barber.rating} ({barber.reviews})
                          </span>
                          <span className="text-xs text-gray-400">• {barber.experience}yr exp</span>
                        </div>
                      </div>
                      {!barber.available && (
                        <span className="text-xs text-red-500 font-medium bg-red-50 px-2 py-1 rounded-full">
                          Unavailable
                        </span>
                      )}
                    </div>
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Step 3: Select Date & Time */}
          {booking.step === 3 && (
            <div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Pick Date & Time</h2>
              <p className="text-gray-500 mb-6">Choose your preferred date and time slot</p>

              {/* Date Selection */}
              <div className="mb-6">
                <label className="block text-sm font-semibold text-gray-700 mb-3">
                  <Calendar className="w-4 h-4 inline mr-1" />
                  Select Date
                </label>
                {errors.date && (
                  <div className="flex items-center gap-2 text-red-500 text-sm mb-2">
                    <AlertCircle className="w-4 h-4" />
                    {errors.date}
                  </div>
                )}
                <div className="flex gap-2 overflow-x-auto pb-2">
                  {dates.map((d) => (
                    <button
                      key={d.value}
                      onClick={() => dispatch(setSelectedDate(d.value))}
                      className={`flex-shrink-0 w-16 py-3 rounded-xl border-2 text-center transition-all ${
                        booking.selectedDate === d.value
                          ? 'border-amber-500 bg-amber-50 shadow-md'
                          : 'border-gray-200 hover:border-amber-300'
                      }`}
                    >
                      <p className="text-xs text-gray-500">{d.day}</p>
                      <p className="text-lg font-bold text-gray-900">{d.date}</p>
                      <p className="text-xs text-gray-500">{d.month}</p>
                    </button>
                  ))}
                </div>
              </div>

              {/* Time Selection */}
              {booking.selectedDate && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-3">
                    <Clock className="w-4 h-4 inline mr-1" />
                    Select Time
                  </label>
                  {errors.time && (
                    <div className="flex items-center gap-2 text-red-500 text-sm mb-2">
                      <AlertCircle className="w-4 h-4" />
                      {errors.time}
                    </div>
                  )}
                  <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-6 gap-2">
                    {timeSlots.map((slot) => (
                      <button
                        key={slot.id}
                        onClick={() => slot.available && dispatch(setSelectedTime(slot.time))}
                        disabled={!slot.available}
                        className={`py-2.5 px-3 rounded-lg border text-sm font-medium transition-all ${
                          !slot.available
                            ? 'border-gray-100 bg-gray-50 text-gray-300 cursor-not-allowed line-through'
                            : booking.selectedTime === slot.time
                            ? 'border-amber-500 bg-amber-500 text-white shadow-md'
                            : 'border-gray-200 text-gray-700 hover:border-amber-300 hover:bg-amber-50'
                        }`}
                      >
                        {slot.time}
                      </button>
                    ))}
                  </div>
                </div>
              )}
            </div>
          )}

          {/* Step 4: Customer Details */}
          {booking.step === 4 && (
            <div>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">Your Details</h2>
              <p className="text-gray-500 mb-6">Please provide your contact information</p>

              {/* Booking Summary */}
              <div className="bg-amber-50 border border-amber-200 rounded-xl p-4 mb-6">
                <h3 className="font-semibold text-amber-800 mb-2">Booking Summary</h3>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div>
                    <span className="text-gray-500">Service:</span>{' '}
                    <span className="font-medium text-gray-900">{booking.selectedService?.name}</span>
                  </div>
                  <div>
                    <span className="text-gray-500">Barber:</span>{' '}
                    <span className="font-medium text-gray-900">{booking.selectedBarber?.name}</span>
                  </div>
                  <div>
                    <span className="text-gray-500">Date:</span>{' '}
                    <span className="font-medium text-gray-900">
                      {new Date(booking.selectedDate).toLocaleDateString('en-US', {
                        weekday: 'short',
                        month: 'short',
                        day: 'numeric',
                      })}
                    </span>
                  </div>
                  <div>
                    <span className="text-gray-500">Time:</span>{' '}
                    <span className="font-medium text-gray-900">{booking.selectedTime}</span>
                  </div>
                  <div>
                    <span className="text-gray-500">Duration:</span>{' '}
                    <span className="font-medium text-gray-900">
                      {booking.selectedService?.duration} min
                    </span>
                  </div>
                  <div>
                    <span className="text-gray-500">Price:</span>{' '}
                    <span className="font-bold text-amber-700">
                      ${booking.selectedService?.price}
                    </span>
                  </div>
                </div>
              </div>

              {/* Form Fields */}
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Full Name *
                  </label>
                  <input
                    type="text"
                    value={booking.customerName}
                    onChange={(e) => dispatch(setCustomerName(e.target.value))}
                    className={`w-full px-4 py-3 rounded-xl border ${
                      errors.name ? 'border-red-300 bg-red-50' : 'border-gray-200'
                    } focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent`}
                    placeholder="John Doe"
                  />
                  {errors.name && (
                    <p className="text-red-500 text-xs mt-1">{errors.name}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Phone Number *
                  </label>
                  <input
                    type="tel"
                    value={booking.customerPhone}
                    onChange={(e) => dispatch(setCustomerPhone(e.target.value))}
                    className={`w-full px-4 py-3 rounded-xl border ${
                      errors.phone ? 'border-red-300 bg-red-50' : 'border-gray-200'
                    } focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent`}
                    placeholder="+1 (555) 123-4567"
                  />
                  {errors.phone && (
                    <p className="text-red-500 text-xs mt-1">{errors.phone}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Email Address *
                  </label>
                  <input
                    type="email"
                    value={booking.customerEmail}
                    onChange={(e) => dispatch(setCustomerEmail(e.target.value))}
                    className={`w-full px-4 py-3 rounded-xl border ${
                      errors.email ? 'border-red-300 bg-red-50' : 'border-gray-200'
                    } focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent`}
                    placeholder="john@example.com"
                  />
                  {errors.email && (
                    <p className="text-red-500 text-xs mt-1">{errors.email}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Notes (Optional)
                  </label>
                  <textarea
                    value={booking.notes}
                    onChange={(e) => dispatch(setNotes(e.target.value))}
                    rows={3}
                    className="w-full px-4 py-3 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
                    placeholder="Any special requests or notes..."
                  />
                </div>
              </div>
            </div>
          )}

          {/* Navigation Buttons */}
          <div className="flex items-center justify-between mt-8 pt-6 border-t border-gray-100">
            {booking.step > 1 ? (
              <button
                onClick={handlePrev}
                className="flex items-center gap-2 px-5 py-2.5 text-gray-600 font-medium hover:text-gray-900 transition-colors"
              >
                <ChevronLeft className="w-4 h-4" />
                Back
              </button>
            ) : (
              <div />
            )}

            {booking.step < 4 ? (
              <button
                onClick={handleNext}
                className="flex items-center gap-2 bg-gradient-to-r from-amber-500 to-amber-600 text-white px-6 py-3 rounded-xl font-semibold hover:from-amber-600 hover:to-amber-700 transition-all shadow-md"
              >
                Continue
                <ChevronRight className="w-4 h-4" />
              </button>
            ) : (
              <button
                onClick={handleConfirm}
                className="flex items-center gap-2 bg-gradient-to-r from-green-500 to-green-600 text-white px-6 py-3 rounded-xl font-semibold hover:from-green-600 hover:to-green-700 transition-all shadow-md"
              >
                <CheckCircle className="w-4 h-4" />
                Confirm Booking
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default BookingPage;
