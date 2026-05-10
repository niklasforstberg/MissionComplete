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

export interface TeamMemberDto {
  UserId: number;
  Email: string;
  JoinedAt: string;
}

export interface TeamCoachDto {
  UserId: number;
  Email: string;
  JoinedAt: string;
}

export interface TeamDto {
  Id: number;
  Name: string;
  Description?: string;
  CreatedAt: string;
  Coaches: TeamCoachDto[];
  Members: TeamMemberDto[];
}

export interface UserTeamDto {
  Id: number;
  Name: string;
  JoinedAt: string;
}

export interface CreateTeamDto {
  Name: string;
  Description?: string;
}

export const teamService = {
  async getMyTeams(): Promise<TeamListDto[]> {
    const response = await api.get<TeamListDto[]>('/api/teams/my');
    return response.data;
  },

  async getMyPlayerTeams(): Promise<UserTeamDto[]> {
    const response = await api.get<UserTeamDto[]>('/api/user/teams');
    return response.data;
  },

  async getAllTeams(): Promise<TeamListDto[]> {
    const response = await api.get<TeamListDto[]>('/api/teams');
    return response.data;
  },

  async getTeam(id: number): Promise<TeamDto> {
    const response = await api.get<TeamDto>(`/api/teams/${id}`);
    return response.data;
  },

  async createTeam(data: CreateTeamDto): Promise<TeamDto> {
    const response = await api.post<TeamDto>('/api/teams', data);
    return response.data;
  },

  async inviteMember(teamId: number, email: string): Promise<TeamMemberDto> {
    const response = await api.post<TeamMemberDto>(`/api/teams/${teamId}/members`, { Email: email });
    return response.data;
  },

  async removeMember(teamId: number, userId: number): Promise<void> {
    await api.delete(`/api/teams/${teamId}/members/${userId}`);
  },
};

