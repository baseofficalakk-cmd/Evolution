using UnityEngine;
using Shared.Enums;
using Shared.Models;

namespace EVOLUTION.Gameplay.Projectiles
{
    /// <summary>
    /// Base Projectile class for all projectile types
    /// Handles movement, collision, damage, and lifetime
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private float damage = 100f;
        [SerializeField] private ProjectileType projectileType;
        [SerializeField] private DamageType damageType;
        
        [Header("Visual Settings")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject trailPrefab;
        
        // Components
        private Rigidbody2D _rb;
        private Collider2D _collider;
        
        // State
        private Vector2 _direction;
        private float _remainingLifetime;
        private HeroData _ownerData;
        private bool _isInitialized;
        
        // Pooling
        private static readonly System.Collections.Generic.Queue<Projectile> _pool = new();
        private const int MAX_POOL_SIZE = 50;
        
        // Events
        public event System.Action<Projectile, Collider2D> OnHit;
        public event System.Action<Projectile> OnExpired;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            
            if (trailPrefab != null)
            {
                Instantiate(trailPrefab, transform);
            }
        }
        
        private void Update()
        {
            if (!_isInitialized) return;
            
            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Expire();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isInitialized) return;
            
            // Ignore owner's collider
            if (other.gameObject == _ownerData?.OwnerGameObject) return;
            
            // Check for wall collision
            if (other.CompareTag("Wall"))
            {
                SpawnHitEffect(other.transform.position);
                Expire();
                return;
            }
            
            // Check for enemy collision
            if (other.CompareTag("Player") || other.CompareTag("Enemy") || other.CompareTag("Bot"))
            {
                var targetHero = other.GetComponentInParent<Hero>();
                if (targetHero != null && !IsAlly(targetHero))
                {
                    ApplyDamage(targetHero);
                    SpawnHitEffect(other.transform.position);
                    OnHit?.Invoke(this, other);
                    Expire();
                }
            }
        }
        
        private void ApplyDamage(Hero target)
        {
            if (_ownerData == null) return;
            
            float finalDamage = damage;
            
            // Apply damage type modifiers
            switch (damageType)
            {
                case DamageType.Normal:
                    finalDamage = damage;
                    break;
                case DamageType.Critical:
                    finalDamage = damage * 1.5f;
                    break;
                case DamageType.Area:
                    finalDamage = damage * 0.7f; // Reduced for AoE
                    break;
            }
            
            target.TakeDamage(finalDamage, _ownerData, damageType, projectileType);
        }
        
        private bool IsAlly(Hero other)
        {
            if (_ownerData == null || other.HeroData == null) return false;
            return _ownerData.TeamId == other.HeroData.TeamId;
        }
        
        private void SpawnHitEffect(Vector3 position)
        {
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, position, Quaternion.identity);
            }
        }
        
        private void Expire()
        {
            OnExpired?.Invoke(this);
            ReturnToPool();
        }
        
        #region Initialization & Pooling
        
        public void Initialize(Vector2 startPosition, Vector2 direction, HeroData owner, ProjectileConfig config)
        {
            transform.position = startPosition;
            _direction = direction.normalized;
            _ownerData = owner;
            speed = config.Speed;
            lifetime = config.Lifetime;
            damage = config.BaseDamage;
            projectileType = config.Type;
            damageType = config.DamageType;
            
            _rb.velocity = _direction * speed;
            _remainingLifetime = lifetime;
            _isInitialized = true;
            
            // Rotate to face direction
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        public static Projectile GetFromPool()
        {
            if (_pool.Count > 0)
            {
                var projectile = _pool.Dequeue();
                projectile.gameObject.SetActive(true);
                return projectile;
            }
            
            // Create new if pool is empty
            GameObject obj = new GameObject("Projectile");
            return obj.AddComponent<Projectile>();
        }
        
        public void ReturnToPool()
        {
            _isInitialized = false;
            _rb.velocity = Vector2.zero;
            gameObject.SetActive(false);
            
            if (_pool.Count < MAX_POOL_SIZE)
            {
                _pool.Enqueue(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        #endregion
        
        #region Getters
        
        public ProjectileType GetProjectileType() => projectileType;
        public DamageType GetDamageType() => damageType;
        public float GetDamage() => damage;
        public HeroData GetOwner() => _ownerData;
        
        #endregion
    }
}
