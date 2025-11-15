import { useState, useEffect, type FormEvent } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { AxiosError } from 'axios';
import { authService } from '../services/auth';
import { useAuth } from '../contexts/AuthContext';

export default function CompleteRegistration() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { login } = useAuth();
  const [token, setToken] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const tokenFromUrl = searchParams.get('token');
    if (!tokenFromUrl) {
      setError('Invalid registration link. Please verify your email again.');
    } else {
      setToken(tokenFromUrl);
    }
  }, [searchParams]);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');

    if (!firstName.trim() || !lastName.trim()) {
      setError('First name and last name are required');
      return;
    }

    if (!token) {
      setError('Invalid registration link. Please verify your email again.');
      return;
    }

    setLoading(true);

    try {
      const response = await authService.completeRegistration({
        Token: token,
        FirstName: firstName.trim(),
        LastName: lastName.trim(),
      });
      
      // Set token and load user via auth context
      authService.setToken(response.Token);
      const currentUser = await authService.getCurrentUser();
      
      // Update auth context
      // We'll need to manually update the user state or reload
      window.location.href = '/dashboard';
    } catch (err) {
      const error = err as AxiosError<{ message?: string }>;
      setError(error.response?.data?.message || 'Failed to complete registration. Please try again.');
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
          <h1 className="login-title">Complete Registration</h1>
          <p className="login-subtitle">Please provide your name to finish setting up your account</p>

          <form onSubmit={handleSubmit} className="login-form">
            {error && <div className="login-error">{error}</div>}

            <div className="form-group">
              <label htmlFor="firstName">First Name</label>
              <input
                id="firstName"
                type="text"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                required
                autoComplete="given-name"
                placeholder="John"
              />
            </div>

            <div className="form-group">
              <label htmlFor="lastName">Last Name</label>
              <input
                id="lastName"
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                required
                autoComplete="family-name"
                placeholder="Doe"
              />
            </div>

            <button
              type="submit"
              disabled={loading || !token}
              className="login-button"
            >
              {loading ? 'Completing...' : 'Complete Registration'}
            </button>
          </form>

          <div className="login-footer">
            <p>
              <Link to="/login" className="login-link">
                Back to Sign In
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

