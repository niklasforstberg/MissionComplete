import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { teamService, type TeamListDto } from '../services/team';
import CreateTeamForm from '../components/CreateTeamForm';

export default function CoachDashboard() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [teams, setTeams] = useState<TeamListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);

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

  useEffect(() => {
    if (!loading && !error && teams.length === 1 && !showCreateForm) {
      navigate(`/team/${teams[0].Id}`, { replace: true });
    }
  }, [loading, error, teams, showCreateForm, navigate]);

  const handleTeamCreated = (newTeam: TeamListDto) => {
    setTeams([...teams, newTeam]);
    setShowCreateForm(false);
  };

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Coach Dashboard</h1>
          <button onClick={logout} className="dashboard-logout-btn">
            Logout
          </button>
        </div>

        {showCreateForm && (
          <CreateTeamForm
            onCancel={() => setShowCreateForm(false)}
            onSuccess={handleTeamCreated}
          />
        )}

        {loading ? (
          <div className="dashboard-card">
            <p>Loading teams...</p>
          </div>
        ) : error && !showCreateForm ? (
          <div className="dashboard-card">
            <p className="dashboard-error">{error}</p>
          </div>
        ) : teams.length === 0 ? (
          <div className="dashboard-card">
            <p className="dashboard-info">
              You have no team yet.{' '}
              <button
                onClick={() => setShowCreateForm(true)}
                style={{
                  background: 'none',
                  border: 'none',
                  color: 'var(--color-accent-primary)',
                  textDecoration: 'underline',
                  cursor: 'pointer',
                  fontFamily: 'inherit',
                  fontSize: 'inherit',
                  padding: 0,
                }}
              >
                Create one!
              </button>
            </p>
          </div>
        ) : teams.length > 1 ? (
          <div className="dashboard-card">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
              <h2 className="dashboard-section-title" style={{ margin: 0 }}>Your Teams</h2>
              <button
                onClick={() => setShowCreateForm(true)}
                className="add-team-button"
                title="Add Team"
              >
                +
              </button>
            </div>
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
        ) : null}
      </div>
    </div>
  );
}

