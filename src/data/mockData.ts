import { Service, Barber, TimeSlot } from '../types';

export const services: Service[] = [
  {
    id: '1',
    name: 'Classic Haircut',
    description: 'Traditional haircut with precision cutting and styling',
    duration: 30,
    price: 25,
    category: 'Hair',
  },
  {
    id: '2',
    name: 'Beard Trim & Shape',
    description: 'Expert beard trimming, shaping, and conditioning',
    duration: 20,
    price: 15,
    category: 'Beard',
  },
  {
    id: '3',
    name: 'Hot Towel Shave',
    description: 'Luxurious hot towel straight razor shave experience',
    duration: 45,
    price: 35,
    category: 'Shave',
  },
  {
    id: '4',
    name: 'Hair & Beard Combo',
    description: 'Complete haircut and beard grooming package',
    duration: 50,
    price: 35,
    category: 'Combo',
  },
  {
    id: '5',
    name: 'Kids Haircut',
    description: 'Fun and friendly haircut for children under 12',
    duration: 20,
    price: 18,
    category: 'Hair',
  },
  {
    id: '6',
    name: 'Hair Coloring',
    description: 'Professional hair coloring with premium products',
    duration: 60,
    price: 55,
    category: 'Hair',
  },
  {
    id: '7',
    name: 'Scalp Treatment',
    description: 'Relaxing scalp massage and treatment for healthy hair',
    duration: 30,
    price: 30,
    category: 'Treatment',
  },
  {
    id: '8',
    name: 'Premium Package',
    description: 'Haircut, beard trim, hot towel shave, and scalp massage',
    duration: 90,
    price: 75,
    category: 'Combo',
  },
];

export const barbers: Barber[] = [
  {
    id: '1',
    name: 'James Wilson',
    avatar: '👨‍🦱',
    specialty: 'Classic Cuts & Fades',
    rating: 4.9,
    reviews: 234,
    experience: 8,
    available: true,
  },
  {
    id: '2',
    name: 'Marcus Johnson',
    avatar: '👨‍🦲',
    specialty: 'Beard Styling & Design',
    rating: 4.8,
    reviews: 189,
    experience: 6,
    available: true,
  },
  {
    id: '3',
    name: 'David Chen',
    avatar: '👨',
    specialty: 'Modern Styles & Color',
    rating: 4.7,
    reviews: 156,
    experience: 5,
    available: true,
  },
  {
    id: '4',
    name: 'Alex Rivera',
    avatar: '🧔',
    specialty: 'Hot Towel Shaves',
    rating: 4.9,
    reviews: 312,
    experience: 12,
    available: true,
  },
  {
    id: '5',
    name: 'Ryan Thompson',
    avatar: '👱‍♂️',
    specialty: 'Kids & Teen Cuts',
    rating: 4.6,
    reviews: 98,
    experience: 3,
    available: false,
  },
];

export const generateTimeSlots = (date: string): TimeSlot[] => {
  const slots: TimeSlot[] = [];
  const hours = [9, 10, 11, 12, 13, 14, 15, 16, 17];
  const minutes = ['00', '30'];

  hours.forEach((hour) => {
    minutes.forEach((min) => {
      const timeStr = `${hour.toString().padStart(2, '0')}:${min}`;
      // Simulate some slots being unavailable
      const isAvailable = Math.random() > 0.3;
      slots.push({
        id: `${date}-${timeStr}`,
        time: timeStr,
        available: isAvailable,
      });
    });
  });

  return slots;
};

export const categories = ['All', 'Hair', 'Beard', 'Shave', 'Combo', 'Treatment'];
