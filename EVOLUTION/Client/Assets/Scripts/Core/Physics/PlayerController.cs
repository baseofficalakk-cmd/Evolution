using UnityEngine;
using Shared.Models;
using EVOLUTION.Core.Input;

namespace EVOLUTION.Core.Physics
{
    /// <summary>
    /// Player Controller handling movement, rotation, and physical interactions
    /// Works with InputManager for smooth character control
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float deceleration = 30f;
        [SerializeField] private float rotationSmoothness = 10f;
        
        [Header("Boundaries")]
        [SerializeField] private float mapWidth = 2400f;
        [SerializeField] private float mapHeight = 1600f;
        
        // Components
        private Rigidbody2D _rb;
        private InputManager _inputManager;
        private HeroData _heroData;
        
        // State
        private Vector2 _currentVelocity;
        private Vector2 _targetDirection;
        private bool _isInitialized;
        
        // Events
        public event System.Action<Vector2> OnPositionChanged;
        public event System.Action<float> OnRotationChanged;
        public event System.Action<bool> OnMovementStateChanged;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _inputManager = FindObjectOfType<InputManager>();
            
            if (_inputManager == null)
            {
                Debug.LogError("InputManager not found! Creating one automatically.");
                GameObject inputObj = new GameObject("InputManager");
                _inputManager = inputObj.AddComponent<InputManager>();
            }
        }
        
        private void Start()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            // Subscribe to input events
            _inputManager.OnMoveInput += HandleMoveInput;
            _inputManager.OnAimInput += HandleAimInput;
            
            // Set initial speed from hero data if available
            if (_heroData != null)
            {
                moveSpeed = _heroData.BaseStats.MoveSpeed;
            }
            
            _isInitialized = true;
            OnMovementStateChanged?.Invoke(false); // Not moving initially
        }
        
        private void OnDestroy()
        {
            if (_inputManager != null)
            {
                _inputManager.OnMoveInput -= HandleMoveInput;
                _inputManager.OnAimInput -= HandleAimInput;
            }
        }
        
        private void FixedUpdate()
        {
            if (!_isInitialized) return;
            
            ApplyMovement();
            ApplyRotation();
            ApplyBoundaries();
        }
        
        private void HandleMoveInput(Vector2 input)
        {
            if (input.magnitude > 0.1f)
            {
                _targetDirection = input.normalized;
                Accelerate();
            }
            else
            {
                Decelerate();
            }
        }
        
        private void HandleAimInput(Vector2 aimPosition)
        {
            Vector2 direction = (Vector2)transform.position - aimPosition;
            if (direction.magnitude > 0.1f)
            {
                _targetDirection = -direction.normalized;
            }
        }
        
        private void Accelerate()
        {
            _currentVelocity = Vector2.Lerp(
                _currentVelocity,
                _targetDirection * moveSpeed,
                acceleration * Time.fixedDeltaTime
            );
            
            _rb.velocity = _currentVelocity;
            OnMovementStateChanged?.Invoke(_currentVelocity.magnitude > 0.5f);
        }
        
        private void Decelerate()
        {
            _currentVelocity = Vector2.Lerp(
                _currentVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
            
            _rb.velocity = _currentVelocity;
            
            if (_currentVelocity.magnitude < 0.1f)
            {
                OnMovementStateChanged?.Invoke(false);
            }
        }
        
        private void ApplyMovement()
        {
            // Movement is applied through Rigidbody2D velocity in Accelerate/Decelerate
            OnPositionChanged?.Invoke((Vector2)transform.position);
        }
        
        private void ApplyRotation()
        {
            if (_targetDirection.magnitude < 0.1f) return;
            
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, _targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime);
            
            float angle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg - 90f;
            OnRotationChanged?.Invoke(angle);
        }
        
        private void ApplyBoundaries()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -mapWidth / 2f, mapWidth / 2f);
            pos.y = Mathf.Clamp(pos.y, -mapHeight / 2f, mapHeight / 2f);
            transform.position = pos;
        }
        
        public void SetHeroData(HeroData data)
        {
            _heroData = data;
            if (_isInitialized && data != null)
            {
                moveSpeed = data.BaseStats.MoveSpeed;
            }
        }
        
        public void SetMoveSpeed(float speed) => moveSpeed = speed;
        public float GetMoveSpeed() => moveSpeed;
        public Vector2 GetCurrentVelocity() => _currentVelocity;
        public bool IsMoving() => _currentVelocity.magnitude > 0.5f;
        
        public void StopMovement()
        {
            _currentVelocity = Vector2.zero;
            _rb.velocity = Vector2.zero;
            _targetDirection = Vector2.zero;
        }
    }
}
