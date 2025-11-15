import { useState, useEffect, type FormEvent } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { AxiosError } from 'axios';
import { authService } from '../services/auth';

export default function VerifyEmail() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [token, setToken] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);
  const [nameCollectionToken, setNameCollectionToken] = useState('');

  useEffect(() => {
    const tokenFromUrl = searchParams.get('token');
    if (!tokenFromUrl) {
      setError('Invalid verification link. Please check your email.');
    } else {
      setToken(tokenFromUrl);
      handleVerify(tokenFromUrl);
    }
  }, [searchParams]);

  const handleVerify = async (verificationToken: string) => {
    setLoading(true);
    setError('');

    try {
      const response = await authService.verifyEmail(verificationToken);
      setNameCollectionToken(response.Token);
      setSuccess(true);
    } catch (err) {
      const error = err as AxiosError<{ message?: string }>;
      setError(error.response?.data?.message || 'Failed to verify email. The link may have expired.');
    } finally {
      setLoading(false);
    }
  };

  const handleContinue = () => {
    if (nameCollectionToken) {
      navigate(`/complete-registration?token=${nameCollectionToken}`);
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
          {loading ? (
            <div style={{ textAlign: 'center', padding: '2rem 0' }}>
              <div style={{ 
                color: '#a3b18a', 
                marginBottom: '1rem',
                fontSize: '1.1rem'
              }}>
                Verifying your email...
              </div>
            </div>
          ) : success ? (
            <div style={{ textAlign: 'center', padding: '2rem 0' }}>
              <h1 className="login-title">Email Verified!</h1>
              <p className="login-subtitle" style={{ marginBottom: '2rem' }}>
                Your email has been successfully verified. Please complete your registration by providing your name.
              </p>
              <button
                onClick={handleContinue}
                className="login-button"
                style={{ width: '100%' }}
              >
                Continue
              </button>
            </div>
          ) : (
            <div>
              <h1 className="login-title">Email Verification</h1>
              <p className="login-subtitle">Verifying your email address</p>
              {error && <div className="login-error">{error}</div>}
              {!error && !token && (
                <div style={{ textAlign: 'center', padding: '2rem 0' }}>
                  <p style={{ color: 'rgba(234, 224, 213, 0.7)' }}>
                    No verification token found. Please check your email for the verification link.
                  </p>
                </div>
              )}
            </div>
          )}

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

