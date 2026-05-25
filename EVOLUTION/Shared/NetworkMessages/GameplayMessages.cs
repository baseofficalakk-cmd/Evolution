using System;
using Evolution.Shared.Enums;

namespace Evolution.Networking.Messages
{
    // ============================================
    // CONNECTION MESSAGES
    // ============================================
    
    [Serializable]
    public class ConnectRequestMessage : INetworkMessage
    {
        public string Username { get; set; }
        public string SessionToken { get; set; }
        public int ClientVersion { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class ConnectResponseMessage : INetworkMessage
    {
        public bool Success { get; set; }
        public string PlayerId { get; set; }
        public string ErrorMessage { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class DisconnectMessage : INetworkMessage
    {
        public string Reason { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    // ============================================
    // PLAYER MESSAGES
    // ============================================
    
    [Serializable]
    public class PlayerInputMessage : INetworkMessage
    {
        public float MoveX { get; set; }
        public float MoveY { get; set; }
        public UnityEngine.Vector2 AimDirection { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsManualAiming { get; set; }
        public bool IsUsingUltimate { get; set; }
        public int SequenceNumber { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    [Serializable]
    public class PlayerStateMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public UnityEngine.Vector3 Position { get; set; }
        public UnityEngine.Vector2 Rotation { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public bool IsDead { get; set; }
        public int SequenceNumber { get; set; }
        public float Timestamp { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    [Serializable]
    public class PlayerSpawnMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public string HeroId { get; set; }
        public UnityEngine.Vector3 SpawnPosition { get; set; }
        public int TeamId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class PlayerDeathMessage : INetworkMessage
    {
        public string VictimId { get; set; }
        public string KillerId { get; set; }
        public string HeroAbilityId { get; set; }
        public UnityEngine.Vector3 DeathPosition { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class PlayerRespawnMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public UnityEngine.Vector3 RespawnPosition { get; set; }
        public float RespawnTimeRemaining { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    // ============================================
    // COMBAT MESSAGES
    // ============================================
    
    [Serializable]
    public class ProjectileSpawnMessage : INetworkMessage
    {
        public string ProjectileId { get; set; }
        public string OwnerId { get; set; }
        public string AbilityId { get; set; }
        public UnityEngine.Vector3 SpawnPosition { get; set; }
        public UnityEngine.Vector2 Direction { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    [Serializable]
    public class ProjectileHitMessage : INetworkMessage
    {
        public string ProjectileId { get; set; }
        public string TargetId { get; set; }
        public float Damage { get; set; }
        public UnityEngine.Vector3 HitPosition { get; set; }
        public bool IsCritical { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class AbilityUsedMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public string AbilityId { get; set; }
        public UnityEngine.Vector3 CastPosition { get; set; }
        public UnityEngine.Vector2 CastDirection { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class DamageDealtMessage : INetworkMessage
    {
        public string AttackerId { get; set; }
        public string VictimId { get; set; }
        public float Damage { get; set; }
        public DamageType DamageType { get; set; }
        public bool IsCritical { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    // ============================================
    // MATCH MESSAGES
    // ============================================
    
    [Serializable]
    public class MatchStartMessage : INetworkMessage
    {
        public string MatchId { get; set; }
        public GameMode GameMode { get; set; }
        public string MapId { get; set; }
        public float MatchDuration { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class MatchEndMessage : INetworkMessage
    {
        public string MatchId { get; set; }
        public int WinningTeamId { get; set; }
        public MatchResult[] Results { get; set; }
        public string MVPPlayerId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class MatchResult
    {
        public string PlayerId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public float DamageDealt { get; set; }
        public float HealingDone { get; set; }
        public int Score { get; set; }
    }
    
    [Serializable]
    public class MatchStateUpdateMessage : INetworkMessage
    {
        public float RemainingTime { get; set; }
        public int[] TeamScores { get; set; }
        public UnityEngine.Vector2 ZonePosition { get; set; }
        public float ZoneRadius { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    // ============================================
    // LOBBY & MATCHMAKING MESSAGES
    // ============================================
    
    [Serializable]
    public class QueueRequestMessage : INetworkMessage
    {
        public GameMode GameMode { get; set; }
        public int[] PartyMemberIds { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class QueueStatusMessage : INetworkMessage
    {
        public int PositionInQueue { get; set; }
        public float EstimatedWaitTime { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class MatchFoundMessage : INetworkMessage
    {
        public string MatchId { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class MatchCancelledMessage : INetworkMessage
    {
        public string Reason { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    // ============================================
    // HERO SELECTION & BAN PHASE
    // ============================================
    
    [Serializable]
    public class HeroSelectRequestMessage : INetworkMessage
    {
        public string HeroId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class HeroSelectResponseMessage : INetworkMessage
    {
        public bool Success { get; set; }
        public string PlayerId { get; set; }
        public string HeroId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class HeroBanRequestMessage : INetworkMessage
    {
        public string HeroId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class HeroBanResponseMessage : INetworkMessage
    {
        public bool Success { get; set; }
        public string BannedHeroId { get; set; }
        public string BanningTeamId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class BanPhaseStartMessage : INetworkMessage
    {
        public int BansPerTeam { get; set; }
        public float BanTimer { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class BanPhaseEndMessage : INetworkMessage
    {
        public string[] BannedHeroIds { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    // ============================================
    // CHAT & PING MESSAGES
    // ============================================
    
    [Serializable]
    public class ChatMessage : INetworkMessage
    {
        public string SenderId { get; set; }
        public string Content { get; set; }
        public ChatType ChatType { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class QuickChatMessage : INetworkMessage
    {
        public string SenderId { get; set; }
        public int QuickChatId { get; set; }
        public UnityEngine.Vector3 ContextPosition { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    [Serializable]
    public class PingMessage : INetworkMessage
    {
        public string SenderId { get; set; }
        public PingType PingType { get; set; }
        public UnityEngine.Vector3 PingPosition { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    // ============================================
    // GAME STATE SYNC MESSAGES
    // ============================================
    
    [Serializable]
    public class GameStateSnapshotMessage : INetworkMessage
    {
        public float Timestamp { get; set; }
        public PlayerStateMessage[] PlayerStates { get; set; }
        public ProjectileSpawnMessage[] ActiveProjectiles { get; set; }
        public MessageChannel Channel => MessageChannel.Unreliable;
    }
    
    [Serializable]
    public class LagCompensationRequestMessage : INetworkMessage
    {
        public string ShooterId { get; set; }
        public float ShotTimestamp { get; set; }
        public UnityEngine.Vector3 ShotPosition { get; set; }
        public UnityEngine.Vector2 ShotDirection { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class ReconnectRequestMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public string MatchId { get; set; }
        public int LastReceivedSequence { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class ReconnectResponseMessage : INetworkMessage
    {
        public bool Success { get; set; }
        public GameStateSnapshotMessage CurrentState { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    // ============================================
    // ADMIN & ANTI-CHEAT MESSAGES
    // ============================================
    
    [Serializable]
    public class AdminCommandMessage : INetworkMessage
    {
        public string AdminId { get; set; }
        public string Command { get; set; }
        public string[] Parameters { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class AdminResponseMessage : INetworkMessage
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class AntiCheatReportMessage : INetworkMessage
    {
        public string ReportedPlayerId { get; set; }
        public string CheatType { get; set; }
        public string Evidence { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class AFKWarningMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public float InactivityTime { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
    
    [Serializable]
    public class AFKKickMessage : INetworkMessage
    {
        public string PlayerId { get; set; }
        public MessageChannel Channel => MessageChannel.Reliable;
    }
}
