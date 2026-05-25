using System;

namespace EVOLUTION.Shared.Models
{
    /// <summary>
    /// Match data model for multiplayer sessions
    /// </summary>
    [Serializable]
    public class MatchData
    {
        public string Id { get; set; }
        public GameMode Mode { get; set; }
        public MatchState State { get; set; }
        
        // Teams
        public TeamData BlueTeam { get; set; }
        public TeamData RedTeam { get; set; }
        
        // Timing
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float MatchDuration { get; set; } // seconds
        
        // Map
        public MapData Map { get; set; }
        
        // Ban phase (for ranked)
        public BanPhaseData BanPhase { get; set; }
        
        // Battle Royale specific
        public ZoneData ShrinkingZone { get; set; }
        
        // Titan Trial specific
        public TitanBossData TitanBoss { get; set; }
        
        // Results
        public TeamId WinningTeam { get; set; }
        public PlayerMatchResult[] PlayerResults { get; set; }
        public HeroMatchStats[] HeroStats { get; set; }
        
        // Recording
        public ReplayData Replay { get; set; }
        
        public MatchData()
        {
            Id = Guid.NewGuid().ToString();
            BlueTeam = new TeamData { Id = TeamId.Blue };
            RedTeam = new TeamData { Id = TeamId.Red };
            PlayerResults = Array.Empty<PlayerMatchResult>();
            HeroStats = Array.Empty<HeroMatchStats>();
        }
    }

    /// <summary>
    /// Team data
    /// </summary>
    [Serializable]
    public class TeamData
    {
        public TeamId Id { get; set; }
        public PlayerInfo[] Players { get; set; }
        public int Score { get; set; }
        public bool HasSurrendered { get; set; }
        public string[] BannedHeroIds { get; set; }
        
        public TeamData()
        {
            Players = Array.Empty<PlayerInfo>();
            BannedHeroIds = Array.Empty<string>();
        }
    }

    /// <summary>
    /// Player info in match context
    /// </summary>
    [Serializable]
    public class PlayerInfo
    {
        public string PlayerId { get; set; }
        public string Username { get; set; }
        public string HeroId { get; set; }
        public bool IsReady { get; set; }
        public bool IsBot { get; set; }
        public int Ping { get; set; }
        public bool IsAlive { get; set; }
        public float RespawnTimer { get; set; }
        public int Level { get; set; } // In-match level
        public int Gold { get; set; } // In-match currency
    }

    /// <summary>
    /// Ban phase data for ranked matches
    /// </summary>
    [Serializable]
    public class BanPhaseData
    {
        public bool IsActive { get; set; }
        public int BansPerTeam { get; set; }
        public float TimePerBan { get; set; }
        public string[] BlueTeamBans { get; set; }
        public string[] RedTeamBans { get; set; }
        public bool BlueTeamVoted { get; set; }
        public bool RedTeamVoted { get; set; }
        
        public BanPhaseData()
        {
            BansPerTeam = 3;
            TimePerBan = GameConstants.BAN_PHASE_DURATION;
            BlueTeamBans = Array.Empty<string>();
            RedTeamBans = Array.Empty<string>();
        }
    }

    /// <summary>
    /// Shrinking zone for Battle Royale
    /// </summary>
    [Serializable]
    public class ZoneData
    {
        public Vector2Float Center { get; set; }
        public float CurrentRadius { get; set; }
        public float TargetRadius { get; set; }
        public float ShrinkSpeed { get; set; }
        public float DamagePerSecond { get; set; }
        public int CurrentPhase { get; set; }
        public float TimeUntilShrink { get; set; }
    }

    /// <summary>
    /// Titan boss data for Titan Trial mode
    /// </summary>
    [Serializable]
    public class TitanBossData
    {
        public string BossId { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public Vector2Float Position { get; set; }
        public float AggroRadius { get; set; }
        public string CurrentTargetId { get; set; }
        public float AttackCooldown { get; set; }
        public float LastAttackTime { get; set; }
    }

    /// <summary>
    /// Player result after match
    /// </summary>
    [Serializable]
    public class PlayerMatchResult
    {
        public string PlayerId { get; set; }
        public string Username { get; set; }
        public TeamId Team { get; set; }
        public string HeroId { get; set; }
        public bool IsVictory { get; set; }
        public int TrophyChange { get; set; }
        public int XPGranted { get; set; }
        public RewardData[] Rewards { get; set; }
        public int MVPVotes { get; set; }
        public bool IsMVP { get; set; }
        public string Title { get; set; } // e.g., "First Blood", "Legendary"
    }

    /// <summary>
    /// Simple vector struct for networking
    /// </summary>
    [Serializable]
    public struct Vector2Float
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public Vector2Float(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Vector3 for 3D positions
    /// </summary>
    [Serializable]
    public struct Vector3Float
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public Vector3Float(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    /// Replay data for match recording
    /// </summary>
    [Serializable]
    public class ReplayData
    {
        public string MatchId { get; set; }
        public int RecordedFPS { get; set; }
        public float Duration { get; set; }
        public byte[] CompressedData { get; set; }
        public DateTime RecordingDate { get; set; }
        public string MapName { get; set; }
        public GameMode Mode { get; set; }
        public PlayerInfo[] Participants { get; set; }
    }
}
