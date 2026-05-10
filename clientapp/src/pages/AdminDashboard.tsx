import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { teamService, type TeamListDto } from '../services/team';
import axios from 'axios';

interface AdminUser {
  Id: number;
  Email: string;
  FirstName: string;
  LastName: string;
  Role: string;
  EmailVerified: boolean;
  CreatedAt: string;
}

const api = axios.create({ baseURL: import.meta.env.VITE_API_URL || '' });
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default function AdminDashboard() {
  const [teams, setTeams] = useState<TeamListDto[]>([]);
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'users' | 'teams'>('users');

  useEffect(() => {
    Promise.all([loadTeams(), loadUsers()]).finally(() => setLoading(false));
  }, []);

  async function loadTeams() {
    try {
      const data = await teamService.getAllTeams();
      setTeams(data);
    } catch (err) {
      console.error('Failed to load teams:', err);
    }
  }

  async function loadUsers() {
    try {
      const response = await api.get<AdminUser[]>('/api/users');
      setUsers(response.data);
    } catch (err) {
      console.error('Failed to load users:', err);
    }
  }

  if (loading) return <div className="loading-screen">Loading...</div>;

  const coaches = users.filter(u => u.Role === 'Coach');
  const players = users.filter(u => u.Role === 'Player');

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Admin</h1>
        </div>

        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-value">{users.length}</div>
            <div className="stat-label">Total Users</div>
          </div>
          <div className="stat-card">
            <div className="stat-value">{coaches.length}</div>
            <div className="stat-label">Coaches</div>
          </div>
          <div className="stat-card">
            <div className="stat-value">{players.length}</div>
            <div className="stat-label">Players</div>
          </div>
          <div className="stat-card">
            <div className="stat-value">{teams.length}</div>
            <div className="stat-label">Teams</div>
          </div>
        </div>

        <div className="team-tabs">
          <button className={`team-tab ${activeTab === 'users' ? 'active' : ''}`} onClick={() => setActiveTab('users')}>
            Users ({users.length})
          </button>
          <button className={`team-tab ${activeTab === 'teams' ? 'active' : ''}`} onClick={() => setActiveTab('teams')}>
            Teams ({teams.length})
          </button>
        </div>

        {activeTab === 'users' && (
          <div className="dashboard-card">
            <h2 className="dashboard-section-title">All Users</h2>
            {users.length === 0 ? (
              <p className="dashboard-placeholder">No users found.</p>
            ) : (
              <div className="admin-table-wrapper">
                <table className="admin-table">
                  <thead>
                    <tr>
                      <th>Email</th>
                      <th>Name</th>
                      <th>Role</th>
                      <th>Verified</th>
                      <th>Joined</th>
                    </tr>
                  </thead>
                  <tbody>
                    {users.map(u => (
                      <tr key={u.Id}>
                        <td>{u.Email}</td>
                        <td>{[u.FirstName, u.LastName].filter(Boolean).join(' ') || '—'}</td>
                        <td>
                          <span className={`badge ${u.Role === 'Coach' ? 'badge-type' : u.Role === 'Admin' ? 'badge-admin' : 'badge-freq'}`}>
                            {u.Role}
                          </span>
                        </td>
                        <td>
                          <span style={{ color: u.EmailVerified ? '#4ade80' : 'var(--color-text-tertiary)' }}>
                            {u.EmailVerified ? '✓' : '—'}
                          </span>
                        </td>
                        <td style={{ color: 'var(--color-text-tertiary)', fontSize: '0.85rem' }}>
                          {new Date(u.CreatedAt).toLocaleDateString('sv-SE')}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}

        {activeTab === 'teams' && (
          <div className="dashboard-card">
            <h2 className="dashboard-section-title">All Teams</h2>
            {teams.length === 0 ? (
              <p className="dashboard-placeholder">No teams found.</p>
            ) : (
              <div className="challenge-grid">
                {teams.map(team => (
                  <Link key={team.Id} to={`/team/${team.Id}`} style={{ textDecoration: 'none' }}>
                    <div className="challenge-card" style={{ cursor: 'pointer' }}>
                      <div className="challenge-card-header">
                        <div>
                          <h3 className="challenge-name">{team.Name}</h3>
                          {team.Description && <p className="challenge-desc">{team.Description}</p>}
                        </div>
                        <span className="badge badge-freq">View →</span>
                      </div>
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
