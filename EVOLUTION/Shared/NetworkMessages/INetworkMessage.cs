using System;

namespace Shared.NetworkMessages
{
    /// <summary>
    /// Base interface for all network messages
    /// </summary>
    public interface INetworkMessage
    {
        MessageType Type { get; }
        bool IsReliable { get; }
        int SequenceNumber { get; set; }
        long Timestamp { get; set; }
    }
    
    /// <summary>
    /// Message types for client-server communication
    /// </summary>
    public enum MessageType
    {
        // Connection
        None = 0,
        ConnectRequest,
        ConnectResponse,
        Disconnect,
        Ping,
        Pong,
        
        // Matchmaking
        FindMatchRequest,
        MatchFound,
        JoinMatchRequest,
        JoinMatchResponse,
        LeaveMatch,
        
        // Gameplay - Player
        PlayerSpawn,
        PlayerMove,
        PlayerAim,
        PlayerAttack,
        PlayerAbility,
        PlayerDamage,
        PlayerDeath,
        PlayerRespawn,
        PlayerPositionUpdate,
        
        // Gameplay - Match
        MatchStart,
        MatchEnd,
        MatchStateUpdate,
        TimerUpdate,
        ScoreUpdate,
        
        // Chat & Social
        ChatMessage,
        PingSignal,
        FriendRequest,
        FriendResponse,
        PartyInvite,
        PartyResponse,
        
        // Progression
        XPUpdate,
        LevelUp,
        TrophyUpdate,
        RankUpdate,
        QuestProgress,
        QuestComplete,
        
        // Economy
        CurrencyUpdate,
        ChestOpenRequest,
        ChestOpenResponse,
        ShopPurchase,
        
        // Admin
        AdminCommand,
        AdminResponse,
        BanPlayer,
        MutePlayer
    }
    
    /// <summary>
    /// Reliable message base class
    /// </summary>
    [Serializable]
    public abstract class ReliableMessage : INetworkMessage
    {
        public abstract MessageType Type { get; }
        public bool IsReliable => true;
        public int SequenceNumber { get; set; }
        public long Timestamp { get; set; }
    }
    
    /// <summary>
    /// Unreliable message base class (for frequent updates)
    /// </summary>
    [Serializable]
    public abstract class UnreliableMessage : INetworkMessage
    {
        public abstract MessageType Type { get; }
        public bool IsReliable => false;
        public int SequenceNumber { get; set; }
        public long Timestamp { get; set; }
    }
}
