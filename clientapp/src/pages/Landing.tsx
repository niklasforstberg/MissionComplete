import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Landing.css';

export default function Landing() {
  const { isAuthenticated } = useAuth();

  return (
    <div className="landing">
      <div className="landing-background">
        <div className="grid-pattern"></div>
        <div className="gradient-orb orb-1"></div>
        <div className="gradient-orb orb-2"></div>
      </div>
      
      <nav className="landing-nav">
        <div className="nav-brand">MissionComplete</div>
        {isAuthenticated ? (
          <Link to="/dashboard" className="nav-link">Dashboard</Link>
        ) : (
          <Link to="/login" className="nav-link">Sign In</Link>
        )}
      </nav>

      <main className="landing-content">
        <div className="landing-hero">
          <h1 className="landing-title">
            <span className="title-line">Track Your</span>
            <span className="title-line title-accent">Off-Season</span>
            <span className="title-line">Progress</span>
          </h1>
          <p className="landing-subtitle">
            Transform your training into achievements. Set goals, complete challenges, 
            and build the discipline that wins championships.
          </p>
          <div className="landing-cta">
            {isAuthenticated ? (
              <Link to="/dashboard" className="cta-button primary">
                Go to Dashboard
              </Link>
            ) : (
              <>
                <Link to="/login" className="cta-button primary">
                  Get Started
                </Link>
                <Link to="/login" className="cta-button secondary">
                  Sign In
                </Link>
              </>
            )}
          </div>
        </div>

        <div className="landing-features">
          <div className="feature-card">
            <div className="feature-icon">🎯</div>
            <h3>Set Goals</h3>
            <p>Define clear objectives and track your progress toward them</p>
          </div>
          <div className="feature-card">
            <div className="feature-icon">🏆</div>
            <h3>Complete Challenges</h3>
            <p>Take on structured challenges that push your limits</p>
          </div>
          <div className="feature-card">
            <div className="feature-icon">👥</div>
            <h3>Team Up</h3>
            <p>Join teams, compete together, and achieve more</p>
          </div>
        </div>
      </main>
    </div>
  );
}

