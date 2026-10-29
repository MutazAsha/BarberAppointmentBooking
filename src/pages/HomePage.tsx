import React from 'react';
import { Link } from 'react-router-dom';
import { Calendar, Star, Clock, Award, Users, ChevronRight, Sparkles } from 'lucide-react';
import { services, barbers } from '../data/mockData';

const HomePage: React.FC = () => {
  const topBarbers = barbers.filter((b) => b.available).slice(0, 3);
  const popularServices = services.slice(0, 4);

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="relative min-h-[90vh] flex items-center overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-gray-900 via-gray-800 to-amber-900" />
        <div className="absolute inset-0 opacity-20">
          <div className="absolute top-20 left-10 w-72 h-72 bg-amber-500 rounded-full blur-3xl" />
          <div className="absolute bottom-20 right-10 w-96 h-96 bg-amber-700 rounded-full blur-3xl" />
        </div>
        <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-20">
          <div className="grid lg:grid-cols-2 gap-12 items-center">
            <div className="space-y-8">
              <div className="inline-flex items-center gap-2 bg-amber-500/10 border border-amber-500/20 rounded-full px-4 py-2">
                <Sparkles className="w-4 h-4 text-amber-400" />
                <span className="text-sm text-amber-300 font-medium">Premium Grooming Experience</span>
              </div>
              <h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold text-white leading-tight">
                Where Style Meets{' '}
                <span className="text-transparent bg-clip-text bg-gradient-to-r from-amber-400 to-amber-600">
                  Precision
                </span>
              </h1>
              <p className="text-lg text-gray-300 max-w-lg leading-relaxed">
                Experience world-class barbering with our team of master stylists. Book your appointment
                today and discover the art of modern grooming.
              </p>
              <div className="flex flex-col sm:flex-row gap-4">
                <Link
                  to="/booking"
                  className="inline-flex items-center justify-center gap-2 bg-gradient-to-r from-amber-500 to-amber-600 text-white px-8 py-4 rounded-xl text-lg font-semibold hover:from-amber-600 hover:to-amber-700 transition-all shadow-lg hover:shadow-xl transform hover:-translate-y-0.5"
                >
                  <Calendar className="w-5 h-5" />
                  Book Appointment
                </Link>
                <Link
                  to="/services"
                  className="inline-flex items-center justify-center gap-2 bg-white/10 backdrop-blur-sm border border-white/20 text-white px-8 py-4 rounded-xl text-lg font-semibold hover:bg-white/20 transition-all"
                >
                  View Services
                  <ChevronRight className="w-5 h-5" />
                </Link>
              </div>
              <div className="flex items-center gap-8 pt-4">
                <div className="text-center">
                  <p className="text-2xl font-bold text-white">10+</p>
                  <p className="text-xs text-gray-400">Years Experience</p>
                </div>
                <div className="w-px h-10 bg-gray-700" />
                <div className="text-center">
                  <p className="text-2xl font-bold text-white">5K+</p>
                  <p className="text-xs text-gray-400">Happy Clients</p>
                </div>
                <div className="w-px h-10 bg-gray-700" />
                <div className="text-center">
                  <p className="text-2xl font-bold text-white">4.9</p>
                  <p className="text-xs text-gray-400">Avg Rating</p>
                </div>
              </div>
            </div>
            <div className="hidden lg:block relative">
              <div className="relative w-full aspect-square max-w-md mx-auto">
                <div className="absolute inset-0 bg-gradient-to-br from-amber-500/20 to-amber-700/20 rounded-3xl rotate-6" />
                <div className="absolute inset-0 bg-gradient-to-br from-gray-800 to-gray-900 rounded-3xl shadow-2xl flex items-center justify-center border border-gray-700">
                  <div className="text-center space-y-6 p-8">
                    <div className="text-8xl">💈</div>
                    <div className="space-y-2">
                      <p className="text-amber-400 font-semibold text-lg">Next Available</p>
                      <p className="text-white text-3xl font-bold">Today 2:30 PM</p>
                      <p className="text-gray-400 text-sm">Walk-ins welcome</p>
                    </div>
                    <Link
                      to="/booking"
                      className="inline-block bg-amber-500 text-white px-6 py-3 rounded-xl font-semibold hover:bg-amber-600 transition-colors"
                    >
                      Book Now →
                    </Link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
            {[
              { icon: Award, title: 'Expert Barbers', desc: 'Master craftsmen' },
              { icon: Clock, title: 'Quick Service', desc: 'No long waits' },
              { icon: Star, title: '5-Star Rated', desc: 'Top quality' },
              { icon: Users, title: 'All Welcome', desc: 'Everyone welcome' },
            ].map((feature, i) => (
              <div
                key={i}
                className="text-center p-6 rounded-2xl bg-gray-50 hover:bg-amber-50 transition-colors group"
              >
                <div className="w-12 h-12 bg-amber-100 rounded-xl flex items-center justify-center mx-auto mb-3 group-hover:bg-amber-200 transition-colors">
                  <feature.icon className="w-6 h-6 text-amber-600" />
                </div>
                <h3 className="font-semibold text-gray-900 text-sm">{feature.title}</h3>
                <p className="text-xs text-gray-500 mt-1">{feature.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Popular Services */}
      <section className="py-16 bg-gray-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl sm:text-4xl font-bold text-gray-900">Popular Services</h2>
            <p className="mt-3 text-gray-600 max-w-2xl mx-auto">
              Choose from our wide range of premium grooming services
            </p>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {popularServices.map((service) => (
              <div
                key={service.id}
                className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-lg transition-all duration-300 border border-gray-100 group hover:-translate-y-1"
              >
                <div className="w-12 h-12 bg-amber-100 rounded-xl flex items-center justify-center mb-4 group-hover:bg-amber-500 transition-colors">
                  <span className="text-2xl">
                    {service.category === 'Hair' ? '✂️' : service.category === 'Beard' ? '🧔' : service.category === 'Shave' ? '🪒' : '💆'}
                  </span>
                </div>
                <h3 className="font-bold text-gray-900 text-lg">{service.name}</h3>
                <p className="text-sm text-gray-500 mt-2 line-clamp-2">{service.description}</p>
                <div className="flex items-center justify-between mt-4 pt-4 border-t border-gray-100">
                  <span className="text-amber-600 font-bold text-lg">${service.price}</span>
                  <span className="text-sm text-gray-400 flex items-center gap-1">
                    <Clock className="w-3 h-3" />
                    {service.duration} min
                  </span>
                </div>
              </div>
            ))}
          </div>
          <div className="text-center mt-8">
            <Link
              to="/services"
              className="inline-flex items-center gap-2 text-amber-600 font-semibold hover:text-amber-700 transition-colors"
            >
              View All Services
              <ChevronRight className="w-4 h-4" />
            </Link>
          </div>
        </div>
      </section>

      {/* Our Barbers */}
      <section className="py-16 bg-white">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-12">
            <h2 className="text-3xl sm:text-4xl font-bold text-gray-900">Meet Our Barbers</h2>
            <p className="mt-3 text-gray-600 max-w-2xl mx-auto">
              Our talented team of professionals is ready to give you the perfect look
            </p>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
            {topBarbers.map((barber) => (
              <div
                key={barber.id}
                className="bg-gradient-to-b from-gray-50 to-white rounded-2xl p-8 text-center border border-gray-100 hover:shadow-lg transition-all duration-300 group"
              >
                <div className="text-6xl mb-4">{barber.avatar}</div>
                <h3 className="text-xl font-bold text-gray-900">{barber.name}</h3>
                <p className="text-amber-600 text-sm font-medium mt-1">{barber.specialty}</p>
                <div className="flex items-center justify-center gap-1 mt-3">
                  <Star className="w-4 h-4 text-amber-400 fill-amber-400" />
                  <span className="text-sm font-semibold text-gray-900">{barber.rating}</span>
                  <span className="text-sm text-gray-400">({barber.reviews} reviews)</span>
                </div>
                <p className="text-sm text-gray-500 mt-2">{barber.experience} years experience</p>
                <Link
                  to="/booking"
                  className="mt-4 inline-block bg-gray-900 text-white px-6 py-2.5 rounded-xl text-sm font-semibold hover:bg-gray-800 transition-colors"
                >
                  Book with {barber.name.split(' ')[0]}
                </Link>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 bg-gradient-to-r from-amber-500 to-amber-600">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-3xl sm:text-4xl font-bold text-white">Ready for a Fresh Look?</h2>
          <p className="mt-4 text-amber-100 text-lg max-w-2xl mx-auto">
            Book your appointment online in just a few clicks. Choose your service, pick your barber,
            and select a time that works for you.
          </p>
          <Link
            to="/booking"
            className="mt-8 inline-flex items-center gap-2 bg-white text-amber-700 px-8 py-4 rounded-xl text-lg font-bold hover:bg-gray-50 transition-all shadow-lg"
          >
            <Calendar className="w-5 h-5" />
            Book Your Appointment
          </Link>
        </div>
      </section>
    </div>
  );
};

export default HomePage;
