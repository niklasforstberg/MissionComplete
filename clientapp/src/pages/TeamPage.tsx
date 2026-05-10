import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { teamService, type TeamDto } from '../services/team';
import { challengeService, type ChallengeDto, type CreateChallengeDto, type ChallengeType, type ChallengeFrequency } from '../services/challenge';
import { useAuth } from '../contexts/AuthContext';

const CHALLENGE_TYPES: ChallengeType[] = ['Cardio', 'Strength', 'SkillBased', 'Other'];
const CHALLENGE_FREQUENCIES: ChallengeFrequency[] = ['Daily', 'Weekly', 'Monthly', 'OneTime', 'Custom'];

const today = () => new Date().toISOString().split('T')[0];
const inDays = (n: number) => new Date(Date.now() + n * 86400000).toISOString().split('T')[0];

export default function TeamPage() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();
  const teamId = parseInt(id!);

  const [team, setTeam] = useState<TeamDto | null>(null);
  const [challenges, setChallenges] = useState<ChallengeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Invite form
  const [inviteEmail, setInviteEmail] = useState('');
  const [inviting, setInviting] = useState(false);
  const [inviteError, setInviteError] = useState('');
  const [inviteSuccess, setInviteSuccess] = useState('');

  // Create challenge form
  const [showChallengeForm, setShowChallengeForm] = useState(false);
  const [challengeForm, setChallengeForm] = useState<Omit<CreateChallengeDto, 'TeamId'>>({
    Name: '',
    Description: '',
    Type: 'Cardio',
    Frequency: 'Weekly',
    StartDate: today(),
    EndDate: inDays(30),
  });
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');

  useEffect(() => {
    loadAll();
  }, [teamId]);

  async function loadAll() {
    try {
      const [teamData, challengeData] = await Promise.all([
        teamService.getTeam(teamId),
        challengeService.getTeamChallenges(teamId),
      ]);
      setTeam(teamData);
      setChallenges(challengeData);
    } catch {
      setError('Failed to load team');
    } finally {
      setLoading(false);
    }
  }

  async function handleInvite(e: React.FormEvent) {
    e.preventDefault();
    if (!inviteEmail.trim()) return;
    setInviting(true);
    setInviteError('');
    setInviteSuccess('');
    try {
      const member = await teamService.inviteMember(teamId, inviteEmail.trim());
      setTeam(prev => prev ? { ...prev, Members: [...prev.Members, member] } : prev);
      setInviteSuccess(`Invitation sent to ${inviteEmail}`);
      setInviteEmail('');
    } catch {
      setInviteError('Failed to send invite. Check the email address.');
    } finally {
      setInviting(false);
    }
  }

  async function handleRemoveMember(userId: number, email: string) {
    if (!confirm(`Remove ${email} from the team?`)) return;
    try {
      await teamService.removeMember(teamId, userId);
      setTeam(prev => prev ? { ...prev, Members: prev.Members.filter(m => m.UserId !== userId) } : prev);
    } catch {
      alert('Failed to remove member');
    }
  }

  async function handleCreateChallenge(e: React.FormEvent) {
    e.preventDefault();
    if (!challengeForm.Name.trim()) return;
    setCreating(true);
    setCreateError('');
    try {
      const newChallenge = await challengeService.createChallenge({ ...challengeForm, TeamId: teamId });
      setChallenges(prev => [newChallenge, ...prev]);
      setShowChallengeForm(false);
      setChallengeForm({ Name: '', Description: '', Type: 'Cardio', Frequency: 'Weekly', StartDate: today(), EndDate: inDays(30) });
    } catch {
      setCreateError('Failed to create challenge');
    } finally {
      setCreating(false);
    }
  }

  async function handleDeleteChallenge(challengeId: number) {
    if (!confirm('Delete this challenge?')) return;
    try {
      await challengeService.deleteChallenge(challengeId);
      setChallenges(prev => prev.filter(c => c.Id !== challengeId));
    } catch {
      alert('Failed to delete challenge');
    }
  }

  if (loading) return <div className="loading-screen">Loading...</div>;
  if (error || !team) return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-card"><p className="dashboard-error">{error || 'Team not found'}</p></div>
      </div>
    </div>
  );

  const now = new Date();
  const activeChallenges = challenges.filter(c => new Date(c.EndDate) >= now);
  const pastChallenges = challenges.filter(c => new Date(c.EndDate) < now);

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <div>
            <Link to="/dashboard" className="dashboard-back-link">← Back</Link>
          </div>
        </div>

        <div style={{ marginBottom: '1.5rem' }}>
          <h1 className="dashboard-title">{team.Name}</h1>
          {team.Description && <p style={{ color: 'var(--color-text-secondary)', marginTop: '0.5rem' }}>{team.Description}</p>}
        </div>

        {/* Members */}
        <div className="dashboard-card">
          <div className="section-actions">
            <h2 className="dashboard-section-title" style={{ margin: 0 }}>Players</h2>
          </div>

          {team.Members.length === 0 ? (
            <p className="dashboard-placeholder" style={{ marginBottom: '1rem' }}>No players yet.</p>
          ) : (
            <div className="member-list" style={{ marginBottom: '1.25rem' }}>
              {team.Members.map(m => (
                <div key={m.UserId} className="member-row">
                  <div>
                    <span className="member-email">{m.Email}</span>
                    <span className="member-joined">Joined {new Date(m.JoinedAt).toLocaleDateString('sv-SE')}</span>
                  </div>
                  <button
                    className="btn-danger btn-sm"
                    onClick={() => handleRemoveMember(m.UserId, m.Email)}
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          )}

          <form onSubmit={handleInvite} className="invite-form">
            <div style={{ flex: 1 }}>
              <label className="form-label">Invite by email</label>
              <input
                type="email"
                className="form-input"
                placeholder="player@example.com"
                value={inviteEmail}
                onChange={e => setInviteEmail(e.target.value)}
              />
            </div>
            <button type="submit" className="btn-primary" disabled={inviting} style={{ alignSelf: 'flex-end' }}>
              {inviting ? 'Sending...' : 'Invite'}
            </button>
          </form>
          {inviteError && <p className="dashboard-error" style={{ marginTop: '0.5rem' }}>{inviteError}</p>}
          {inviteSuccess && <p style={{ color: '#4ade80', fontSize: '0.875rem', marginTop: '0.5rem' }}>{inviteSuccess}</p>}
        </div>

        {/* Challenges */}
        <div className="dashboard-card">
          <div className="section-actions">
            <h2 className="dashboard-section-title" style={{ margin: 0 }}>Challenges</h2>
            <button className="btn-primary btn-sm" onClick={() => setShowChallengeForm(v => !v)}>
              {showChallengeForm ? 'Cancel' : '+ New Challenge'}
            </button>
          </div>

          {showChallengeForm && (
            <form onSubmit={handleCreateChallenge} className="form-inline" style={{ margin: '1rem 0 1.25rem' }}>
              <div>
                <label className="form-label">Name *</label>
                <input
                  className="form-input"
                  value={challengeForm.Name}
                  onChange={e => setChallengeForm(p => ({ ...p, Name: e.target.value }))}
                  required
                  placeholder="e.g. 5km run"
                />
              </div>
              <div>
                <label className="form-label">Description</label>
                <input
                  className="form-input"
                  value={challengeForm.Description || ''}
                  onChange={e => setChallengeForm(p => ({ ...p, Description: e.target.value }))}
                  placeholder="Optional description"
                />
              </div>
              <div className="form-row">
                <div>
                  <label className="form-label">Type</label>
                  <select
                    className="form-select"
                    value={challengeForm.Type}
                    onChange={e => setChallengeForm(p => ({ ...p, Type: e.target.value as ChallengeType }))}
                  >
                    {CHALLENGE_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                  </select>
                </div>
                <div>
                  <label className="form-label">Frequency</label>
                  <select
                    className="form-select"
                    value={challengeForm.Frequency}
                    onChange={e => setChallengeForm(p => ({ ...p, Frequency: e.target.value as ChallengeFrequency }))}
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
                    value={challengeForm.StartDate}
                    onChange={e => setChallengeForm(p => ({ ...p, StartDate: e.target.value }))}
                  />
                </div>
                <div>
                  <label className="form-label">End Date</label>
                  <input
                    type="date"
                    className="form-input"
                    value={challengeForm.EndDate}
                    onChange={e => setChallengeForm(p => ({ ...p, EndDate: e.target.value }))}
                  />
                </div>
              </div>
              {createError && <p className="dashboard-error">{createError}</p>}
              <div className="form-actions">
                <button type="submit" className="btn-primary" disabled={creating}>
                  {creating ? 'Creating...' : 'Create Challenge'}
                </button>
                <button type="button" className="btn-ghost" onClick={() => setShowChallengeForm(false)}>
                  Cancel
                </button>
              </div>
            </form>
          )}

          {challenges.length === 0 ? (
            <p className="dashboard-placeholder">No challenges yet. Create one above.</p>
          ) : (
            <>
              {activeChallenges.length > 0 && (
                <div className="challenge-grid" style={{ marginBottom: pastChallenges.length > 0 ? '1rem' : 0 }}>
                  {activeChallenges.map(c => (
                    <CoachChallengeCard
                      key={c.Id}
                      challenge={c}
                      canDelete={c.CreatedById === user?.Id}
                      onDelete={() => handleDeleteChallenge(c.Id)}
                    />
                  ))}
                </div>
              )}
              {pastChallenges.length > 0 && (
                <>
                  <p className="form-label" style={{ marginTop: '0.75rem', marginBottom: '0.5rem' }}>Past</p>
                  <div className="challenge-grid">
                    {pastChallenges.map(c => (
                      <CoachChallengeCard
                        key={c.Id}
                        challenge={c}
                        canDelete={c.CreatedById === user?.Id}
                        onDelete={() => handleDeleteChallenge(c.Id)}
                        past
                      />
                    ))}
                  </div>
                </>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}

function CoachChallengeCard({ challenge, canDelete, onDelete, past }: {
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
