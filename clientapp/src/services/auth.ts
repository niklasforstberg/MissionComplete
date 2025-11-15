import axios from 'axios';

// In dev, Vite proxy handles /api. In production, use relative URLs
// If VITE_API_URL is not set, use empty string to rely on Vite proxy in dev or relative URLs in production
const API_BASE_URL = import.meta.env.VITE_API_URL || '';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', {
      url: error.config?.url,
      status: error.response?.status,
      message: error.message,
    });
    return Promise.reject(error);
  }
);

// Add token to requests if available
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface LoginRequest {
  Email: string;
  Password: string;
}

export interface LoginResponse {
  Token: string;
}

export interface ForgotPasswordRequest {
  Email: string;
}

export interface ForgotPasswordResponse {
  Message: string;
}

export interface ResetPasswordRequest {
  Token: string;
  Password: string;
}

export interface ResetPasswordResponse {
  Token: string;
}

export interface RegisterRequest {
  Email: string;
  Password: string;
  Role: 'Player' | 'Coach';
}

export interface RegisterResponse {
  Message: string;
}

export interface VerifyEmailRequest {
  Token: string;
}

export interface VerifyEmailResponse {
  Token: string;
  Message: string;
}

export interface CompleteRegistrationRequest {
  Token: string;
  FirstName: string;
  LastName: string;
}

export interface CompleteRegistrationResponse {
  Token: string;
}

export interface User {
  Id: number;
  Email: string;
  Role: string;
  Invited: boolean;
  InvitedBy?: {
    Id: number;
    Email: string;
    Role: string;
  } | null;
  Teams: Array<{
    Id: number;
    Name: string;
    JoinedAt: string;
  }>;
}

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await api.post<LoginResponse>('/api/auth/login', credentials);
    return response.data;
  },

  async getCurrentUser(): Promise<User> {
    const response = await api.get<User>('/api/auth/me');
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
    window.location.href = '/';
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  },

  getToken(): string | null {
    return localStorage.getItem('token');
  },

  setToken(token: string) {
    localStorage.setItem('token', token);
  },

  async forgotPassword(email: string): Promise<ForgotPasswordResponse> {
    const response = await api.post<ForgotPasswordResponse>('/api/auth/forgot-password', { Email: email });
    return response.data;
  },

  async resetPassword(token: string, password: string): Promise<ResetPasswordResponse> {
    const response = await api.post<ResetPasswordResponse>('/api/auth/set-password', { Token: token, Password: password });
    return response.data;
  },

  async register(credentials: RegisterRequest): Promise<RegisterResponse> {
    const response = await api.post<RegisterResponse>('/api/auth/register', credentials);
    return response.data;
  },

  async verifyEmail(token: string): Promise<VerifyEmailResponse> {
    const response = await api.post<VerifyEmailResponse>('/api/auth/verify-email', { Token: token });
    return response.data;
  },

  async completeRegistration(credentials: CompleteRegistrationRequest): Promise<CompleteRegistrationResponse> {
    const response = await api.post<CompleteRegistrationResponse>('/api/auth/complete-registration', credentials);
    return response.data;
  },
};

