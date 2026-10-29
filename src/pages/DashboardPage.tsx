import React, { useState, useEffect } from 'react';
import { api } from '../services/api';
import { Appointment, Barber, Service } from '../types';
import {
  Calendar,
  Users,
  DollarSign,
  Clock,
  CheckCircle,
  XCircle,
  TrendingUp,
  BarChart3,
  Loader2,
  RefreshCw,
  AlertCircle,
} from 'lucide-react';
import { useToast } from '../components/Toast';

interface Stats {
  totalAppointments: number;
  todayAppointments: number;
  pendingAppointments: number;
  confirmedAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
  totalRevenue: number;
}

const DashboardPage: React.FC = () => {
  const { showToast } = useToast();
  const [stats, setStats] = useState<Stats | null>(null);
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [barbers, setBarbers] = useState<Barber[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'overview' | 'appointments' | 'barbers'>('overview');

  const loadData = async () => {
    setLoading(true);
    try {
      const [statsData, appointmentsData, barbersData, servicesData] = await Promise.all([
        api.getStats(),
        api.getAppointments(),
        api.getBarbers(),
        api.getServices(),
      ]);
      setStats(statsData);
      setAppointments(appointmentsData);
      setBarbers(barbersData);
      setServices(servicesData);
    } catch (err) {
      showToast('error', 'Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const todayAppointments = appointments.filter(
    a => a.date === new Date().toISOString().split('T')[0] && a.status !== 'cancelled'
  );

  const barberWorkload = barbers.map(barber => ({
    ...barber,
    appointmentCount: appointments.filter(a => a.barberId === barber.id && a.status !== 'cancelled').length,
    revenue: appointments
      .filter(a => a.barberId === barber.id && a.status !== 'cancelled')
      .reduce((sum, a) => sum + a.price, 0),
  }));

  const servicePopularity = services.map(service => ({
    ...service,
    bookingCount: appointments.filter(a => a.serviceId === service.id && a.status !== 'cancelled').length,
  })).sort((a, b) => b.bookingCount - a.bookingCount);

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 pt-20 flex items-center justify-center">
        <Loader2 className="w-10 h-10 text-amber-500 animate-spin" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 pt-20">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-8">
          <div>
            <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-2">
              <BarChart3 className="w-8 h-8 text-amber-500" />
              Dashboard
            </h1>
            <p className="text-gray-500 mt-1">Real-time overview of your barber shop</p>
          </div>
          <button
            onClick={loadData}
            className="flex items-center gap-2 px-4 py-2.5 border border-gray-200 rounded-xl text-sm font-medium text-gray-600 hover:bg-gray-50 transition-colors"
          >
            <RefreshCw className="w-4 h-4" />
            Refresh Data
          </button>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <StatCard
            icon={<Calendar className="w-6 h-6 text-blue-500" />}
            title="Today's Bookings"
            value={stats?.todayAppointments || 0}
            bgColor="bg-blue-50"
          />
          <StatCard
            icon={<DollarSign className="w-6 h-6 text-green-500" />}
            title="Total Revenue"
            value={`$${stats?.totalRevenue || 0}`}
            bgColor="bg-green-50"
          />
          <StatCard
            icon={<CheckCircle className="w-6 h-6 text-amber-500" />}
            title="Confirmed"
            value={stats?.confirmedAppointments || 0}
            bgColor="bg-amber-50"
          />
          <StatCard
            icon={<Clock className="w-6 h-6 text-purple-500" />}
            title="Pending"
            value={stats?.pendingAppointments || 0}
            bgColor="bg-purple-50"
          />
        </div>

        {/* Tabs */}
        <div className="flex gap-2 mb-6 border-b border-gray-200">
          {(['overview', 'appointments', 'barbers'] as const).map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`px-4 py-3 text-sm font-medium border-b-2 transition-all ${
                activeTab === tab
                  ? 'border-amber-500 text-amber-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700'
              }`}
            >
              {tab.charAt(0).toUpperCase() + tab.slice(1)}
            </button>
          ))}
        </div>

        {/* Tab Content */}
        {activeTab === 'overview' && (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Today's Schedule */}
            <div className="bg-white rounded-2xl p-6 border border-gray-100 shadow-sm">
              <h3 className="font-bold text-gray-900 text-lg mb-4 flex items-center gap-2">
                <Calendar className="w-5 h-5 text-amber-500" />
                Today's Schedule
              </h3>
              {todayAppointments.length > 0 ? (
                <div className="space-y-3">
                  {todayAppointments
                    .sort((a, b) => a.time.localeCompare(b.time))
                    .map((apt) => (
                      <div key={apt.id} className="flex items-center gap-3 p-3 bg-gray-50 rounded-xl">
                        <div className="text-center min-w-[50px]">
                          <p className="text-sm font-bold text-amber-600">{apt.time}</p>
                          <p className="text-xs text-gray-400">{apt.duration}min</p>
                        </div>
                        <div className="w-px h-10 bg-gray-200" />
                        <div className="flex-1 min-w-0">
                          <p className="font-medium text-gray-900 text-sm truncate">{apt.serviceName}</p>
                          <p className="text-xs text-gray-500">{apt.customerName} • {apt.barberName}</p>
                        </div>
                        <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${
                          apt.status === 'confirmed' ? 'bg-green-100 text-green-700' : 'bg-yellow-100 text-yellow-700'
                        }`}>
                          {apt.status}
                        </span>
                      </div>
                    ))}
                </div>
              ) : (
                <div className="text-center py-8 text-gray-400">
                  <Calendar className="w-10 h-10 mx-auto mb-2 opacity-50" />
                  <p className="text-sm">No appointments today</p>
                </div>
              )}
            </div>

            {/* Service Popularity */}
            <div className="bg-white rounded-2xl p-6 border border-gray-100 shadow-sm">
              <h3 className="font-bold text-gray-900 text-lg mb-4 flex items-center gap-2">
                <TrendingUp className="w-5 h-5 text-amber-500" />
                Service Popularity
              </h3>
              <div className="space-y-3">
                {servicePopularity.slice(0, 5).map((service, index) => {
                  const maxBookings = Math.max(...servicePopularity.map(s => s.bookingCount), 1);
                  const percentage = (service.bookingCount / maxBookings) * 100;
                  return (
                    <div key={service.id}>
                      <div className="flex items-center justify-between mb-1">
                        <span className="text-sm font-medium text-gray-700 flex items-center gap-2">
                          <span className="text-amber-500 font-bold">#{index + 1}</span>
                          {service.name}
                        </span>
                        <span className="text-sm text-gray-500">{service.bookingCount} bookings</span>
                      </div>
                      <div className="w-full bg-gray-100 rounded-full h-2">
                        <div
                          className="bg-gradient-to-r from-amber-400 to-amber-600 h-2 rounded-full transition-all duration-500"
                          style={{ width: `${percentage}%` }}
                        />
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Revenue by Service */}
            <div className="bg-white rounded-2xl p-6 border border-gray-100 shadow-sm">
              <h3 className="font-bold text-gray-900 text-lg mb-4 flex items-center gap-2">
                <DollarSign className="w-5 h-5 text-green-500" />
                Revenue Breakdown
              </h3>
              <div className="space-y-3">
                {servicePopularity.slice(0, 5).map((service) => {
                  const revenue = appointments
                    .filter(a => a.serviceId === service.id && a.status !== 'cancelled')
                    .reduce((sum, a) => sum + a.price, 0);
                  const maxRevenue = Math.max(
                    ...servicePopularity.map(s =>
                      appointments.filter(a => a.serviceId === s.id && a.status !== 'cancelled')
                        .reduce((sum, a) => sum + a.price, 0)
                    ), 1
                  );
                  return (
                    <div key={service.id} className="flex items-center gap-3">
                      <div className="flex-1">
                        <div className="flex justify-between mb-1">
                          <span className="text-sm font-medium text-gray-700">{service.name}</span>
                          <span className="text-sm font-bold text-green-600">${revenue}</span>
                        </div>
                        <div className="w-full bg-gray-100 rounded-full h-2">
                          <div
                            className="bg-gradient-to-r from-green-400 to-green-600 h-2 rounded-full transition-all duration-500"
                            style={{ width: `${(revenue / maxRevenue) * 100}%` }}
                          />
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Appointment Status Distribution */}
            <div className="bg-white rounded-2xl p-6 border border-gray-100 shadow-sm">
              <h3 className="font-bold text-gray-900 text-lg mb-4 flex items-center gap-2">
                <AlertCircle className="w-5 h-5 text-amber-500" />
                Status Distribution
              </h3>
              <div className="grid grid-cols-2 gap-4">
                <StatusBlock label="Confirmed" count={stats?.confirmedAppointments || 0} color="green" />
                <StatusBlock label="Pending" count={stats?.pendingAppointments || 0} color="yellow" />
                <StatusBlock label="Completed" count={stats?.completedAppointments || 0} color="blue" />
                <StatusBlock label="Cancelled" count={stats?.cancelledAppointments || 0} color="red" />
              </div>
              <div className="mt-4 pt-4 border-t border-gray-100">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Total Appointments</span>
                  <span className="font-bold text-gray-900">{stats?.totalAppointments || 0}</span>
                </div>
              </div>
            </div>
          </div>
        )}

        {activeTab === 'appointments' && (
          <div className="bg-white rounded-2xl border border-gray-100 shadow-sm overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-100">
                  <tr>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Customer</th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Service</th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Barber</th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Date</th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Time</th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Status</th>
                    <th className="text-left px-6 py-3 text-xs font-semibold text-gray-500 uppercase">Price</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {appointments
                    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
                    .slice(0, 20)
                    .map((apt) => (
                      <tr key={apt.id} className="hover:bg-gray-50 transition-colors">
                        <td className="px-6 py-4">
                          <p className="font-medium text-gray-900 text-sm">{apt.customerName}</p>
                          <p className="text-xs text-gray-500">{apt.customerEmail}</p>
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-700">{apt.serviceName}</td>
                        <td className="px-6 py-4 text-sm text-gray-700">{apt.barberName}</td>
                        <td className="px-6 py-4 text-sm text-gray-700">
                          {new Date(apt.date + 'T00:00:00').toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                        </td>
                        <td className="px-6 py-4 text-sm text-gray-700">{apt.time}</td>
                        <td className="px-6 py-4">
                          <span className={`text-xs px-2 py-1 rounded-full font-medium ${
                            apt.status === 'confirmed' ? 'bg-green-100 text-green-700' :
                            apt.status === 'pending' ? 'bg-yellow-100 text-yellow-700' :
                            apt.status === 'completed' ? 'bg-blue-100 text-blue-700' :
                            'bg-red-100 text-red-700'
                          }`}>
                            {apt.status}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-sm font-medium text-gray-900">${apt.price}</td>
                      </tr>
                    ))}
                </tbody>
              </table>
            </div>
            {appointments.length === 0 && (
              <div className="text-center py-12 text-gray-400">
                <Calendar className="w-10 h-10 mx-auto mb-2 opacity-50" />
                <p>No appointments yet</p>
              </div>
            )}
          </div>
        )}

        {activeTab === 'barbers' && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {barberWorkload.map((barber) => (
              <div key={barber.id} className="bg-white rounded-2xl p-6 border border-gray-100 shadow-sm">
                <div className="flex items-center gap-4 mb-4">
                  <div className="text-4xl">{barber.avatar}</div>
                  <div>
                    <h3 className="font-bold text-gray-900">{barber.name}</h3>
                    <p className="text-sm text-amber-600">{barber.specialty}</p>
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div className="bg-gray-50 rounded-xl p-3 text-center">
                    <p className="text-2xl font-bold text-gray-900">{barber.appointmentCount}</p>
                    <p className="text-xs text-gray-500">Appointments</p>
                  </div>
                  <div className="bg-gray-50 rounded-xl p-3 text-center">
                    <p className="text-2xl font-bold text-green-600">${barber.revenue}</p>
                    <p className="text-xs text-gray-500">Revenue</p>
                  </div>
                </div>
                <div className="mt-4 flex items-center justify-between">
                  <span className={`text-xs px-2 py-1 rounded-full font-medium ${
                    barber.available ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'
                  }`}>
                    {barber.available ? 'Available' : 'Off Duty'}
                  </span>
                  <span className="text-xs text-gray-400">{barber.experience} years exp</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

// Stat Card Component
const StatCard: React.FC<{
  icon: React.ReactNode;
  title: string;
  value: string | number;
  bgColor: string;
}> = ({ icon, title, value, bgColor }) => (
  <div className="bg-white rounded-2xl p-5 border border-gray-100 shadow-sm">
    <div className={`w-12 h-12 ${bgColor} rounded-xl flex items-center justify-center mb-3`}>
      {icon}
    </div>
    <p className="text-2xl font-bold text-gray-900">{value}</p>
    <p className="text-sm text-gray-500">{title}</p>
  </div>
);

// Status Block Component
const StatusBlock: React.FC<{
  label: string;
  count: number;
  color: string;
}> = ({ label, count, color }) => {
  const colorClasses: Record<string, string> = {
    green: 'bg-green-100 text-green-700',
    yellow: 'bg-yellow-100 text-yellow-700',
    blue: 'bg-blue-100 text-blue-700',
    red: 'bg-red-100 text-red-700',
  };
  return (
    <div className={`${colorClasses[color]} rounded-xl p-4 text-center`}>
      <p className="text-2xl font-bold">{count}</p>
      <p className="text-xs font-medium">{label}</p>
    </div>
  );
};

export default DashboardPage;
