using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Scripts.Player
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

        private bool _isGrounded;
        private bool _jumpHeld;
        private float _lastGroundedTime = float.NegativeInfinity;
        private float _lastJumpPressedTime = float.NegativeInfinity;

        [Inject]
        private void Init(PlayerInputSystem playerInput) => _playerInput = playerInput;

        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void OnEnable()
        {
            _playerInput.Player.Enable();
            _playerInput.Player.Jump.performed += OnJumpPerformed;
            _playerInput.Player.Jump.canceled += OnJumpCanceled;
        }

        private void OnDisable()
        {
            _playerInput.Player.Jump.performed -= OnJumpPerformed;
            _playerInput.Player.Jump.canceled -= OnJumpCanceled;
            _playerInput.Player.Disable();
        }

        private void FixedUpdate()
        {
            CheckGround();
            Move();
            Jump();
        }

        private void CheckGround()
        {
            _isGrounded = _rb.linearVelocity.y <= 0.1f &&
                          Physics.CheckSphere(transform.position + _groundCheckOffset, _groundCheckRadius,
                              _groundLayers, QueryTriggerInteraction.Ignore);

            if (_isGrounded)
                _lastGroundedTime = Time.time;
        }

        private void Move()
        {
            var input = _playerInput.Player.Move.ReadValue<Vector2>();
            var direction = new Vector3(input.x, 0f, input.y);


            var velocity = _rb.linearVelocity;
            var horizontal = new Vector3(velocity.x, 0f, velocity.z);
            var acceleration = _isGrounded ? _acceleration : _acceleration * _airControl;

            horizontal = Vector3.MoveTowards(horizontal, direction * _moveSpeed, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);

            if (direction.sqrMagnitude > 0.0001f)
            {
                var target = Quaternion.LookRotation(direction, Vector3.up);
                _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, target, _rotationSpeed * Time.fixedDeltaTime));
            }
        }

        private void Jump()
        {
            var hasBufferedJump = Time.time - _lastJumpPressedTime <= _jumpBufferTime;
            var canJump = Time.time - _lastGroundedTime <= _coyoteTime;

            if (hasBufferedJump && canJump)
            {
                var velocity = _rb.linearVelocity;
                velocity.y = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * _jumpHeight);
                _rb.linearVelocity = velocity;

                _lastJumpPressedTime = float.NegativeInfinity;
                _lastGroundedTime = float.NegativeInfinity;
            }

            var rising = _rb.linearVelocity.y > 0f;
            if (!rising || !_jumpHeld)
                _rb.AddForce(Physics.gravity * (_gravityMultiplier - 1f), ForceMode.Acceleration);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            _jumpHeld = true;
            _lastJumpPressedTime = Time.time;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context) => _jumpHeld = false;
    }
}
