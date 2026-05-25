using System;

namespace EVOLUTION.Shared.Models
{
    /// <summary>
    /// Hero data model with all progression and stats
    /// </summary>
    [Serializable]
    public class HeroData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Lore { get; set; }
        public HeroRarity Rarity { get; set; }
        
        // Stats
        public int Health { get; set; }
        public int Speed { get; set; }
        public int Damage { get; set; }
        public float AttackRange { get; set; }
        public float AttackSpeed { get; set; }
        
        // Abilities
        public AbilityData BasicAbility { get; set; }
        public AbilityData SpecialAbility { get; set; }
        public AbilityData UltimateAbility { get; set; }
        public AbilityData PassiveAbility { get; set; }
        
        // Appearance
        public string PortraitPath { get; set; }
        public string IconPath { get; set; }
        public string ModelPath { get; set; }
        public ColorScheme ColorScheme { get; set; }
        
        // Skins
        public SkinData[] AvailableSkins { get; set; }
        
        // Unlock requirements
        public int UnlockCost { get; set; }
        public CurrencyType UnlockCurrency { get; set; }
        public string UnlockRequirement { get; set; } // e.g., "TitanTrial" for unique
        
        // AI behavior config
        public AIHeroConfig AIConfig { get; set; }
        
        // Tips
        public string[] GameplayTips { get; set; }
        
        public HeroData()
        {
            Id = Guid.NewGuid().ToString();
            AvailableSkins = Array.Empty<SkinData>();
            GameplayTips = Array.Empty<string>();
        }
    }

    /// <summary>
    /// Ability data model
    /// </summary>
    [Serializable]
    public class AbilityData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AbilityType Type { get; set; }
        
        // Combat values
        public int Damage { get; set; }
        public float Cooldown { get; set; }
        public float CastRange { get; set; }
        public float CastTime { get; set; }
        public int EnergyCost { get; set; }
        
        // Effects
        public EffectType[] AppliedEffects { get; set; }
        public float EffectDuration { get; set; }
        
        // Visuals
        public string VFXPath { get; set; }
        public string SFXPath { get; set; }
        public string AnimationTrigger { get; set; }
        
        // Projectile info
        public ProjectileType ProjectileType { get; set; }
        public float ProjectileSpeed { get; set; }
        public float ProjectileLifetime { get; set; }
        
        // AoE info
        public float AoERadius { get; set; }
        
        public AbilityData()
        {
            Id = Guid.NewGuid().ToString();
            AppliedEffects = Array.Empty<EffectType>();
        }
    }

    /// <summary>
    /// Skin data model
    /// </summary>
    [Serializable]
    public class SkinData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HeroId { get; set; }
        public string ModelPath { get; set; }
        public string PortraitPath { get; set; }
        public string IconPath { get; set; }
        public string VFXOverride { get; set; }
        public string SFXOverride { get; set; }
        public int Price { get; set; }
        public CurrencyType PriceCurrency { get; set; }
        public bool IsExclusive { get; set; }
        public string AcquisitionMethod { get; set; } // "BattlePass", "Event", "Shop"
    }

    /// <summary>
    /// Color scheme for hero styling
    /// </summary>
    [Serializable]
    public class ColorScheme
    {
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string AccentColor { get; set; }
        public string OutlineColor { get; set; }
    }

    /// <summary>
    /// AI configuration for hero bots
    /// </summary>
    [Serializable]
    public class AIHeroConfig
    {
        public float AggressionLevel { get; set; } // 0-1
        public float RetreatHPThreshold { get; set; } // 0-1
        public float BushUsagePreference { get; set; } // 0-1
        public float UltimateUsagePriority { get; set; } // 0-1
        public bool PrefersObjectives { get; set; }
        public float ReactionTime { get; set; } // seconds
        public float AccuracyBase { get; set; } // 0-1
        public float DodgeChance { get; set; } // 0-1
        public float TeamPlayScore { get; set; } // 0-1
        
        public AIHeroConfig()
        {
            AggressionLevel = 0.5f;
            RetreatHPThreshold = 0.3f;
            BushUsagePreference = 0.5f;
            UltimateUsagePriority = 0.7f;
            PrefersObjectives = true;
            ReactionTime = 0.2f;
            AccuracyBase = 0.8f;
            DodgeChance = 0.3f;
            TeamPlayScore = 0.6f;
        }
    }

    /// <summary>
    /// Match statistics for a single hero in a match
    /// </summary>
    [Serializable]
    public class HeroMatchStats
    {
        public string HeroId { get; set; }
        public string PlayerId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int DamageDealt { get; set; }
        public int HealingDone { get; set; }
        public int ObjectivesCompleted { get; set; }
        public float SurvivalTime { get; set; } // seconds
        public int UltimatesUsed { get; set; }
        public int AccuracyPercentage { get; set; }
        public float MVP_Score { get; set; }
        public bool IsMVP { get; set; }
    }
}
