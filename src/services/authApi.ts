// ============================================
// FRONTEND - Authentication API Service
// ============================================
// Handles login, register, and token management.
// Tokens are stored in localStorage and sent with every request.

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber?: string;
}

export interface AuthResponse {
  token: string;
  fullName: string;
  email: string;
  role: string;
  expiresAt: string;
}

// Storage keys
const TOKEN_KEY = 'sharpcuts_token';
const USER_KEY = 'sharpcuts_user';

/**
 * Authentication service.
 * Manages JWT tokens and user session.
 */
export const authService = {
  /**
   * Login with email and password.
   * Stores JWT token on success.
   */
  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Login failed');
    }

    // Store token and user info
    localStorage.setItem(TOKEN_KEY, data.data.token);
    localStorage.setItem(USER_KEY, JSON.stringify({
      fullName: data.data.fullName,
      email: data.data.email,
      role: data.data.role,
    }));

    return data.data;
  },

  /**
   * Register a new account.
   * Automatically logs in after registration.
   */
  async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await fetch(`${API_URL}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.error || 'Registration failed');
    }

    // Store token and user info
    localStorage.setItem(TOKEN_KEY, data.data.token);
    localStorage.setItem(USER_KEY, JSON.stringify({
      fullName: data.data.fullName,
      email: data.data.email,
      role: data.data.role,
    }));

    return data.data;
  },

  /**
   * Logout - clear token and user data.
   */
  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  },

  /**
   * Get the stored JWT token.
   */
  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  },

  /**
   * Get the current user info.
   */
  getCurrentUser(): { fullName: string; email: string; role: string } | null {
    const user = localStorage.getItem(USER_KEY);
    return user ? JSON.parse(user) : null;
  },

  /**
   * Check if user is authenticated.
   */
  isAuthenticated(): boolean {
    return !!this.getToken();
  },

  /**
   * Check if user is admin.
   */
  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user?.role === 'Admin';
  },
};

/**
 * Fetch wrapper that automatically adds JWT token to requests.
 * Use this for all authenticated API calls.
 */
export async function authFetch<T>(
  url: string,
  options: RequestInit = {}
): Promise<T> {
  const token = authService.getToken();

  const headers = {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` }),
    ...options.headers,
  };

  const response = await fetch(`${API_URL}${url}`, {
    ...options,
    headers,
  });

  // Handle 401 Unauthorized (token expired or invalid)
  if (response.status === 401) {
    authService.logout();
    window.location.href = '/login';
    throw new Error('Session expired. Please login again.');
  }

  const data = await response.json();

  if (!response.ok) {
    throw new Error(data.error || 'Request failed');
  }

  return data;
}
