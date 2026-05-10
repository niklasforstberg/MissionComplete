import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { teamService, type UserTeamDto } from '../services/team';
import { challengeService, type ChallengeDto, type LeaderboardEntry } from '../services/challenge';

export default function PlayerDashboard() {
  const { user } = useAuth();
  const [teams, setTeams] = useState<UserTeamDto[]>([]);
  const [selectedTeamId, setSelectedTeamId] = useState<number | null>(null);
  const [challenges, setChallenges] = useState<ChallengeDto[]>([]);
  const [completedIds, setCompletedIds] = useState<Set<number>>(new Set());
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [completing, setCompleting] = useState<number | null>(null);

  useEffect(() => {
    Promise.all([loadTeams(), loadCompletedChallenges()]);
  }, []);

  useEffect(() => {
    if (selectedTeamId) loadTeamData(selectedTeamId);
  }, [selectedTeamId]);

  async function loadTeams() {
    try {
      const data = await teamService.getMyPlayerTeams();
      setTeams(data);
      if (data.length > 0) setSelectedTeamId(data[0].Id);
    } catch (err) {
      console.error('Failed to load teams:', err);
    } finally {
      setLoading(false);
    }
  }

  async function loadCompletedChallenges() {
    try {
      const completions = await challengeService.getMyCompletedChallenges();
      setCompletedIds(new Set(completions.map(c => c.Id)));
    } catch (err) {
      console.error('Failed to load completions:', err);
    }
  }

  async function loadTeamData(teamId: number) {
    try {
      const [challengeData, lbData] = await Promise.all([
        challengeService.getTeamChallenges(teamId),
        challengeService.getTeamLeaderboard(teamId),
      ]);
      setChallenges(challengeData);
      setLeaderboard(lbData);
    } catch (err) {
      console.error('Failed to load team data:', err);
    }
  }

  async function handleComplete(challengeId: number) {
    setCompleting(challengeId);
    try {
      await challengeService.completeChallenge(challengeId);
      setCompletedIds(prev => new Set([...prev, challengeId]));
      if (selectedTeamId) {
        const lbData = await challengeService.getTeamLeaderboard(selectedTeamId);
        setLeaderboard(lbData);
      }
    } catch (err) {
      console.error('Failed to complete challenge:', err);
    } finally {
      setCompleting(null);
    }
  }

  if (loading) return <div className="loading-screen">Loading...</div>;

  const now = new Date();
  const activeChallenges = challenges.filter(c => new Date(c.EndDate) >= now);
  const pastChallenges = challenges.filter(c => new Date(c.EndDate) < now);

  return (
    <div className="dashboard-page">
      <div className="dashboard-container">
        <div className="dashboard-header">
          <h1 className="dashboard-title">My Dashboard</h1>
        </div>

        {teams.length === 0 ? (
          <div className="dashboard-card">
            <p className="dashboard-info">You haven't been added to a team yet. Ask your coach to invite you.</p>
          </div>
        ) : (
          <>
            {teams.length > 1 && (
              <div className="team-tabs">
                {teams.map(team => (
                  <button
                    key={team.Id}
                    className={`team-tab ${selectedTeamId === team.Id ? 'active' : ''}`}
                    onClick={() => setSelectedTeamId(team.Id)}
                  >
                    {team.Name}
                  </button>
                ))}
              </div>
            )}

            <div className="dashboard-grid">
              <div className="dashboard-grid-main">
                <div className="dashboard-card">
                  <h2 className="dashboard-section-title">Active Challenges</h2>
                  {activeChallenges.length === 0 ? (
                    <p className="dashboard-placeholder">No active challenges right now.</p>
                  ) : (
                    <div className="challenge-grid">
                      {activeChallenges.map(c => (
                        <ChallengeCard
                          key={c.Id}
                          challenge={c}
                          completed={completedIds.has(c.Id)}
                          completing={completing === c.Id}
                          onComplete={() => handleComplete(c.Id)}
                        />
                      ))}
                    </div>
                  )}
                </div>

                {pastChallenges.length > 0 && (
                  <div className="dashboard-card">
                    <h2 className="dashboard-section-title">Past Challenges</h2>
                    <div className="challenge-grid">
                      {pastChallenges.map(c => (
                        <ChallengeCard
                          key={c.Id}
                          challenge={c}
                          completed={completedIds.has(c.Id)}
                          completing={false}
                          onComplete={() => {}}
                          readonly
                        />
                      ))}
                    </div>
                  </div>
                )}
              </div>

              <div className="dashboard-grid-side">
                <div className="dashboard-card">
                  <h2 className="dashboard-section-title">Leaderboard</h2>
                  {leaderboard.length === 0 ? (
                    <p className="dashboard-placeholder">No completions yet.</p>
                  ) : (
                    <div className="leaderboard-list">
                      {leaderboard.map((entry, idx) => (
                        <div key={entry.UserId} className="leaderboard-item">
                          <span className={`leaderboard-rank ${idx === 0 ? 'gold' : idx === 1 ? 'silver' : idx === 2 ? 'bronze' : ''}`}>
                            #{idx + 1}
                          </span>
                          <span className="leaderboard-name">
                            {entry.FirstName ? `${entry.FirstName} ${entry.LastName}`.trim() : entry.Email.split('@')[0]}
                            {entry.Email === user?.Email && <span className="leaderboard-you"> (you)</span>}
                          </span>
                          <span className="leaderboard-count">{entry.CompletionCount}</span>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}

interface ChallengeCardProps {
  challenge: ChallengeDto;
  completed: boolean;
  completing: boolean;
  onComplete: () => void;
  readonly?: boolean;
}

function ChallengeCard({ challenge, completed, completing, onComplete, readonly }: ChallengeCardProps) {
  const endDate = new Date(challenge.EndDate);
  const isExpired = endDate < new Date();

  return (
    <div className={`challenge-card ${completed ? 'completed' : ''}`}>
      <div className="challenge-card-header">
        <div style={{ flex: 1, minWidth: 0 }}>
          <h3 className="challenge-name">{challenge.Name}</h3>
          {challenge.Description && <p className="challenge-desc">{challenge.Description}</p>}
        </div>
        {!readonly && !isExpired && (
          <button
            className={`btn-complete ${completed ? 'done' : ''}`}
            onClick={onComplete}
            disabled={completed || completing}
          >
            {completing ? '...' : completed ? '✓ Done' : 'Complete'}
          </button>
        )}
        {(readonly || isExpired) && completed && (
          <span className="badge badge-done">✓ Done</span>
        )}
      </div>
      <div className="challenge-meta">
        <span className="badge badge-type">{challenge.Type}</span>
        <span className="badge badge-freq">{challenge.Frequency}</span>
        <span className="challenge-date">
          {new Date(challenge.StartDate).toLocaleDateString('sv-SE')} – {endDate.toLocaleDateString('sv-SE')}
        </span>
        {challenge.CompletionCount > 0 && (
          <span className="badge badge-count">{challenge.CompletionCount} completions</span>
        )}
      </div>
    </div>
  );
}
