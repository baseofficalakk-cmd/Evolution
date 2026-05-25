using System;

namespace EVOLUTION.Shared.Interfaces
{
    /// <summary>
    /// Base interface for all network messages
    /// </summary>
    public interface INetworkMessage
    {
        string MessageId { get; set; }
        MessageType Type { get; }
        long Timestamp { get; set; }
    }

    /// <summary>
    /// Message types for networking
    /// </summary>
    public enum MessageType
    {
        // Connection
        ConnectRequest = 0,
        ConnectResponse = 1,
        Disconnect = 2,
        Heartbeat = 3,
        
        // Authentication
        LoginRequest = 4,
        LoginResponse = 5,
        RegisterRequest = 6,
        RegisterResponse = 7,
        Logout = 8,
        
        // Matchmaking
        QueueRequest = 9,
        QueueResponse = 10,
        MatchFound = 11,
        MatchCancelled = 12,
        
        // Match control
        MatchStart = 13,
        MatchEnd = 14,
        PlayerJoined = 15,
        PlayerLeft = 16,
        
        // Gameplay
        PlayerInput = 17,
        PlayerState = 18,
        ProjectileSpawn = 19,
        ProjectileHit = 20,
        AbilityCast = 21,
        DamageDealt = 22,
        PlayerDeath = 23,
        PlayerRespawn = 24,
        
        // Chat
        ChatMessage = 25,
        PingMessage = 26,
        
        // Progression
        XPGranted = 27,
        LevelUp = 28,
        QuestProgress = 29,
        QuestCompleted = 30,
        
        // Social
        FriendRequest = 31,
        FriendResponse = 32,
        ClanInvite = 33,
        ClanJoin = 34,
        
        // Economy
        PurchaseRequest = 35,
        PurchaseResponse = 36,
        ChestOpen = 37,
        ChestResult = 38,
        
        // Admin
        AdminCommand = 39,
        AdminResponse = 40,
        
        // Replay
        ReplayStart = 41,
        ReplayData = 42,
        ReplayEnd = 43,
        
        // Spectator
        SpectatorJoin = 44,
        SpectatorLeave = 45,
        
        // Ban phase
        BanPhaseStart = 46,
        HeroBan = 47,
        BanPhaseEnd = 48,
        
        // Hero selection
        HeroSelectRequest = 49,
        HeroSelectResponse = 50,
        HeroLockIn = 51,
        
        // Vote
        SurrenderVote = 52,
        VoteResult = 53,
        
        // AFK
        AFKWarning = 54,
        AFKPunishment = 55,
        
        // Seasonal
        SeasonStart = 56,
        SeasonEnd = 57,
        RankUpdate = 58,
        
        // Battle Pass
        BattlePassUpdate = 59,
        BattlePassReward = 60,
        
        // Tournament
        TournamentRegister = 61,
        TournamentBracket = 62,
        TournamentMatchStart = 63,
        
        // Error
        ErrorMessage = 100
    }

    /// <summary>
    /// Reliable message delivery guarantee
    /// </summary>
    public interface IReliableMessage : INetworkMessage
    {
        int SequenceNumber { get; set; }
        bool RequiresAcknowledgement { get; }
    }

    /// <summary>
    /// Unreliable fast message (UDP style)
    /// </summary>
    public interface IUnreliableMessage : INetworkMessage
    {
        bool CanBeDropped { get; }
    }

    /// <summary>
    /// Message that needs interpolation
    /// </summary>
    public interface IInterpolatedMessage : INetworkMessage
    {
        float InterpolationTime { get; }
        Vector3Float Position { get; }
        Vector3Float Velocity { get; }
    }

    /// <summary>
    /// Message with prediction support
    /// </summary>
    public interface IPredictedMessage : INetworkMessage
    {
        int ClientPredictionId { get; set; }
        bool WasCorrected { get; set; }
    }

    /// <summary>
    /// Network client interface
    /// </summary>
    public interface INetworkClient
    {
        void Connect(string host, int port);
        void Disconnect();
        void Send<T>(T message) where T : INetworkMessage;
        event Action<INetworkMessage> OnMessageReceived;
        event Action OnConnected;
        event Action OnDisconnected;
        int Ping { get; }
        bool IsConnected { get; }
    }

    /// <summary>
    /// Network server interface
    /// </summary>
    public interface INetworkServer
    {
        void Start(int port);
        void Stop();
        void Broadcast<T>(T message) where T : INetworkMessage;
        void SendTo<T>(string clientId, T message) where T : INetworkMessage;
        event Action<string, INetworkMessage> OnMessageReceived;
        event Action<string> OnClientConnected;
        event Action<string> OnClientDisconnected;
        int ConnectedClientCount { get; }
    }

    /// <summary>
    /// Serializable message base class
    /// </summary>
    [Serializable]
    public abstract class NetworkMessageBase : INetworkMessage
    {
        public string MessageId { get; set; }
        public abstract MessageType Type { get; }
        public long Timestamp { get; set; }
        
        protected NetworkMessageBase()
        {
            MessageId = Guid.NewGuid().ToString();
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
