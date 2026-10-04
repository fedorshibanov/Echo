using System.Collections.Generic;
using _Project.Scripts.RecordSystem;
using UnityEngine;

namespace _Project.Scripts.Echo
{
    [RequireComponent(typeof(Rigidbody))]
    public class EchoController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _acceleration = 60f;
        [SerializeField] private float _deceleration = 80f;
        [Range(0f, 1f)]
        [SerializeField] private float _airControl = 0.5f;
        [SerializeField] private float _rotationSpeed = 720f;

        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private float _fallMultiplier = 2.5f;
        [SerializeField] private float _coyoteTime = 0.12f;
        [SerializeField] private float _jumpBufferTime = 0.12f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask _groundLayers = ~0;
        [SerializeField] private Vector3 _groundCheckOffset = new Vector3(0f, 0.05f, 0f);
        [SerializeField] private float _groundCheckRadius = 0.3f;
        
        [SerializeField] private Rigidbody _rb;

        private List<InputFrame> _inputFrames;
        private int _frameIndex;
        private bool _isPlaying;

        private Vector2 _moveInput;
        private bool _isGrounded;

        private float _lastGroundedTime = float.NegativeInfinity;
        private float _lastJumpPressedTime = float.NegativeInfinity;
        private float _groundIgnoreUntil;

        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void SetFrames(List<InputFrame> frames)
        {
            _inputFrames = frames;
        }

        public void MoveEcho()
        {
            if (_isPlaying) return;
            if (_inputFrames == null || _inputFrames.Count == 0) return;

            _frameIndex = 0;
            _isPlaying = true;
        }

        private void FixedUpdate()
        {
            ReadCurrentFrame();
            CheckGround();

            var moveDirection = GetMoveDirection();

            Move(moveDirection);
            Rotate(moveDirection);
            TryJump();
            ApplyExtraGravity();
        }

        private void ReadCurrentFrame()
        {
            if (!_isPlaying)
            {
                _moveInput = Vector2.zero;
                return;
            }

            if (_frameIndex >= _inputFrames.Count)
            {
                StopPlayback();
                return;
            }

            var frame = _inputFrames[_frameIndex++];

            _moveInput = frame.Move;

            if (frame.Jump)
                _lastJumpPressedTime = Time.time;
        }

        private void StopPlayback()
        {
            _isPlaying = false;
            _moveInput = Vector2.zero;
        }

        private void CheckGround()
        {
            _isGrounded = Time.time >= _groundIgnoreUntil &&
                          Physics.CheckSphere(
                              transform.position + _groundCheckOffset,
                              _groundCheckRadius,
                              _groundLayers,
                              QueryTriggerInteraction.Ignore);

            if (_isGrounded)
                _lastGroundedTime = Time.time;
        }

        private Vector3 GetMoveDirection()
        {
            var input = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0f, _moveInput.y), 1f);
            return input;
        }

        private void Move(Vector3 moveDirection)
        {
            var velocity = _rb.linearVelocity;
            var horizontal = new Vector3(velocity.x, 0f, velocity.z);
            var target = moveDirection * _moveSpeed;

            var rate = moveDirection.sqrMagnitude > 0.0001f ? _acceleration : _deceleration;
            if (!_isGrounded)
                rate *= _airControl;

            horizontal = Vector3.MoveTowards(horizontal, target, rate * Time.fixedDeltaTime);

            _rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
        }

        private void Rotate(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude < 0.0001f)
                return;

            var targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            _rb.MoveRotation(Quaternion.RotateTowards(
                _rb.rotation,
                targetRotation,
                _rotationSpeed * Time.fixedDeltaTime));
        }

        private void TryJump()
        {
            var hasBufferedJump = Time.time - _lastJumpPressedTime <= _jumpBufferTime;
            var canJump = Time.time - _lastGroundedTime <= _coyoteTime;

            if (!hasBufferedJump || !canJump)
                return;

            var velocity = _rb.linearVelocity;
            velocity.y = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * _jumpHeight);
            _rb.linearVelocity = velocity;

            _lastJumpPressedTime = float.NegativeInfinity;
            _lastGroundedTime = float.NegativeInfinity;
            _groundIgnoreUntil = Time.time + 0.1f;
            _isGrounded = false;
        }

        private void ApplyExtraGravity()
        {
            if (_rb.linearVelocity.y < 0f && _fallMultiplier > 1f)
                _rb.AddForce(Physics.gravity * (_fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }
}