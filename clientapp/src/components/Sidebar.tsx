import { Link, useLocation } from 'react-router-dom';

interface SidebarProps {
  collapsed: boolean;
  onToggle: () => void;
}

export default function Sidebar({ collapsed, onToggle }: SidebarProps) {
  const location = useLocation();

  const isActive = (path: string) => {
    if (path === '/dashboard') {
      return location.pathname === '/dashboard';
    }
    return location.pathname.startsWith(path);
  };

  return (
    <aside className={`sidebar ${collapsed ? 'collapsed' : ''}`}>
      <button className="sidebar-toggle" onClick={onToggle} aria-label="Toggle sidebar">
        {collapsed ? '→' : '←'}
      </button>
      <nav className="sidebar-nav">
        <Link
          to="/dashboard"
          className={`sidebar-item ${isActive('/dashboard') ? 'active' : ''}`}
        >
          <span className="sidebar-icon">🏃</span>
          {!collapsed && <span className="sidebar-label">Teams</span>}
        </Link>
        <Link
          to="/exercises"
          className={`sidebar-item ${isActive('/exercises') ? 'active' : ''}`}
        >
          <span className="sidebar-icon">💪</span>
          {!collapsed && <span className="sidebar-label">Exercises</span>}
        </Link>
      </nav>
    </aside>
  );
}

