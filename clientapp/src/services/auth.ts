import axios from 'axios';

// In dev, Vite proxy handles /api. In production, use relative URLs
const API_BASE_URL = import.meta.env.VITE_API_URL || '';

const api = axios.create({
  baseURL: API_BASE_URL,
});

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

export interface User {
  Id: number;
  Email: string;
  Role: string;
  Invited: boolean;
  InvitedBy?: {
    Id: number;
    Email: string;
    Role: string;
  };
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
};

