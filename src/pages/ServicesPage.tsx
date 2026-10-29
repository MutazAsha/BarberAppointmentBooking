import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Clock, DollarSign, ChevronRight, Filter } from 'lucide-react';
import { services, categories } from '../data/mockData';
import { useDispatch } from 'react-redux';
import { setSelectedService } from '../store/bookingSlice';

const ServicesPage: React.FC = () => {
  const [activeCategory, setActiveCategory] = useState('All');
  const dispatch = useDispatch();

  const filteredServices =
    activeCategory === 'All'
      ? services
      : services.filter((s) => s.category === activeCategory);

  const handleBookService = (service: typeof services[0]) => {
    dispatch(setSelectedService(service));
  };

  return (
    <div className="min-h-screen bg-gray-50 pt-20">
      {/* Header */}
      <div className="bg-white border-b border-gray-100">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
          <h1 className="text-3xl sm:text-4xl font-bold text-gray-900">Our Services</h1>
          <p className="mt-3 text-gray-600 max-w-2xl">
            Browse our complete menu of premium grooming services. Each service is performed by our
            expert barbers using top-quality products.
          </p>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Category Filter */}
        <div className="flex items-center gap-3 mb-8 overflow-x-auto pb-2">
          <Filter className="w-5 h-5 text-gray-400 flex-shrink-0" />
          {categories.map((cat) => (
            <button
              key={cat}
              onClick={() => setActiveCategory(cat)}
              className={`px-4 py-2 rounded-full text-sm font-medium whitespace-nowrap transition-all ${
                activeCategory === cat
                  ? 'bg-amber-500 text-white shadow-md'
                  : 'bg-white text-gray-600 border border-gray-200 hover:border-amber-300 hover:text-amber-600'
              }`}
            >
              {cat}
            </button>
          ))}
        </div>

        {/* Services Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredServices.map((service) => (
            <div
              key={service.id}
              className="bg-white rounded-2xl p-6 shadow-sm hover:shadow-md transition-all duration-300 border border-gray-100 group"
            >
              <div className="flex items-start justify-between mb-4">
                <div className="w-14 h-14 bg-amber-100 rounded-xl flex items-center justify-center group-hover:bg-amber-500 transition-colors">
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
                <span className="px-3 py-1 bg-gray-100 text-gray-600 text-xs font-medium rounded-full">
                  {service.category}
                </span>
              </div>

              <h3 className="text-xl font-bold text-gray-900">{service.name}</h3>
              <p className="text-sm text-gray-500 mt-2 leading-relaxed">{service.description}</p>

              <div className="flex items-center gap-4 mt-4 pt-4 border-t border-gray-100">
                <div className="flex items-center gap-1.5 text-gray-600">
                  <Clock className="w-4 h-4 text-amber-500" />
                  <span className="text-sm font-medium">{service.duration} min</span>
                </div>
                <div className="flex items-center gap-1.5 text-gray-600">
                  <DollarSign className="w-4 h-4 text-amber-500" />
                  <span className="text-sm font-medium">${service.price}</span>
                </div>
              </div>

              <Link
                to="/booking"
                onClick={() => handleBookService(service)}
                className="mt-4 w-full flex items-center justify-center gap-2 bg-gray-900 text-white py-3 rounded-xl text-sm font-semibold hover:bg-gray-800 transition-colors"
              >
                Book This Service
                <ChevronRight className="w-4 h-4" />
              </Link>
            </div>
          ))}
        </div>

        {filteredServices.length === 0 && (
          <div className="text-center py-16">
            <p className="text-gray-500 text-lg">No services found in this category.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default ServicesPage;
