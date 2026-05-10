import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { teamService, type TeamListDto } from '../services/team';
import { challengeService, type ChallengeDto, type CreateChallengeDto, type ChallengeType, type ChallengeFrequency } from '../services/challenge';

const CHALLENGE_TYPES: ChallengeType[] = ['Cardio', 'Strength', 'SkillBased', 'Other'];
const CHALLENGE_FREQUENCIES: ChallengeFrequency[] = ['Daily', 'Weekly', 'Monthly', 'OneTime', 'Custom'];

const today = () => new Date().toISOString().split('T')[0];
const inDays = (n: number) => new Date(Date.now() + n * 86400000).toISOString().split('T')[0];

export default function ExercisesPage() {
  const { user } = useAuth();
  const [teams, setTeams] = useState<TeamListDto[]>([]);
  const [selectedTeamId, setSelectedTeamId] = useState<number | null>(null);
  const [challenges, setChallenges] = useState<ChallengeDto[]>([]);
  const [loading, setLoading] = useState(true);

  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState<CreateChallengeDto>({
    Name: '',
    Description: '',
    Type: 'Cardio',
    Frequency: 'Weekly',
    StartDate: today(),
    EndDate: inDays(30),
    TeamId: 0,
  });
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');

  useEffect(() => {
    loadTeams();
  }, []);

  useEffect(() => {
    if (selectedTeamId) {
      loadChallenges(selectedTeamId);
      setForm(p => ({ ...p, TeamId: selectedTeamId }));
    }
  }, [selectedTeamId]);

  async function loadTeams() {
    try {
      const data = await teamService.getMyTeams();
      setTeams(data);
      if (data.length > 0) setSelectedTeamId(data[0].Id);
    } catch (err) {
      console.error('Failed to load teams:', err);
    } finally {
      setLoading(false);
    }
  }

  async function loadChallenges(teamId: number) {
    try {
      const data = await challengeService.getTeamChallenges(teamId);
      setChallenges(data);
    } catch (err) {
      console.error('Failed to load challenges:', err);
    }
  }

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    if (!form.Name.trim() || !form.TeamId) return;
    setCreating(true);
    setCreateError('');
    try {
      const newChallenge = await challengeService.createChallenge(form);
      setChallenges(prev => [newChallenge, ...prev]);
      setShowForm(false);
      setForm(p => ({ ...p, Name: '', Description: '', StartDate: today(), EndDate: inDays(30) }));
    } catch {
      setCreateError('Failed to create challenge');
    } finally {
      setCreating(false);
    }
  }

  async function handleDelete(challengeId: number) {
    if (!confirm('Delete this challenge?')) return;
    try {
      await challengeService.deleteChallenge(challengeId);
      setChallenges(prev => prev.filter(c => c.Id !== challengeId));
    } catch {
      alert('Failed to delete challenge');
    }
  }

  if (loading) return <div className="loading-screen">Loading...</div>;

  const now = new Date();
  const active = challenges.filter(c => new Date(c.EndDate) >= now);
  const past = challenges.filter(c => new Date(c.EndDate) < now);

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">Challenges</h1>
        </div>

        {teams.length === 0 ? (
          <div className="dashboard-card">
            <p className="dashboard-info">You have no teams yet. Create a team from the dashboard first.</p>
          </div>
        ) : (
          <>
            {teams.length > 1 && (
              <div className="team-tabs">
                {teams.map(t => (
                  <button
                    key={t.Id}
                    className={`team-tab ${selectedTeamId === t.Id ? 'active' : ''}`}
                    onClick={() => setSelectedTeamId(t.Id)}
                  >
                    {t.Name}
                  </button>
                ))}
              </div>
            )}

            <div className="dashboard-card">
              <div className="section-actions">
                <h2 className="dashboard-section-title" style={{ margin: 0 }}>
                  {teams.length === 1 ? teams[0].Name : (teams.find(t => t.Id === selectedTeamId)?.Name ?? '')}
                </h2>
                <button className="btn-primary btn-sm" onClick={() => setShowForm(v => !v)}>
                  {showForm ? 'Cancel' : '+ New Challenge'}
                </button>
              </div>

              {showForm && (
                <form onSubmit={handleCreate} className="form-inline" style={{ margin: '1rem 0 1.25rem' }}>
                  <div>
                    <label className="form-label">Name *</label>
                    <input
                      className="form-input"
                      value={form.Name}
                      onChange={e => setForm(p => ({ ...p, Name: e.target.value }))}
                      required
                      placeholder="e.g. 5km run"
                    />
                  </div>
                  <div>
                    <label className="form-label">Description</label>
                    <input
                      className="form-input"
                      value={form.Description || ''}
                      onChange={e => setForm(p => ({ ...p, Description: e.target.value }))}
                      placeholder="Optional description"
                    />
                  </div>
                  <div className="form-row">
                    <div>
                      <label className="form-label">Type</label>
                      <select
                        className="form-select"
                        value={form.Type}
                        onChange={e => setForm(p => ({ ...p, Type: e.target.value as ChallengeType }))}
                      >
                        {CHALLENGE_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                      </select>
                    </div>
                    <div>
                      <label className="form-label">Frequency</label>
                      <select
                        className="form-select"
                        value={form.Frequency}
                        onChange={e => setForm(p => ({ ...p, Frequency: e.target.value as ChallengeFrequency }))}
                      >
                        {CHALLENGE_FREQUENCIES.map(f => <option key={f} value={f}>{f}</option>)}
                      </select>
                    </div>
                  </div>
                  <div className="form-row">
                    <div>
                      <label className="form-label">Start Date</label>
                      <input
                        type="date"
                        className="form-input"
                        value={form.StartDate}
                        onChange={e => setForm(p => ({ ...p, StartDate: e.target.value }))}
                      />
                    </div>
                    <div>
                      <label className="form-label">End Date</label>
                      <input
                        type="date"
                        className="form-input"
                        value={form.EndDate}
                        onChange={e => setForm(p => ({ ...p, EndDate: e.target.value }))}
                      />
                    </div>
                  </div>
                  {createError && <p className="dashboard-error">{createError}</p>}
                  <div className="form-actions">
                    <button type="submit" className="btn-primary" disabled={creating}>
                      {creating ? 'Creating...' : 'Create Challenge'}
                    </button>
                    <button type="button" className="btn-ghost" onClick={() => setShowForm(false)}>
                      Cancel
                    </button>
                  </div>
                </form>
              )}

              {challenges.length === 0 ? (
                <p className="dashboard-placeholder">No challenges yet for this team.</p>
              ) : (
                <>
                  {active.length > 0 && (
                    <div className="challenge-grid">
                      {active.map(c => (
                        <ExerciseChallengeCard
                          key={c.Id}
                          challenge={c}
                          canDelete={c.CreatedById === user?.Id}
                          onDelete={() => handleDelete(c.Id)}
                        />
                      ))}
                    </div>
                  )}
                  {past.length > 0 && (
                    <>
                      <p className="form-label" style={{ margin: '1rem 0 0.5rem' }}>Past</p>
                      <div className="challenge-grid">
                        {past.map(c => (
                          <ExerciseChallengeCard
                            key={c.Id}
                            challenge={c}
                            canDelete={c.CreatedById === user?.Id}
                            onDelete={() => handleDelete(c.Id)}
                            past
                          />
                        ))}
                      </div>
                    </>
                  )}
                </>
              )}
            </div>
          </>
        )}
      </div>
    </div>
  );
}

function ExerciseChallengeCard({ challenge, canDelete, onDelete, past }: {
  challenge: ChallengeDto;
  canDelete: boolean;
  onDelete: () => void;
  past?: boolean;
}) {
  return (
    <div className={`challenge-card ${past ? 'past' : ''}`}>
      <div className="challenge-card-header">
        <div style={{ flex: 1, minWidth: 0 }}>
          <h3 className="challenge-name">{challenge.Name}</h3>
          {challenge.Description && <p className="challenge-desc">{challenge.Description}</p>}
        </div>
        <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'flex-start', flexShrink: 0 }}>
          <span className="badge badge-count">{challenge.CompletionCount} done</span>
          {canDelete && (
            <button className="btn-danger btn-sm" onClick={onDelete}>Delete</button>
          )}
        </div>
      </div>
      <div className="challenge-meta">
        <span className="badge badge-type">{challenge.Type}</span>
        <span className="badge badge-freq">{challenge.Frequency}</span>
        <span className="challenge-date">
          {new Date(challenge.StartDate).toLocaleDateString('sv-SE')} – {new Date(challenge.EndDate).toLocaleDateString('sv-SE')}
        </span>
      </div>
    </div>
  );
}
