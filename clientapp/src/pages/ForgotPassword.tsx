import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { AxiosError } from 'axios';
import { authService } from '../services/auth';

export default function ForgotPassword() {
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess(false);
    setLoading(true);

    try {
      await authService.forgotPassword(email);
      setSuccess(true);
    } catch (err) {
      const error = err as AxiosError<{ message?: string }>;
      setError(error.response?.data?.message || 'Failed to send reset email. Please try again.');
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
          <h1 className="login-title">Reset Password</h1>
          <p className="login-subtitle">Enter your email to receive a password reset link</p>

          {success ? (
            <div style={{ textAlign: 'center', padding: '2rem 0' }}>
              <div style={{ 
                color: '#a3b18a', 
                marginBottom: '1rem',
                fontSize: '1.1rem'
              }}>
                Check your email!
              </div>
              <p style={{ color: 'rgba(234, 224, 213, 0.7)', marginBottom: '1.5rem' }}>
                If an account with that email exists, a password reset link has been sent.
              </p>
              <Link to="/login" className="login-link">
                Back to Sign In
              </Link>
            </div>
          ) : (
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

              <button
                type="submit"
                disabled={loading}
                className="login-button"
              >
                {loading ? 'Sending...' : 'Send Reset Link'}
              </button>
            </form>
          )}

          <div className="login-footer">
            <p>
              Remember your password?{' '}
              <Link to="/login" className="login-link">
                Sign In
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

