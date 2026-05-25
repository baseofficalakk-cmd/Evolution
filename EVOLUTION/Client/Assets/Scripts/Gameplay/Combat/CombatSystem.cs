using UnityEngine;
using Shared.Enums;
using Shared.Models;
using EVOLUTION.Core.Input;
using EVOLUTION.Gameplay.Projectiles;
using System.Collections.Generic;

namespace EVOLUTION.Gameplay.Combat
{
    /// <summary>
    /// Combat System handling attacks, aiming, cooldowns, and ability execution
    /// Supports auto-attack and manual aim mechanics like Brawl Stars
    /// </summary>
    public class CombatSystem : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float manualAimRange = 12f;
        
        [Header("Aiming")]
        [SerializeField] private GameObject aimLinePrefab;
        [SerializeField] private Color autoAimColor = Color.yellow;
        [SerializeField] private Color manualAimColor = Color.red;
        
        // Components
        private InputManager _inputManager;
        private PlayerController _playerController;
        private HeroData _heroData;
        private LineRenderer _aimLine;
        
        // State
        private float _lastAttackTime;
        private bool _canAttack = true;
        private Vector2 _currentAimDirection;
        private AbilityType _currentAbilityType;
        
        // Cooldown tracking
        private Dictionary<AbilityType, float> _abilityCooldowns = new();
        private Dictionary<AbilityType, float> _abilityCooldownDurations = new();
        
        // Events
        public event System.Action<AbilityType> OnAbilityUsed;
        public event System.ActionEvent<float, AbilityType> OnCooldownUpdated;
        public event System.Action<Vector2> OnAimDirectionChanged;
        
        private void Awake()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _playerController = GetComponent<PlayerController>();
            
            if (_inputManager == null)
            {
                Debug.LogError("InputManager not found!");
                return;
            }
            
            // Subscribe to input events
            _inputManager.OnAttackPressed += HandleAttackPressed;
            _inputManager.OnAimStarted += HandleAimStarted;
            _inputManager.OnAimEnded += HandleAimEnded;
            _inputManager.OnSuperPressed += HandleSuperPressed;
        }
        
        private void Start()
        {
            InitializeAimLine();
            InitializeAbilityCooldowns();
        }
        
        private void OnDestroy()
        {
            if (_inputManager != null)
            {
                _inputManager.OnAttackPressed -= HandleAttackPressed;
                _inputManager.OnAimStarted -= HandleAimStarted;
                _inputManager.OnAimEnded -= HandleAimEnded;
                _inputManager.OnSuperPressed -= HandleSuperPressed;
            }
        }
        
        private void Update()
        {
            if (!_canAttack)
            {
                UpdateAttackCooldown();
            }
            
            UpdateAbilityCooldowns();
            UpdateAimLine();
        }
        
        #region Initialization
        
        private void InitializeAimLine()
        {
            if (aimLinePrefab != null)
            {
                _aimLine = Instantiate(aimLinePrefab, transform).GetComponent<LineRenderer>();
            }
            else
            {
                // Create aim line programmatically
                GameObject aimObj = new GameObject("AimLine");
                aimObj.transform.SetParent(transform);
                _aimLine = aimObj.AddComponent<LineRenderer>();
                _aimLine.positionCount = 2;
                _aimLine.startWidth = 0.2f;
                _aimLine.endWidth = 0.1f;
                _aimLine.material = new Material(Shader.Find("Sprites/Default"));
                _aimLine.startColor = autoAimColor;
                _aimLine.endColor = autoAimColor;
            }
            
            _aimLine.enabled = false;
        }
        
        private void InitializeAbilityCooldowns()
        {
            // Initialize all ability types with default cooldowns
            foreach (AbilityType ability in System.Enum.GetValues(typeof(AbilityType)))
            {
                if (ability != AbilityType.None)
                {
                    _abilityCooldowns[ability] = 0f;
                    _abilityCooldownDurations[ability] = GetDefaultCooldown(ability);
                }
            }
        }
        
        private float GetDefaultCooldown(AbilityType ability)
        {
            return ability switch
            {
                AbilityType.Basic => attackCooldown,
                AbilityType.Skill1 => 8f,
                AbilityType.Skill2 => 12f,
                AbilityType.Ultimate => 30f,
                _ => 1f
            };
        }
        
        #endregion
        
        #region Input Handlers
        
        private void HandleAttackPressed()
        {
            if (_canAttack)
            {
                PerformAttack();
            }
        }
        
        private void HandleAimStarted()
        {
            _aimLine.enabled = true;
            _aimLine.startColor = manualAimColor;
            _aimLine.endColor = manualAimColor;
        }
        
        private void HandleAimEnded()
        {
            _aimLine.enabled = false;
        }
        
        private void HandleSuperPressed()
        {
            if (IsAbilityReady(AbilityType.Ultimate))
            {
                UseAbility(AbilityType.Ultimate);
            }
        }
        
        #endregion
        
        #region Combat Logic
        
        private void PerformAttack()
        {
            bool isManualAiming = _inputManager.IsAiming();
            Vector2 attackDirection = CalculateAttackDirection(isManualAiming);
            
            if (attackDirection.magnitude < 0.1f) return;
            
            _lastAttackTime = Time.time;
            _canAttack = false;
            
            // Determine projectile type based on hero and aim mode
            ProjectileType projType = isManualAiming ? ProjectileType.ManualAim : ProjectileType.AutoAim;
            
            FireProjectile(attackDirection, AbilityType.Basic, projType);
            
            OnAbilityUsed?.Invoke(AbilityType.Basic);
        }
        
        private Vector2 CalculateAttackDirection(bool manualAiming)
        {
            if (manualAiming)
            {
                // Manual aim: use mouse position directly
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                Vector2 direction = (Vector2)mousePos - (Vector2)transform.position;
                return direction.normalized;
            }
            else
            {
                // Auto aim: find nearest enemy
                Vector2 autoAimDir = FindNearestEnemyDirection();
                return autoAimDir.magnitude > 0.1f ? autoAimDir : transform.up;
            }
        }
        
        private Vector2 FindNearestEnemyDirection()
        {
            // This would integrate with the networking/AI system
            // For now, return forward direction
            return transform.up;
        }
        
        private void FireProjectile(Vector2 direction, AbilityType abilityType, ProjectileType projectileType)
        {
            Projectile projectile = Projectile.GetFromPool();
            
            ProjectileConfig config = new ProjectileConfig
            {
                Speed = 15f,
                Lifetime = 3f,
                BaseDamage = _heroData?.BaseStats.AttackDamage ?? 100f,
                Type = projectileType,
                DamageType = DamageType.Normal
            };
            
            projectile.Initialize(transform.position, direction, _heroData, config);
        }
        
        public void UseAbility(AbilityType abilityType)
        {
            if (!IsAbilityReady(abilityType)) return;
            
            StartAbilityCooldown(abilityType);
            OnAbilityUsed?.Invoke(abilityType);
            
            // Execute ability logic based on type
            switch (abilityType)
            {
                case AbilityType.Skill1:
                    ExecuteSkill1();
                    break;
                case AbilityType.Skill2:
                    ExecuteSkill2();
                    break;
                case AbilityType.Ultimate:
                    ExecuteUltimate();
                    break;
            }
        }
        
        private void ExecuteSkill1()
        {
            // Override in specific hero implementations
            Debug.Log("Skill 1 executed");
        }
        
        private void ExecuteSkill2()
        {
            // Override in specific hero implementations
            Debug.Log("Skill 2 executed");
        }
        
        private void ExecuteUltimate()
        {
            // Override in specific hero implementations
            Debug.Log("Ultimate executed");
        }
        
        #endregion
        
        #region Cooldown Management
        
        private void UpdateAttackCooldown()
        {
            float elapsed = Time.time - _lastAttackTime;
            if (elapsed >= attackCooldown)
            {
                _canAttack = true;
            }
            else
            {
                float remaining = attackCooldown - elapsed;
                OnCooldownUpdated?.Invoke(remaining / attackCooldown, AbilityType.Basic);
            }
        }
        
        private void UpdateAbilityCooldowns()
        {
            var abilitiesToRemove = new List<AbilityType>();
            
            foreach (var kvp in _abilityCooldowns)
            {
                if (kvp.Value > 0f)
                {
                    _abilityCooldowns[kvp.Key] = kvp.Value - Time.deltaTime;
                    
                    if (_abilityCooldowns[kvp.Key] <= 0f)
                    {
                        abilitiesToRemove.Add(kvp.Key);
                    }
                    else
                    {
                        float ratio = _abilityCooldowns[kvp.Key] / _abilityCooldownDurations[kvp.Key];
                        OnCooldownUpdated?.Invoke(ratio, kvp.Key);
                    }
                }
            }
            
            foreach (var ability in abilitiesToRemove)
            {
                _abilityCooldowns[ability] = 0f;
            }
        }
        
        private void StartAbilityCooldown(AbilityType abilityType)
        {
            if (!_abilityCooldownDurations.ContainsKey(abilityType))
            {
                _abilityCooldownDurations[abilityType] = GetDefaultCooldown(abilityType);
            }
            
            _abilityCooldowns[abilityType] = _abilityCooldownDurations[abilityType];
        }
        
        public bool IsAbilityReady(AbilityType abilityType)
        {
            return !_abilityCooldowns.ContainsKey(abilityType) || _abilityCooldowns[abilityType] <= 0f;
        }
        
        public float GetRemainingCooldown(AbilityType abilityType)
        {
            return _abilityCooldowns.TryGetValue(abilityType, out float remaining) ? remaining : 0f;
        }
        
        public void SetCooldownDuration(AbilityType abilityType, float duration)
        {
            _abilityCooldownDurations[abilityType] = duration;
        }
        
        #endregion
        
        #region Aim Line
        
        private void UpdateAimLine()
        {
            if (!_aimLine.enabled) return;
            
            Vector2 aimDirection = _inputManager.IsAiming() 
                ? CalculateAttackDirection(true) 
                : CalculateAttackDirection(false);
            
            _currentAimDirection = aimDirection;
            OnAimDirectionChanged?.Invoke(aimDirection);
            
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + (Vector3)(aimDirection * (_inputManager.IsAiming() ? manualAimRange : attackRange));
            
            _aimLine.SetPosition(0, startPos);
            _aimLine.SetPosition(1, endPos);
        }
        
        #endregion
        
        #region Public API
        
        public void SetHeroData(HeroData data)
        {
            _heroData = data;
            if (data != null && data.CurrentAbility != null)
            {
                attackCooldown = data.CurrentAbility.Cooldown;
                attackRange = data.CurrentAbility.Range;
            }
        }
        
        public void ResetCooldowns()
        {
            foreach (var key in _abilityCooldowns.Keys.ToList())
            {
                _abilityCooldowns[key] = 0f;
            }
            _canAttack = true;
        }
        
        public bool CanAttack() => _canAttack;
        public Vector2 GetCurrentAimDirection() => _currentAimDirection;
        
        #endregion
    }
}
