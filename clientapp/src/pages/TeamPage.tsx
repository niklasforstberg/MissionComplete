import { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { teamService, type TeamDto, type TeamListDto } from '../services/team';
import CreateTeamForm from '../components/CreateTeamForm';

export default function TeamPage() {
  const { id } = useParams<{ id: string }>();
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [team, setTeam] = useState<TeamDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showCreateForm, setShowCreateForm] = useState(false);

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

  const handleTeamCreated = (newTeam: TeamListDto) => {
    navigate(`/team/${newTeam.Id}`);
  };

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

        {showCreateForm && (
          <CreateTeamForm
            onCancel={() => setShowCreateForm(false)}
            onSuccess={handleTeamCreated}
          />
        )}

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
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                  <h1 className="dashboard-title" style={{ margin: 0 }}>{team.Name}</h1>
                  {team.Description && (
                    <p className="team-description">{team.Description}</p>
                  )}
                </div>
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

