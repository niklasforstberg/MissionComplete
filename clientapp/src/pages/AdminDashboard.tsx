import { useAuth } from '../contexts/AuthContext';

export default function AdminDashboard() {
  const { user, logout } = useAuth();

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Admin Dashboard</h1>
          <button onClick={logout} className="dashboard-logout-btn">
            Logout
          </button>
        </div>

        <div className="dashboard-card">
          <p className="dashboard-welcome">Welcome, {user?.Email}!</p>
          <p className="dashboard-placeholder">Admin dashboard coming soon...</p>
        </div>
      </div>
    </div>
  );
}

