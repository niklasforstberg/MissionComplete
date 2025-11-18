import { useAuth } from '../contexts/AuthContext';

export default function PlayerDashboard() {
  const { user } = useAuth();

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Player Dashboard</h1>
        </div>

        <div className="dashboard-card">
          <p className="dashboard-welcome">Welcome, {user?.Email}!</p>
          <p className="dashboard-placeholder">Player dashboard coming soon...</p>
        </div>
      </div>
    </div>
  );
}

