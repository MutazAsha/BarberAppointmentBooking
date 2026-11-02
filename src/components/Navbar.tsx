// ============================================
// FRONTEND - Navigation Bar
// ============================================
// Shows different links based on authentication state.

import React, { useState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../store';
import { toggleSidebar } from '../store/uiSlice';
import { Menu, X, Scissors, Calendar, Home, User, Phone, BarChart3, LogOut, LogIn } from 'lucide-react';
import { authService } from '../services/authApi';

const Navbar: React.FC = () => {
  const { sidebarOpen } = useSelector((state: RootState) => state.ui);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const [isAuthenticated, setIsAuthenticated] = useState(authService.isAuthenticated());
  const [currentUser, setCurrentUser] = useState(authService.getCurrentUser());

  // Update auth state when it changes
  useEffect(() => {
    setIsAuthenticated(authService.isAuthenticated());
    setCurrentUser(authService.getCurrentUser());
  }, [location]);

  const isActive = (path: string) => location.pathname === path;

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setCurrentUser(null);
    navigate('/');
  };

  // Public navigation links
  const publicLinks = [
    { path: '/', label: 'Home', icon: Home },
    { path: '/services', label: 'Services', icon: Scissors },
    { path: '/contact', label: 'Contact', icon: Phone },
  ];

  // Authenticated user links
  const userLinks = [
    { path: '/booking', label: 'Book Now', icon: Calendar },
    { path: '/appointments', label: 'My Appointments', icon: User },
  ];

  // Admin links
  const adminLinks = currentUser?.role === 'Admin'
    ? [{ path: '/dashboard', label: 'Dashboard', icon: BarChart3 }]
    : [];

  const navLinks = [...publicLinks, ...userLinks, ...adminLinks].filter(
    link => !isAuthenticated || !['/booking', '/appointments'].includes(link.path) || isAuthenticated
  );

  return (
    <>
      <nav className="fixed top-0 left-0 right-0 z-50 bg-white/95 backdrop-blur-md shadow-sm border-b border-gray-100">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            {/* Logo */}
            <Link to="/" className="flex items-center gap-2">
              <div className="w-10 h-10 bg-gradient-to-br from-amber-500 to-amber-700 rounded-xl flex items-center justify-center shadow-lg">
                <Scissors className="w-5 h-5 text-white" />
              </div>
              <div className="hidden sm:block">
                <h1 className="text-xl font-bold text-gray-900">SharpCuts</h1>
                <p className="text-[10px] text-amber-600 font-medium -mt-1">PREMIUM BARBER SHOP</p>
              </div>
            </Link>

            {/* Desktop Navigation */}
            <div className="hidden md:flex items-center gap-1">
              {publicLinks.map((link) => {
                const Icon = link.icon;
                return (
                  <Link
                    key={link.path}
                    to={link.path}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-all ${
                      isActive(link.path)
                        ? 'bg-amber-50 text-amber-700'
                        : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                    }`}
                  >
                    <Icon className="w-4 h-4" />
                    {link.label}
                  </Link>
                );
              })}

              {isAuthenticated && userLinks.map((link) => {
                const Icon = link.icon;
                return (
                  <Link
                    key={link.path}
                    to={link.path}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-all ${
                      isActive(link.path)
                        ? 'bg-amber-50 text-amber-700'
                        : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                    }`}
                  >
                    <Icon className="w-4 h-4" />
                    {link.label}
                  </Link>
                );
              })}

              {adminLinks.map((link) => {
                const Icon = link.icon;
                return (
                  <Link
                    key={link.path}
                    to={link.path}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-all ${
                      isActive(link.path)
                        ? 'bg-amber-50 text-amber-700'
                        : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                    }`}
                  >
                    <Icon className="w-4 h-4" />
                    {link.label}
                  </Link>
                );
              })}
            </div>

            {/* Auth Buttons */}
            <div className="hidden md:flex items-center gap-3">
              {isAuthenticated ? (
                <>
                  <div className="text-right">
                    <p className="text-sm font-medium text-gray-900">{currentUser?.fullName}</p>
                    <p className="text-xs text-amber-600">{currentUser?.role}</p>
                  </div>
                  <button
                    onClick={handleLogout}
                    className="flex items-center gap-2 px-4 py-2 border border-gray-200 rounded-lg text-sm font-medium text-gray-600 hover:bg-gray-50 transition-colors"
                  >
                    <LogOut className="w-4 h-4" />
                    Logout
                  </button>
                </>
              ) : (
                <Link
                  to="/login"
                  className="flex items-center gap-2 bg-gradient-to-r from-amber-500 to-amber-600 text-white px-5 py-2.5 rounded-lg text-sm font-semibold hover:from-amber-600 hover:to-amber-700 transition-all shadow-md"
                >
                  <LogIn className="w-4 h-4" />
                  Sign In
                </Link>
              )}
            </div>

            {/* Mobile menu button */}
            <button
              onClick={() => dispatch(toggleSidebar())}
              className="md:hidden p-2 rounded-lg text-gray-600 hover:bg-gray-100"
            >
              {sidebarOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
            </button>
          </div>
        </div>
      </nav>

      {/* Mobile Sidebar */}
      <div className={`fixed inset-0 z-40 md:hidden ${sidebarOpen ? '' : 'pointer-events-none'}`}>
        <div className="fixed inset-0 bg-black/50" onClick={() => dispatch(toggleSidebar())} />
        <div className={`fixed top-0 right-0 h-full w-72 bg-white shadow-2xl transform transition-transform ${sidebarOpen ? 'translate-x-0' : 'translate-x-full'}`}>
          <div className="p-6 pt-20">
            <div className="space-y-2">
              {publicLinks.map((link) => {
                const Icon = link.icon;
                return (
                  <Link
                    key={link.path}
                    to={link.path}
                    onClick={() => dispatch(toggleSidebar())}
                    className={`flex items-center gap-3 px-4 py-3 rounded-xl text-base font-medium ${
                      isActive(link.path) ? 'bg-amber-50 text-amber-700' : 'text-gray-700'
                    }`}
                  >
                    <Icon className="w-5 h-5" />
                    {link.label}
                  </Link>
                );
              })}

              {isAuthenticated && (
                <>
                  <div className="border-t border-gray-100 my-4" />
                  {userLinks.map((link) => {
                    const Icon = link.icon;
                    return (
                      <Link
                        key={link.path}
                        to={link.path}
                        onClick={() => dispatch(toggleSidebar())}
                        className={`flex items-center gap-3 px-4 py-3 rounded-xl text-base font-medium ${
                          isActive(link.path) ? 'bg-amber-50 text-amber-700' : 'text-gray-700'
                        }`}
                      >
                        <Icon className="w-5 h-5" />
                        {link.label}
                      </Link>
                    );
                  })}

                  {adminLinks.map((link) => {
                    const Icon = link.icon;
                    return (
                      <Link
                        key={link.path}
                        to={link.path}
                        onClick={() => dispatch(toggleSidebar())}
                        className="flex items-center gap-3 px-4 py-3 rounded-xl text-base font-medium text-gray-700"
                      >
                        <Icon className="w-5 h-5" />
                        {link.label}
                      </Link>
                    );
                  })}
                </>
              )}
            </div>

            <div className="mt-6 pt-6 border-t border-gray-100">
              {isAuthenticated ? (
                <button
                  onClick={() => {
                    handleLogout();
                    dispatch(toggleSidebar());
                  }}
                  className="w-full flex items-center justify-center gap-2 px-4 py-3 border border-gray-200 rounded-xl text-gray-700 font-medium"
                >
                  <LogOut className="w-4 h-4" />
                  Logout
                </button>
              ) : (
                <Link
                  to="/login"
                  onClick={() => dispatch(toggleSidebar())}
                  className="block w-full text-center bg-gradient-to-r from-amber-500 to-amber-600 text-white px-5 py-3 rounded-xl font-semibold"
                >
                  Sign In
                </Link>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default Navbar;
