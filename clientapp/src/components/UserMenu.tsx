import { useState, useRef, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

export default function UserMenu() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isOpen]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleSettings = () => {
    setIsOpen(false);
    // TODO: Navigate to settings page when created
    console.log('Settings clicked');
  };

  return (
    <div className="user-menu" ref={menuRef}>
      <button
        className="user-menu-button"
        onClick={() => setIsOpen(!isOpen)}
        aria-label="User menu"
      >
        <span className="user-menu-avatar">
          {user?.Email?.charAt(0).toUpperCase() || 'U'}
        </span>
        <span className="user-menu-arrow">{isOpen ? '▲' : '▼'}</span>
      </button>
      {isOpen && (
        <div className="user-menu-dropdown">
          <button className="user-menu-item" onClick={handleSettings}>
            Settings
          </button>
          <button className="user-menu-item" onClick={handleLogout}>
            Logout
          </button>
        </div>
      )}
    </div>
  );
}

