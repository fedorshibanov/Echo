using System;
using _Project.Scripts.Level;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Scripts.PlayerSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _acceleration = 50f;
        [Range(0f, 1f)] [SerializeField] private float _airControl = 0.5f;
        [SerializeField] private float _rotationSpeed = 720f;

        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private float _gravityMultiplier = 2.5f;
        [SerializeField] private float _coyoteTime = 0.12f;
        [SerializeField] private float _jumpBufferTime = 0.12f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundLayers = ~0;
        [SerializeField] private float _groundCheckRadius = 0.3f;
        [SerializeField] private Vector3 _groundCheckOffset = new(0f, 0.05f, 0f);

        [Space]
        [SerializeField] private Rigidbody _rb;

        private PlayerInputSystem _playerInput;
        private GameStateService _gameStateService;

        private CharacterMotor _motor;
        private bool _jumpPressed;

        // Raised before simulation so recording can finish after the previous physics step.
        public event Action<InputFrame> InputCaptured;

        public CharacterMovementSettings MovementSettings => new()
        {
            MoveSpeed = _moveSpeed,
            Acceleration = _acceleration,
            AirControl = _airControl,
            RotationSpeed = _rotationSpeed,
            JumpHeight = _jumpHeight,
            GravityMultiplier = _gravityMultiplier,
            CoyoteTime = _coyoteTime,
            JumpBufferTime = _jumpBufferTime,
            GroundLayers = _groundLayers,
            GroundCheckRadius = _groundCheckRadius,
            GroundCheckOffset = _groundCheckOffset
        };

        [Inject]
        private void Init(PlayerInputSystem playerInput, GameStateService gameStateService)
        {
            _playerInput = playerInput;
            _gameStateService = gameStateService;
        }

        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            _motor = new CharacterMotor(_rb, MovementSettings);
        }

        private void OnEnable()
        {
            _motor.Reset();
            _jumpPressed = false;
            _playerInput.Player.Enable();
            _playerInput.Player.Jump.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            _playerInput.Player.Jump.performed -= OnJumpPerformed;
            _playerInput.Player.Disable();
        }

        private void FixedUpdate()
        {
            var frame = new InputFrame(_playerInput.Player.Move.ReadValue<Vector2>(), _jumpPressed);
            _jumpPressed = false;
            InputCaptured?.Invoke(frame);

            // Finishing a recording can disable this controller for the rewind.
            if (isActiveAndEnabled)
                _motor.Simulate(frame, Time.fixedDeltaTime);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (_gameStateService.CurrentState == GameState.Pause) return;

            _jumpPressed = true;
        }
    }
}
