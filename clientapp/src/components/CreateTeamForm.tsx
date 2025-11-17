import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { teamService, type CreateTeamDto, type TeamListDto } from '../services/team';

interface CreateTeamFormProps {
  onCancel: () => void;
  onSuccess: (team: TeamListDto) => void;
}

export default function CreateTeamForm({ onCancel, onSuccess }: CreateTeamFormProps) {
  const navigate = useNavigate();
  const [creating, setCreating] = useState(false);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState<CreateTeamDto>({ Name: '', Description: '' });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.Name.trim()) {
      setError('Team name is required');
      return;
    }

    setCreating(true);
    setError('');

    try {
      const newTeam = await teamService.createTeam({
        Name: formData.Name.trim(),
        Description: formData.Description?.trim() || undefined,
      });
      onSuccess({ Id: newTeam.Id, Name: newTeam.Name, Description: newTeam.Description });
      navigate(`/team/${newTeam.Id}`);
    } catch (err) {
      console.error('Failed to create team:', err);
      setError('Failed to create team');
    } finally {
      setCreating(false);
    }
  };

  return (
    <div className="dashboard-card">
      <h2 className="dashboard-section-title">Create New Team</h2>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: '1rem' }}>
          <label htmlFor="team-name" style={{ display: 'block', marginBottom: '0.5rem', color: 'var(--color-text-secondary)' }}>
            Team Name *
          </label>
          <input
            id="team-name"
            type="text"
            value={formData.Name}
            onChange={(e) => setFormData({ ...formData, Name: e.target.value })}
            required
            style={{
              width: '100%',
              padding: '0.75rem',
              background: 'rgba(30, 41, 59, 0.5)',
              border: '1px solid var(--color-border-subtle)',
              borderRadius: '8px',
              color: 'var(--color-text-primary)',
              fontFamily: 'var(--font-body)',
              fontSize: '1rem',
            }}
          />
        </div>
        <div style={{ marginBottom: '1rem' }}>
          <label htmlFor="team-description" style={{ display: 'block', marginBottom: '0.5rem', color: 'var(--color-text-secondary)' }}>
            Description (optional)
          </label>
          <textarea
            id="team-description"
            value={formData.Description || ''}
            onChange={(e) => setFormData({ ...formData, Description: e.target.value })}
            rows={3}
            style={{
              width: '100%',
              padding: '0.75rem',
              background: 'rgba(30, 41, 59, 0.5)',
              border: '1px solid var(--color-border-subtle)',
              borderRadius: '8px',
              color: 'var(--color-text-primary)',
              fontFamily: 'var(--font-body)',
              fontSize: '1rem',
              resize: 'vertical',
            }}
          />
        </div>
        {error && <p className="dashboard-error" style={{ marginBottom: '1rem' }}>{error}</p>}
        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button
            type="submit"
            disabled={creating}
            className="dashboard-button"
            style={{ marginTop: 0 }}
          >
            {creating ? 'Creating...' : 'Create Team'}
          </button>
          <button
            type="button"
            onClick={onCancel}
            className="dashboard-button"
            style={{
              marginTop: 0,
              background: 'transparent',
              borderColor: 'var(--color-border-primary)',
            }}
          >
            Cancel
          </button>
        </div>
      </form>
    </div>
  );
}

