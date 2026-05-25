using UnityEngine;
using Shared.Enums;
using Shared.Models;

namespace EVOLUTION.Gameplay.Heroes
{
    /// <summary>
    /// Base Hero class for all playable characters
    /// Manages health, abilities, stats, and state synchronization
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(CombatSystem))]
    public class Hero : MonoBehaviour
    {
        [Header("Hero Settings")]
        [SerializeField] private HeroData _heroData;
        [SerializeField] private int teamId = -1;
        
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 1000f;
        [SerializeField] private float currentHealth;
        [SerializeField] private float healthRegen = 0f;
        [SerializeField] private float regenDelay = 5f;
        
        [Header("Visual Settings")]
        [SerializeField] private GameObject[] skinVariants;
        [SerializeField] private Material defaultMaterial;
        
        // Components
        private PlayerController _playerController;
        private CombatSystem _combatSystem;
        
        // State
        private float _lastDamageTime;
        private bool _isDead;
        private bool _isInitialized;
        
        // Events
        public event System.Action<Hero, float, float> OnHealthChanged;
        public event System.Action<Hero> OnHeroDied;
        public event System.Action<Hero> OnHeroRespawned;
        public event System.ActionEvent<Hero, HeroData> OnHeroSpawned;
        
        public HeroData HeroData => _heroData;
        public bool IsDead => _isDead;
        public float HealthPercent => currentHealth / maxHealth;
        
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _combatSystem = GetComponent<CombatSystem>();
            
            if (_playerController == null)
            {
                _playerController = gameObject.AddComponent<PlayerController>();
            }
            
            if (_combatSystem == null)
            {
                _combatSystem = gameObject.AddComponent<CombatSystem>();
            }
        }
        
        private void Start()
        {
            Initialize();
        }
        
        private void Update()
        {
            if (!_isInitialized || _isDead) return;
            
            HandleHealthRegen();
        }
        
        #region Initialization
        
        private void Initialize()
        {
            currentHealth = maxHealth;
            _isDead = false;
            _isInitialized = true;
            
            // Link hero data to components
            if (_heroData != null)
            {
                _heroData.TeamId = teamId;
                _heroData.OwnerGameObject = gameObject;
                _playerController.SetHeroData(_heroData);
                _combatSystem.SetHeroData(_heroData);
                
                // Apply skin variant
                ApplySkinVariant(_heroData.CurrentSkinIndex);
            }
            
            OnHeroSpawned?.Invoke(this, _heroData);
        }
        
        public void SetHeroData(HeroData data)
        {
            _heroData = data;
            if (_isInitialized && data != null)
            {
                maxHealth = data.BaseStats.MaxHealth;
                currentHealth = maxHealth;
                healthRegen = data.BaseStats.HealthRegen;
                teamId = data.TeamId;
                
                _playerController.SetHeroData(data);
                _combatSystem.SetHeroData(data);
            }
        }
        
        #endregion
        
        #region Health & Damage
        
        public void TakeDamage(float damage, HeroData attacker, DamageType damageType, ProjectileType projectileType)
        {
            if (_isDead || !_isInitialized) return;
            
            _lastDamageTime = Time.time;
            
            // Apply damage modifiers
            float finalDamage = ApplyDamageModifiers(damage, damageType, attacker);
            
            currentHealth -= finalDamage;
            OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
            
            // Check for death
            if (currentHealth <= 0f)
            {
                Die(attacker);
            }
        }
        
        private float ApplyDamageModifiers(float baseDamage, DamageType damageType, HeroData attacker)
        {
            float finalDamage = baseDamage;
            
            // Critical hit multiplier
            if (damageType == DamageType.Critical)
            {
                finalDamage *= 1.5f;
            }
            
            // Armor reduction (simplified)
            if (_heroData != null)
            {
                float armor = _heroData.BaseStats.Armor;
                finalDamage *= (100f / (100f + armor));
            }
            
            return finalDamage;
        }
        
        public void Heal(float amount)
        {
            if (_isDead || !_isInitialized) return;
            
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
        }
        
        private void HandleHealthRegen()
        {
            if (healthRegen <= 0f) return;
            if (currentHealth >= maxHealth) return;
            if (Time.time - _lastDamageTime < regenDelay) return;
            
            currentHealth += healthRegen * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            
            OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
        }
        
        private void Die(HeroData killer)
        {
            _isDead = true;
            _playerController.StopMovement();
            _combatSystem.ResetCooldowns();
            
            // Update statistics
            if (_heroData != null)
            {
                _heroData.Matches.LastMatchDeaths++;
            }
            
            // Update killer statistics
            if (killer != null)
            {
                killer.Matches.LastMatchKills++;
            }
            
            OnHeroDied?.Invoke(this);
            
            // Notify match manager for respawn logic
            MatchManager.Instance?.HandleHeroDeath(this, killer);
        }
        
        public void Respawn(Vector3 spawnPosition)
        {
            if (!_isDead) return;
            
            transform.position = spawnPosition;
            currentHealth = maxHealth;
            _isDead = false;
            _isInitialized = true;
            
            _combatSystem.ResetCooldowns();
            
            OnHeroRespawned?.Invoke(this);
        }
        
        #endregion
        
        #region Abilities
        
        public void UseAbility(AbilityType abilityType)
        {
            if (_isDead || !_isInitialized) return;
            _combatSystem.UseAbility(abilityType);
        }
        
        public bool IsAbilityReady(AbilityType abilityType)
        {
            return _combatSystem.IsAbilityReady(abilityType);
        }
        
        public float GetRemainingCooldown(AbilityType abilityType)
        {
            return _combatSystem.GetRemainingCooldown(abilityType);
        }
        
        #endregion
        
        #region Visuals
        
        private void ApplySkinVariant(int skinIndex)
        {
            if (skinVariants == null || skinVariants.Length == 0) return;
            
            // Disable all variants
            foreach (var variant in skinVariants)
            {
                if (variant != null)
                    variant.SetActive(false);
            }
            
            // Enable selected variant
            if (skinIndex >= 0 && skinIndex < skinVariants.Length)
            {
                skinVariants[skinIndex].SetActive(true);
            }
        }
        
        public void ChangeSkin(int skinIndex)
        {
            if (_heroData != null)
            {
                _heroData.CurrentSkinIndex = skinIndex;
                ApplySkinVariant(skinIndex);
            }
        }
        
        #endregion
        
        #region Getters
        
        public PlayerController GetPlayerController() => _playerController;
        public CombatSystem GetCombatSystem() => _combatSystem;
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
        public int GetTeamId() => teamId;
        
        #endregion
    }
}
