using System;

namespace EVOLUTION.Shared.Constants
{
    /// <summary>
    /// Global game constants
    /// </summary>
    public static class GameConstants
    {
        // Map dimensions
        public const float MAP_WIDTH = 2400f;
        public const float MAP_HEIGHT = 1600f;
        
        // Performance limits
        public const int MAX_ACTIVE_OBJECTS = 300;
        public const float FIXED_TIMESTEP = 0.0166667f; // 60 FPS
        public const int TICK_RATE = 60;
        
        // Respawn settings
        public const float RESPAWN_TIME = 10f;
        public const float SPAWN_PROTECTION_TIME = 2f;
        
        // Experience system
        public const int MAX_HERO_LEVEL = 100;
        public const int MAX_HERO_XP = 30000;
        public const int XP_PER_MATCH = 30;
        
        // Match settings
        public const int BATTLE_ROYALE_PLAYERS = 12;
        public const int TITAN_TRIAL_PLAYERS = 5;
        
        // Network settings
        public const int SERVER_TICK_RATE = 60;
        public const float CLIENT_PREDICTION_TIME = 0.1f;
        public const float INTERPOLATION_TIME = 0.1f;
        
        // Currency defaults
        public const int STARTER_CELLS = 0;
        public const int STARTER_GEMS = 100;
        
        // AFK detection
        public const float AFK_DETECTION_TIME = 30f;
        
        // Tutorial rewards
        public const int TUTORIAL_REWARD_CELLS = 500;
        public const int TUTORIAL_REWARD_GEMS = 50;
        
        // Daily quest limits
        public const int DAILY_QUESTS_COUNT = 3;
        public const int WEEKLY_QUESTS_COUNT = 5;
        
        // Login reward days
        public const int LOGIN_REWARD_DAYS = 30;
        
        // Chat message max length
        public const int MAX_CHAT_MESSAGE_LENGTH = 200;
        
        // Ban phase duration (seconds)
        public const int BAN_PHASE_DURATION = 30;
        
        // Hero selection duration (seconds)
        public const int HERO_SELECTION_DURATION = 45;
        
        // Minimum players to start match
        public const int MIN_PLAYERS_TO_START = 2;
        
        // Shrinking zone settings (Battle Royale)
        public const float ZONE_SHRINK_SPEED = 1f;
        public const float ZONE_DAMAGE_PER_SECOND = 10f;
        
        // Max friends per player
        public const int MAX_FRIENDS = 100;
        
        // Max clan members
        public const int MAX_CLAN_MEMBERS = 50;
        
        // Replay recording FPS
        public const int REPLAY_FPS = 60;
        
        // Max replay duration in seconds
        public const int MAX_REPLAY_DURATION = 600;
    }

    /// <summary>
    /// Hero pricing constants
    /// </summary>
    public static class HeroPricing
    {
        public const int RARE_PRICE = 1000;
        public const int EPIC_PRICE = 2500;
        public const int LEGENDARY_PRICE = 5500;
    }

    /// <summary>
    /// Chest system constants
    /// </summary>
    public static class ChestConstants
    {
        // Standard Chest
        public const int STANDARD_CHEST_COST = 50;
        public const int STANDARD_CHEST_MIN_ITEMS = 3;
        public const int STANDARD_CHEST_MAX_ITEMS = 12;
        
        // Golden Chest
        public const int GOLDEN_CHEST_COST = 200;
        public const int GOLDEN_CHEST_MIN_ITEMS = 7;
        public const int GOLDEN_CHEST_MAX_ITEMS = 25;
        
        // Drop rates (Standard)
        public const float STANDARD_RARE_RATE = 0.50f;
        public const float STANDARD_EPIC_RATE = 0.30f;
        public const float STANDARD_LEGENDARY_RATE = 0.05f;
        public const float STANDARD_UNIQUE_RATE = 0.005f;
        
        // Drop rates (Golden)
        public const float GOLDEN_RARE_RATE = 0.75f;
        public const float GOLDEN_EPIC_RATE = 0.50f;
        public const float GOLDEN_LEGENDARY_RATE = 0.075f;
        public const float GOLDEN_UNIQUE_RATE = 0.03f;
    }

    /// <summary>
    /// Trophy system constants
    /// </summary>
    public static class TrophyConstants
    {
        public const int BASE_TROPHY_GAIN = 20;
        public const int BASE_TROPHY_LOSS = 15;
        public const int SEASON_RESET_RETAINED_PERCENT = 50;
    }

    /// <summary>
    /// Rank thresholds
    /// </summary>
    public static class RankThresholds
    {
        public const int BRONZE_THRESHOLD = 0;
        public const int SILVER_THRESHOLD = 500;
        public const int GOLD_THRESHOLD = 1500;
        public const int PLATINUM_THRESHOLD = 3000;
        public const int DIAMOND_THRESHOLD = 5000;
        public const int MASTER_THRESHOLD = 8000;
        public const int GRANDMASTER_THRESHOLD = 12000;
    }

    /// <summary>
    /// Audio constants
    /// </summary>
    public static class AudioConstants
    {
        public const float DEFAULT_MUSIC_VOLUME = 0.5f;
        public const float DEFAULT_SFX_VOLUME = 0.8f;
        public const float DEFAULT_VOICE_VOLUME = 1.0f;
    }

    /// <summary>
    /// Graphics quality presets
    /// </summary>
    public static class GraphicsQuality
    {
        public const bool DEFAULT_FULLSCREEN = true;
        public const bool DEFAULT_VSYNC = true;
        public const bool DEFAULT_SHADOWS = true;
        public const bool DEFAULT_PARTICLES = true;
    }
}
