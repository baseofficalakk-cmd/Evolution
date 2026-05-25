using System;

namespace EVOLUTION.Shared.Models
{
    /// <summary>
    /// Base player data model shared between client and server
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        
        // Progression
        public int ProfileLevel { get; set; }
        public int TotalTrophies { get; set; }
        public RankTier CurrentRank { get; set; }
        public int CurrentTrophiesInRank { get; set; }
        
        // Currencies
        public int Cells { get; set; }
        public int Gems { get; set; }
        
        // Statistics
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalDamageDealt { get; set; }
        public int TotalHealingDone { get; set; }
        
        // Social
        public string ClanId { get; set; }
        public string[] FriendIds { get; set; }
        
        // Customization
        public string FavoriteHeroId { get; set; }
        public string SelectedTitleId { get; set; }
        public string SelectedFrameId { get; set; }
        
        // Owned content
        public string[] OwnedHeroIds { get; set; }
        public string[] OwnedSkinIds { get; set; }
        public string[] OwnedEmoteIds { get; set; }
        public string[] OwnedSprayIds { get; set; }
        
        // Hero mastery (heroId -> level)
        public Dictionary<string, int> HeroLevels { get; set; }
        public Dictionary<string, int> HeroTrophies { get; set; }
        public Dictionary<string, int> HeroXP { get; set; }
        
        // Battle Pass
        public int BattlePassLevel { get; set; }
        public bool HasPremiumBattlePass { get; set; }
        public int BattlePassXP { get; set; }
        
        // Seasonal
        public int CurrentSeason { get; set; }
        public int SeasonHighestRank { get; set; }
        
        // Quests
        public QuestData[] DailyQuests { get; set; }
        public QuestData[] WeeklyQuests { get; set; }
        public DateTime LastDailyReset { get; set; }
        public DateTime LastWeeklyReset { get; set; }
        
        // Login rewards
        public int ConsecutiveLoginDays { get; set; }
        public DateTime LastLoginDate { get; set; }
        
        // Settings
        public PlayerSettings Settings { get; set; }
        
        // Tutorial progress
        public bool CompletedMovementTutorial { get; set; }
        public bool CompletedUltimateTutorial { get; set; }
        public bool CompletedBushesTutorial { get; set; }
        
        // AFK tracking
        public int AFKWarnings { get; set; }
        public DateTime? TemporaryBanUntil { get; set; }
        
        public PlayerData()
        {
            Id = Guid.NewGuid().ToString();
            Role = UserRole.Guest;
            HeroLevels = new Dictionary<string, int>();
            HeroTrophies = new Dictionary<string, int>();
            HeroXP = new Dictionary<string, int>();
            FriendIds = Array.Empty<string>();
            OwnedHeroIds = Array.Empty<string>();
            OwnedSkinIds = Array.Empty<string>();
            OwnedEmoteIds = Array.Empty<string>();
            OwnedSprayIds = Array.Empty<string>();
            Settings = new PlayerSettings();
        }
    }

    /// <summary>
    /// Player settings
    /// </summary>
    [Serializable]
    public class PlayerSettings
    {
        public float MusicVolume { get; set; }
        public float SFXVolume { get; set; }
        public float VoiceVolume { get; set; }
        public bool Fullscreen { get; set; }
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
        public bool VSync { get; set; }
        public bool ShadowsEnabled { get; set; }
        public bool ParticlesEnabled { get; set; }
        public string Language { get; set; }
        
        public PlayerSettings()
        {
            MusicVolume = AudioConstants.DEFAULT_MUSIC_VOLUME;
            SFXVolume = AudioConstants.DEFAULT_SFX_VOLUME;
            VoiceVolume = AudioConstants.DEFAULT_VOICE_VOLUME;
            Fullscreen = GraphicsQuality.DEFAULT_FULLSCREEN;
            ResolutionWidth = 1280;
            ResolutionHeight = 720;
            VSync = GraphicsQuality.DEFAULT_VSYNC;
            ShadowsEnabled = GraphicsQuality.DEFAULT_SHADOWS;
            ParticlesEnabled = GraphicsQuality.DEFAULT_PARTICLES;
            Language = "en";
        }
    }

    /// <summary>
    /// Quest data model
    /// </summary>
    [Serializable]
    public class QuestData
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public QuestType Type { get; set; }
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCollected { get; set; }
        public RewardData[] Rewards { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    /// <summary>
    /// Quest types
    /// </summary>
    public enum QuestType
    {
        WinMatches = 0,
        DealDamage = 1,
        HealAllies = 2,
        GetKills = 3,
        PlayMatches = 4,
        OpenChests = 5,
        UseUltimate = 6
    }

    /// <summary>
    /// Reward data model
    /// </summary>
    [Serializable]
    public class RewardData
    {
        public RewardType Type { get; set; }
        public int Amount { get; set; }
        public string ItemId { get; set; }
    }

    /// <summary>
    /// Reward types
    /// </summary>
    public enum RewardType
    {
        Cells = 0,
        Gems = 1,
        Hero = 2,
        Skin = 3,
        Chest = 4,
        Emote = 5,
        Spray = 6,
        Title = 7,
        Frame = 8,
        BattlePassXP = 9
    }

    /// <summary>
    /// Login reward day
    /// </summary>
    [Serializable]
    public class LoginRewardDay
    {
        public int DayNumber { get; set; }
        public RewardData[] Rewards { get; set; }
        public bool IsClaimed { get; set; }
    }
}
