using UnityEngine;
using Shared.Enums;
using Shared.Models;

namespace EVOLUTION.Core.Input
{
    /// <summary>
    /// Unified Input Manager handling all player inputs
    /// Supports WASD movement, mouse aim, and ability triggers
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float rotationSmoothness = 10f;
        
        // Current input state
        private Vector2 _moveInput;
        private Vector2 _mousePosition;
        private bool _attackPressed;
        private bool _aimHeld;
        private bool _superPressed;
        
        // Cached components
        private Camera _mainCamera;
        
        // Events for input actions
        public event System.Action<Vector2> OnMoveInput;
        public event System.Action<Vector2> OnAimInput;
        public event System.Action OnAttackPressed;
        public event System.Action OnAttackReleased;
        public event System.Action OnAimStarted;
        public event System.Action OnAimEnded;
        public event System.Action OnSuperPressed;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
        }
        
        private void Update()
        {
            HandleMovementInput();
            HandleMouseInput();
            HandleActionInputs();
        }
        
        private void HandleMovementInput()
        {
            _moveInput = Vector2.zero;
            
            if (Input.GetKey(KeyCode.W)) _moveInput.y += 1f;
            if (Input.GetKey(KeyCode.S)) _moveInput.y -= 1f;
            if (Input.GetKey(KeyCode.A)) _moveInput.x -= 1f;
            if (Input.GetKey(KeyCode.D)) _moveInput.x += 1f;
            
            // Normalize to prevent faster diagonal movement
            if (_moveInput.magnitude > 1f)
                _moveInput.Normalize();
            
            OnMoveInput?.Invoke(_moveInput);
        }
        
        private void HandleMouseInput()
        {
            if (_mainCamera == null) return;
            
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            _mousePosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            
            OnAimInput?.Invoke(_mousePosition);
        }
        
        private void HandleActionInputs()
        {
            // Attack (Left Mouse Button)
            if (Input.GetMouseButtonDown(0))
            {
                _attackPressed = true;
                OnAttackPressed?.Invoke();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _attackPressed = false;
                OnAttackReleased?.Invoke();
            }
            
            // Manual Aim (Hold Right Mouse Button)
            if (Input.GetMouseButtonDown(1))
            {
                _aimHeld = true;
                OnAimStarted?.Invoke();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _aimHeld = false;
                OnAimEnded?.Invoke();
            }
            
            // Super Ability (Space)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _superPressed = true;
                OnSuperPressed?.Invoke();
                _superPressed = false; // Reset immediately for single trigger
            }
        }
        
        public Vector2 GetMoveInput() => _moveInput;
        public Vector2 GetMousePosition() => _mousePosition;
        public bool IsAiming() => _aimHeld;
        public bool IsAttacking() => _attackPressed;
        
        public void SetMoveSpeed(float speed) => moveSpeed = speed;
        public float GetMoveSpeed() => moveSpeed;
    }
}
