import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { AxiosError } from 'axios';
import { useAuth } from '../contexts/AuthContext';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login, isAuthenticated, loading: authLoading } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!authLoading && isAuthenticated) {
      navigate('/dashboard', { replace: true });
    }
  }, [isAuthenticated, authLoading, navigate]);

  if (authLoading) {
    return (
      <div className="login-page">
        <div style={{ 
          fontFamily: 'JetBrains Mono, monospace',
          color: '#a3b18a',
          textAlign: 'center'
        }}>
          Loading...
        </div>
      </div>
    );
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login(email, password);
      console.log('Login successful');
      navigate('/dashboard');
    } catch (err) {
      const error = err as AxiosError<{ message?: string }>;
      setError(error.response?.data?.message || 'Invalid email or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-background">
        <div className="login-grid"></div>
        <div className="login-orb"></div>
      </div>

      <div className="login-container">
        <Link to="/" className="login-brand">
          MissionComplete
        </Link>

        <div className="login-card">
          <h1 className="login-title">Welcome Back</h1>
          <p className="login-subtitle">Sign in to continue your journey</p>

          <form onSubmit={handleSubmit} className="login-form">
            {error && <div className="login-error">{error}</div>}

            <div className="form-group">
              <label htmlFor="email">Email</label>
              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                autoComplete="email"
                placeholder="you@example.com"
              />
            </div>

            <div className="form-group">
              <label htmlFor="password">Password</label>
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                autoComplete="current-password"
                placeholder="••••••••"
              />
            </div>

            <div style={{ textAlign: 'right', marginBottom: '1rem' }}>
              <Link to="/forgot-password" className="login-link" style={{ fontSize: '0.9rem' }}>
                Forgot Password?
              </Link>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="login-button"
            >
              {loading ? 'Signing in...' : 'Sign In'}
            </button>
          </form>

          <div className="login-footer">
            <p>
              Don't have an account?{' '}
              <Link to="/register" className="login-link">
                Contact your coach
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

