import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { teamService, type TeamListDto } from '../services/team';

export default function CoachDashboard() {
  const { user, logout } = useAuth();
  const [teams, setTeams] = useState<TeamListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchTeams = async () => {
      try {
        setLoading(true);
        const data = await teamService.getMyTeams();
        setTeams(data);
      } catch (err) {
        console.error('Failed to fetch teams:', err);
        setError('Failed to load teams');
      } finally {
        setLoading(false);
      }
    };

    fetchTeams();
  }, []);

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Coach Dashboard</h1>
          <button onClick={logout} className="dashboard-logout-btn">
            Logout
          </button>
        </div>

        <div className="dashboard-card">
          <p className="dashboard-welcome">Welcome, {user?.Email}!</p>
        </div>

        {loading ? (
          <div className="dashboard-card">
            <p>Loading teams...</p>
          </div>
        ) : error ? (
          <div className="dashboard-card">
            <p className="dashboard-error">{error}</p>
          </div>
        ) : teams.length > 1 ? (
          <div className="dashboard-card">
            <h2 className="dashboard-section-title">Your Teams</h2>
            <div className="team-list">
              {teams.map((team) => (
                <Link
                  key={team.Id}
                  to={`/team/${team.Id}`}
                  className="team-link"
                >
                  <div className="team-link-content">
                    <h3 className="team-link-name">{team.Name}</h3>
                    {team.Description && (
                      <p className="team-link-description">{team.Description}</p>
                    )}
                  </div>
                  <span className="team-link-arrow">→</span>
                </Link>
              ))}
            </div>
          </div>
        ) : teams.length === 1 ? (
          <div className="dashboard-card">
            <p className="dashboard-info">You have one team.</p>
            <Link
              to={`/team/${teams[0].Id}`}
              className="dashboard-button"
            >
              Go to Team
            </Link>
          </div>
        ) : (
          <div className="dashboard-card">
            <p className="dashboard-info">You don't have any teams yet.</p>
          </div>
        )}
      </div>
    </div>
  );
}

