// ============================================
// INFRASTRUCTURE LAYER - Data Storage
// ============================================
// Simulates database using JSON file storage
// In production, replace with PostgreSQL/SQL Server

import fs from 'fs';
import path from 'path';
import { Service, Barber, Appointment, ContactMessage } from '../domain/entities';

const DATA_DIR = path.join(__dirname, '../../data');
const SERVICES_FILE = path.join(DATA_DIR, 'services.json');
const BARBERS_FILE = path.join(DATA_DIR, 'barbers.json');
const APPOINTMENTS_FILE = path.join(DATA_DIR, 'appointments.json');
const MESSAGES_FILE = path.join(DATA_DIR, 'messages.json');

// Ensure data directory exists
if (!fs.existsSync(DATA_DIR)) {
  fs.mkdirSync(DATA_DIR, { recursive: true });
}

// ============================================
// SEED DATA
// ============================================

const seedServices: Service[] = [
  {
    id: '1',
    name: 'Classic Haircut',
    description: 'Traditional haircut with precision cutting and styling',
    duration: 30,
    price: 25,
    category: 'Hair',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '2',
    name: 'Beard Trim & Shape',
    description: 'Expert beard trimming, shaping, and conditioning',
    duration: 20,
    price: 15,
    category: 'Beard',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '3',
    name: 'Hot Towel Shave',
    description: 'Luxurious hot towel straight razor shave experience',
    duration: 45,
    price: 35,
    category: 'Shave',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '4',
    name: 'Hair & Beard Combo',
    description: 'Complete haircut and beard grooming package',
    duration: 50,
    price: 35,
    category: 'Combo',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '5',
    name: 'Kids Haircut',
    description: 'Fun and friendly haircut for children under 12',
    duration: 20,
    price: 18,
    category: 'Hair',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '6',
    name: 'Hair Coloring',
    description: 'Professional hair coloring with premium products',
    duration: 60,
    price: 55,
    category: 'Hair',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '7',
    name: 'Scalp Treatment',
    description: 'Relaxing scalp massage and treatment for healthy hair',
    duration: 30,
    price: 30,
    category: 'Treatment',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
  {
    id: '8',
    name: 'Premium Package',
    description: 'Haircut, beard trim, hot towel shave, and scalp massage',
    duration: 90,
    price: 75,
    category: 'Combo',
    isActive: true,
    createdAt: new Date().toISOString(),
  },
];

const seedBarbers: Barber[] = [
  {
    id: '1',
    name: 'James Wilson',
    avatar: '👨‍🦱',
    specialty: 'Classic Cuts & Fades',
    rating: 4.9,
    reviews: 234,
    experience: 8,
    available: true,
    createdAt: new Date().toISOString(),
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
    createdAt: new Date().toISOString(),
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
    createdAt: new Date().toISOString(),
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
    createdAt: new Date().toISOString(),
  },
  {
    id: '5',
    name: 'Ryan Thompson',
    avatar: '👱‍♂️',
    specialty: 'Kids & Teen Cuts',
    rating: 4.6,
    reviews: 98,
    experience: 3,
    available: true,
    createdAt: new Date().toISOString(),
  },
];

// ============================================
// DATA ACCESS HELPERS
// ============================================

function readData<T>(filePath: string, seedData: T[]): T[] {
  try {
    if (!fs.existsSync(filePath)) {
      fs.writeFileSync(filePath, JSON.stringify(seedData, null, 2));
      return seedData;
    }
    const data = fs.readFileSync(filePath, 'utf-8');
    return JSON.parse(data);
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error);
    return seedData;
  }
}

function writeData<T>(filePath: string, data: T[]): void {
  try {
    fs.writeFileSync(filePath, JSON.stringify(data, null, 2));
  } catch (error) {
    console.error(`Error writing ${filePath}:`, error);
  }
}

// ============================================
// EXPORTED DATA FUNCTIONS
// ============================================

export const dataStore = {
  // Services
  getServices: (): Service[] => readData(SERVICES_FILE, seedServices),
  saveServices: (data: Service[]) => writeData(SERVICES_FILE, data),

  // Barbers
  getBarbers: (): Barber[] => readData(BARBERS_FILE, seedBarbers),
  saveBarbers: (data: Barber[]) => writeData(BARBERS_FILE, data),

  // Appointments
  getAppointments: (): Appointment[] => readData(APPOINTMENTS_FILE, []),
  saveAppointments: (data: Appointment[]) => writeData(APPOINTMENTS_FILE, data),

  // Messages
  getMessages: (): ContactMessage[] => readData(MESSAGES_FILE, []),
  saveMessages: (data: ContactMessage[]) => writeData(MESSAGES_FILE, data),
};
