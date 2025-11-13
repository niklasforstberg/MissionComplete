import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Landing from './pages/Landing';
import Login from './pages/Login';
import './App.css';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return (
      <div style={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'center', 
        minHeight: '100vh',
        fontFamily: 'JetBrains Mono, monospace',
        color: '#a3b18a'
      }}>
        Loading...
      </div>
    );
  }

  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

function Dashboard() {
  const { user, logout } = useAuth();

  return (
    <div style={{ 
      minHeight: '100vh', 
      padding: '2rem',
      background: '#0a0e1a',
      color: '#eae0d5',
      fontFamily: 'JetBrains Mono, monospace'
    }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
          <h1 style={{ 
            fontFamily: 'Crimson Text, serif',
            fontSize: '2rem',
            background: 'linear-gradient(135deg, #a3b18a 0%, #2c5f2d 100%)',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent',
            margin: 0
          }}>
            Dashboard
          </h1>
          <button
            onClick={logout}
            style={{
              padding: '0.5rem 1.5rem',
              background: 'transparent',
              border: '1px solid rgba(163, 177, 138, 0.3)',
              borderRadius: '4px',
              color: '#a3b18a',
              cursor: 'pointer',
              fontFamily: 'inherit'
            }}
          >
            Logout
          </button>
        </div>
        <div style={{
          background: 'rgba(163, 177, 138, 0.05)',
          border: '1px solid rgba(163, 177, 138, 0.2)',
          borderRadius: '8px',
          padding: '2rem'
        }}>
          <p>Welcome, {user?.Email}!</p>
          <p style={{ color: 'rgba(234, 224, 213, 0.6)', fontSize: '0.9rem' }}>
            Role: {user?.Role}
          </p>
        </div>
      </div>
    </div>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Landing />} />
          <Route path="/login" element={<Login />} />
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
