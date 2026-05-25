using UnityEngine;
using Shared.Enums;
using Shared.Models;
using System.Collections.Generic;

namespace EVOLUTION.Gameplay.Managers
{
    /// <summary>
    /// Match Manager - controls match lifecycle, respawn logic, win conditions
    /// Singleton pattern for global access during gameplay
    /// </summary>
    public class MatchManager : MonoBehaviour
    {
        [Header("Match Settings")]
        [SerializeField] private GameMode gameMode;
        [SerializeField] private float matchDuration = 180f; // 3 minutes
        [SerializeField] private int targetScore = 10;
        
        [Header("Respawn Settings")]
        [SerializeField] private float respawnTime = 10f;
        [SerializeField] private float spawnProtectionTime = 2f;
        
        [Header("Battle Royale Settings")]
        [SerializeField] private float shrinkInterval = 30f;
        [SerializeField] private float shrinkAmount = 50f;
        [SerializeField] private float initialZoneRadius = 1200f;
        
        // Singleton
        private static MatchManager _instance;
        public static MatchManager Instance => _instance;
        
        // State
        private MatchData _matchData;
        private float _matchTimer;
        private bool _matchStarted;
        private bool _matchEnded;
        private Dictionary<string, Hero> _playerHeroes = new();
        private Dictionary<string, float> _respawnTimers = new();
        private List<string> _deadPlayers = new();
        
        // Battle Royale zone
        private float _currentZoneRadius;
        private Vector2 _zoneCenter;
        
        // Events
        public event System.Action<MatchState> OnMatchStateChanged;
        public event System.Action<float> OnMatchTimerUpdated;
        public event System.ActionEvent<string, Hero> OnPlayerSpawned;
        public event System.ActionEvent<string, Hero> OnPlayerDied;
        public event System<Action<Hero, Hero>> OnKillOccurred;
        public event System.Action<int> OnTeamScoreChanged;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            InitializeMatch();
        }
        
        private void Update()
        {
            if (!_matchStarted || _matchEnded) return;
            
            UpdateMatchTimer();
            UpdateRespawnTimers();
            
            if (gameMode == GameMode.BattleRoyale)
            {
                UpdateShrinkingZone();
            }
            
            CheckWinCondition();
        }
        
        #region Initialization
        
        private void InitializeMatch()
        {
            _matchData = new MatchData
            {
                Mode = gameMode,
                State = MatchState.WaitingForPlayers,
                StartTime = System.DateTime.Now,
                MaxPlayers = GetMaxPlayersForMode(gameMode)
            };
            
            _matchTimer = matchDuration;
            _currentZoneRadius = initialZoneRadius;
            _zoneCenter = Vector2.zero;
            
            _matchStarted = false;
            _matchEnded = false;
            
            OnMatchStateChanged?.Invoke(MatchState.WaitingForPlayers);
        }
        
        private int GetMaxPlayersForMode(GameMode mode)
        {
            return mode switch
            {
                GameMode.BattleRoyale => 12,
                GameMode.CaptureTheFlag => 6,
                GameMode.Siege => 10,
                GameMode.TitanTrial => 5,
                _ => 6
            };
        }
        
        public void StartMatch()
        {
            if (_matchStarted) return;
            
            _matchStarted = true;
            _matchData.State = MatchState.InProgress;
            _matchData.StartTime = System.DateTime.Now;
            
            OnMatchStateChanged?.Invoke(MatchState.InProgress);
            
            // Spawn all players
            foreach (var playerData in _matchData.Players)
            {
                SpawnPlayer(playerData.Id, GetRandomSpawnPoint());
            }
        }
        
        #endregion
        
        #region Spawning
        
        public void SpawnPlayer(string playerId, Vector3 spawnPosition)
        {
            // Create hero instance (would be networked in multiplayer)
            GameObject heroObj = CreateHeroPrefab(playerId);
            heroObj.transform.position = spawnPosition;
            
            Hero hero = heroObj.GetComponent<Hero>();
            _playerHeroes[playerId] = hero;
            
            _deadPlayers.Remove(playerId);
            _respawnTimers.Remove(playerId);
            
            _matchData.AlivePlayers.Add(playerId);
            
            OnPlayerSpawned?.Invoke(playerId, hero);
        }
        
        private GameObject CreateHeroPrefab(string playerId)
        {
            // In production, this would load from resources/pool
            GameObject heroObj = new GameObject($"Hero_{playerId}");
            heroObj.AddComponent<PlayerController>();
            heroObj.AddComponent<CombatSystem>();
            heroObj.AddComponent<Hero>();
            
            // Set team based on match data
            var player = _matchData.Players.Find(p => p.Id == playerId);
            if (player != null)
            {
                heroObj.GetComponent<Hero>().SetHeroData(new HeroData
                {
                    Id = playerId,
                    TeamId = player.TeamId
                });
            }
            
            return heroObj;
        }
        
        private Vector3 GetRandomSpawnPoint()
        {
            // In production, use predefined spawn points from map data
            float x = Random.Range(-1000f, 1000f);
            float y = Random.Range(-600f, 600f);
            return new Vector3(x, y, 0);
        }
        
        #endregion
        
        #region Death & Respawn
        
        public void HandleHeroDeath(Hero hero, HeroData killer)
        {
            if (!_matchStarted || _matchEnded) return;
            
            string playerId = hero.HeroData?.Id ?? "unknown";
            
            _deadPlayers.Add(playerId);
            _matchData.AlivePlayers.Remove(playerId);
            
            OnPlayerDied?.Invoke(playerId, hero);
            
            // Record kill for killer
            if (killer != null)
            {
                OnKillOccurred?.Invoke(hero, GetHeroByData(killer));
            }
            
            // Check if respawn is enabled for this mode
            if (IsRespawnEnabled())
            {
                StartRespawnTimer(playerId);
            }
            else
            {
                // Battle Royale - check if last player alive
                if (_matchData.AlivePlayers.Count <= 1)
                {
                    EndMatch();
                }
            }
        }
        
        private bool IsRespawnEnabled()
        {
            return gameMode == GameMode.CaptureTheFlag || 
                   gameMode == GameMode.Siege;
        }
        
        private void StartRespawnTimer(string playerId)
        {
            _respawnTimers[playerId] = respawnTime;
        }
        
        private void UpdateRespawnTimers()
        {
            var playersToRespawn = new List<string>();
            
            foreach (var kvp in _respawnTimers)
            {
                _respawnTimers[kvp.Key] -= Time.deltaTime;
                
                if (_respawnTimers[kvp.Key] <= 0f)
                {
                    playersToRespawn.Add(kvp.Key);
                }
            }
            
            foreach (var playerId in playersToRespawn)
            {
                RespawnPlayer(playerId);
            }
        }
        
        private void RespawnPlayer(string playerId)
        {
            if (!_deadPlayers.Contains(playerId)) return;
            
            Vector3 spawnPoint = GetSafeSpawnPoint();
            SpawnPlayer(playerId, spawnPoint);
            
            _respawnTimers.Remove(playerId);
        }
        
        private Vector3 GetSafeSpawnPoint()
        {
            // Find spawn point away from enemies
            // For now, return random point with spawn protection
            return GetRandomSpawnPoint();
        }
        
        #endregion
        
        #region Match Control
        
        private void UpdateMatchTimer()
        {
            _matchTimer -= Time.deltaTime;
            OnMatchTimerUpdated?.Invoke(_matchTimer);
            
            if (_matchTimer <= 0f)
            {
                EndMatch();
            }
        }
        
        private void CheckWinCondition()
        {
            // Check score-based win
            if (_matchData.TeamScores.ContainsKey(0) && _matchData.TeamScores[0] >= targetScore)
            {
                EndMatch(0);
                return;
            }
            
            if (_matchData.TeamScores.ContainsKey(1) && _matchData.TeamScores[1] >= targetScore)
            {
                EndMatch(1);
                return;
            }
            
            // Battle Royale - last player standing
            if (gameMode == GameMode.BattleRoyale && _matchData.AlivePlayers.Count <= 1)
            {
                EndMatch();
            }
        }
        
        public void EndMatch(int winningTeamId = -1)
        {
            if (_matchEnded) return;
            
            _matchEnded = true;
            _matchData.State = MatchState.Finished;
            _matchData.EndTime = System.DateTime.Now;
            _matchData.WinningTeamId = winningTeamId;
            
            // Determine winner for Battle Royale
            if (gameMode == GameMode.BattleRoyale && _matchData.AlivePlayers.Count == 1)
            {
                var winnerId = _matchData.AlivePlayers[0];
                var winner = GetHeroById(winnerId);
                winningTeamId = winner?.GetTeamId() ?? -1;
            }
            
            OnMatchStateChanged?.Invoke(MatchState.Finished);
            
            // Calculate MVP, rewards, etc.
            CalculateMVP();
            DistributeRewards();
        }
        
        private void CalculateMVP()
        {
            // Find player with highest score
            string mvpId = "";
            int highestScore = 0;
            
            foreach (var player in _matchData.Players)
            {
                int score = player.Stats.Kills * 2 + player.Stats.Deaths - player.Stats.Deaths;
                if (score > highestScore)
                {
                    highestScore = score;
                    mvpId = player.Id;
                }
            }
            
            _matchData.MVP = mvpId;
        }
        
        private void DistributeRewards()
        {
            // Award XP, trophies, currency based on performance
            foreach (var player in _matchData.Players)
            {
                int xpReward = 30; // Base XP
                player.Stats.TotalXP += xpReward;
            }
        }
        
        #endregion
        
        #region Battle Royale Zone
        
        private void UpdateShrinkingZone()
        {
            if (_matchTimer % shrinkInterval < Time.deltaTime)
            {
                _currentZoneRadius -= shrinkAmount;
                _currentZoneRadius = Mathf.Max(_currentZoneRadius, 100f);
                
                // Damage players outside zone
                DamagePlayersOutsideZone();
            }
        }
        
        private void DamagePlayersOutsideZone()
        {
            foreach (var hero in _playerHeroes.Values)
            {
                if (hero == null || hero.IsDead) continue;
                
                float distance = Vector2.Distance(hero.transform.position, _zoneCenter);
                if (distance > _currentZoneRadius)
                {
                    // Apply zone damage
                    hero.TakeDamage(10f, null, DamageType.Normal, ProjectileType.None);
                }
            }
        }
        
        #endregion
        
        #region Helpers
        
        private Hero GetHeroById(string playerId)
        {
            return _playerHeroes.TryGetValue(playerId, out Hero hero) ? hero : null;
        }
        
        private Hero GetHeroByData(HeroData data)
        {
            foreach (var hero in _playerHeroes.Values)
            {
                if (hero.HeroData == data) return hero;
            }
            return null;
        }
        
        public MatchData GetMatchData() => _matchData;
        public float GetMatchTimer() => _matchTimer;
        public bool IsMatchStarted() => _matchStarted;
        public bool IsMatchEnded() => _matchEnded;
        
        #endregion
    }
}
