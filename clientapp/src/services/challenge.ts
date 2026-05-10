import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || '';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export type ChallengeType = 'Cardio' | 'Strength' | 'SkillBased' | 'Other';
export type ChallengeFrequency = 'Daily' | 'Weekly' | 'Monthly' | 'OneTime' | 'Custom';

export interface ChallengeDto {
  Id: number;
  Name: string;
  Description?: string;
  Type: ChallengeType;
  Frequency: ChallengeFrequency;
  StartDate: string;
  EndDate: string;
  CompletionCount: number;
  CreatedById: number;
  TeamId: number;
}

export interface CompletedChallengeDto {
  Id: number;
  Name: string;
  Description?: string;
  Type: string;
  Frequency: string;
  StartDate: string;
  EndDate: string;
  TeamId: number;
  TeamName: string;
  CompletionId: number;
  CompletedAt: string;
  Notes?: string;
}

export interface LeaderboardEntry {
  UserId: number;
  Email: string;
  FirstName: string;
  LastName: string;
  CompletionCount: number;
}

export interface CreateChallengeDto {
  Name: string;
  Description?: string;
  Type: ChallengeType;
  Frequency: ChallengeFrequency;
  StartDate: string;
  EndDate: string;
  TeamId: number;
}

export const challengeService = {
  async getTeamChallenges(teamId: number): Promise<ChallengeDto[]> {
    const response = await api.get<ChallengeDto[]>(`/api/team/${teamId}/challenges`);
    return response.data;
  },

  async getMyCompletedChallenges(): Promise<CompletedChallengeDto[]> {
    const response = await api.get<CompletedChallengeDto[]>('/api/user/completed-challenges');
    return response.data;
  },

  async completeChallenge(id: number): Promise<void> {
    await api.post(`/api/challenge/${id}/complete`, { Notes: null });
  },

  async createChallenge(data: CreateChallengeDto): Promise<ChallengeDto> {
    const response = await api.post<ChallengeDto>('/api/challenge', data);
    return response.data;
  },

  async deleteChallenge(id: number): Promise<void> {
    await api.delete(`/api/challenge/${id}`);
  },

  async getTeamLeaderboard(teamId: number): Promise<LeaderboardEntry[]> {
    const response = await api.get<LeaderboardEntry[]>(`/api/team/${teamId}/leaderboard`);
    return response.data;
  },
};
