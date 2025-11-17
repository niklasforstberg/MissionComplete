import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || '';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export interface TeamListDto {
  Id: number;
  Name: string;
  Description?: string;
}

export interface TeamDto {
  Id: number;
  Name: string;
  Description?: string;
  CreatedAt: string;
  Members: Array<{
    UserId: number;
    Email: string;
    JoinedAt: string;
  }>;
}

export const teamService = {
  async getMyTeams(): Promise<TeamListDto[]> {
    const response = await api.get<TeamListDto[]>('/api/teams/my');
    return response.data;
  },

  async getTeam(id: number): Promise<TeamDto> {
    const response = await api.get<TeamDto>(`/api/teams/${id}`);
    return response.data;
  },
};

