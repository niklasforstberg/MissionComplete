import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { teamService, type TeamDto } from '../services/team';

export default function TeamPage() {
  const { id } = useParams<{ id: string }>();
  const { logout } = useAuth();
  const [team, setTeam] = useState<TeamDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchTeam = async () => {
      if (!id) return;

      try {
        setLoading(true);
        const data = await teamService.getTeam(parseInt(id));
        setTeam(data);
      } catch (err) {
        console.error('Failed to fetch team:', err);
        setError('Failed to load team');
      } finally {
        setLoading(false);
      }
    };

    fetchTeam();
  }, [id]);

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <Link to="/dashboard" className="dashboard-back-link">
            ← Back
          </Link>
          <button onClick={logout} className="dashboard-logout-btn">
            Logout
          </button>
        </div>

        {loading ? (
          <div className="dashboard-card">
            <p>Loading team...</p>
          </div>
        ) : error ? (
          <div className="dashboard-card">
            <p className="dashboard-error">{error}</p>
          </div>
        ) : team ? (
          <>
            <div className="dashboard-card">
              <h1 className="dashboard-title">{team.Name}</h1>
              {team.Description && (
                <p className="team-description">{team.Description}</p>
              )}
            </div>

            <div className="dashboard-card">
              <h2 className="dashboard-section-title">Add Players</h2>
              <p className="dashboard-placeholder">Player management coming soon...</p>
            </div>

            <div className="dashboard-card">
              <h2 className="dashboard-section-title">Exercises</h2>
              <p className="dashboard-placeholder">Exercise management coming soon...</p>
            </div>
          </>
        ) : null}
      </div>
    </div>
  );
}

