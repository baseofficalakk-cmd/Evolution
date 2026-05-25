using System;

namespace EVOLUTION.Shared.Enums
{
    /// <summary>
    /// Hero rarity levels
    /// </summary>
    public enum HeroRarity
    {
        Starter = 0,
        Rare = 1,
        Epic = 2,
        Legendary = 3,
        Unique = 4
    }

    /// <summary>
    /// Game modes available
    /// </summary>
    public enum GameMode
    {
        BattleRoyale = 0,
        CaptureTheFlag = 1,
        Siege = 2,
        TitanTrial = 3
    }

    /// <summary>
    /// Match state machine states
    /// </summary>
    public enum MatchState
    {
        WaitingForPlayers = 0,
        BanPhase = 1,
        HeroSelection = 2,
        Loading = 3,
        InProgress = 4,
        Finished = 5
    }

    /// <summary>
    /// Player team assignment
    /// </summary>
    public enum TeamId
    {
        None = 0,
        Blue = 1,
        Red = 2
    }

    /// <summary>
    /// Ability types
    /// </summary>
    public enum AbilityType
    {
        Basic = 0,
        Special = 1,
        Ultimate = 2,
        Passive = 3
    }

    /// <summary>
    /// Projectile types for networking
    /// </summary>
    public enum ProjectileType
    {
        Bullet = 0,
        Missile = 1,
        Beam = 2,
        AoE = 3,
        Healing = 4
    }

    /// <summary>
    /// Chat message types
    /// </summary>
    public enum ChatType
    {
        Global = 0,
        Team = 1,
        Clan = 2,
        Private = 3,
        System = 4
    }

    /// <summary>
    /// Ping types for tactical communication
    /// </summary>
    public enum PingType
    {
        Attack = 0,
        Defend = 1,
        EnemySpotted = 2,
        NeedHelp = 3,
        Retreat = 4,
        UltimateReady = 5,
        FollowMe = 6,
        Danger = 7
    }

    /// <summary>
    /// Account roles for permissions
    /// </summary>
    public enum UserRole
    {
        Guest = 0,
        Player = 1,
        Moderator = 2,
        Admin = 3,
        Owner = 4
    }

    /// <summary>
    /// Currency types
    /// </summary>
    public enum CurrencyType
    {
        Cells = 0,
        Gems = 1
    }

    /// <summary>
    /// Chest types
    /// </summary>
    public enum ChestType
    {
        Standard = 0,
        Golden = 1
    }

    /// <summary>
    /// Season rank tiers
    /// </summary>
    public enum RankTier
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2,
        Platinum = 3,
        Diamond = 4,
        Master = 5,
        Grandmaster = 6
    }

    /// <summary>
    /// Input action types
    /// </summary>
    public enum InputAction
    {
        Move = 0,
        Aim = 1,
        Attack = 2,
        ManualAim = 3,
        Ultimate = 4,
        Ping = 5,
        Chat = 6
    }

    /// <summary>
    /// Damage types for calculations
    /// </summary>
    public enum DamageType
    {
        Physical = 0,
        Energy = 1,
        Fire = 2,
        Poison = 3,
        True = 4
    }

    /// <summary>
    /// Effect types for buffs/debuffs
    /// </summary>
    public enum EffectType
    {
        SpeedBoost = 0,
        DamageBoost = 1,
        Shield = 2,
        HealOverTime = 3,
        Slow = 4,
        Stun = 5,
        Silence = 6,
        Poison = 7,
        Burn = 8,
        Invisible = 9
    }
}
