using _Project.Scripts.RecordSystem;
using UnityEngine;

namespace _Project.Scripts.PlayerSystem
{
    public struct CharacterMovementSettings
    {
        public float MoveSpeed;
        public float Acceleration;
        public float AirControl;
        public float RotationSpeed;
        public float JumpHeight;
        public float GravityMultiplier;
        public float CoyoteTime;
        public float JumpBufferTime;
        public LayerMask GroundLayers;
        public float GroundCheckRadius;
        public Vector3 GroundCheckOffset;
    }

    public sealed class CharacterMotor
    {
        private readonly Rigidbody _body;
        private readonly CharacterMovementSettings _settings;
        private float _time;
        private float _lastGroundedTime = float.NegativeInfinity;
        private float _lastJumpPressedTime = float.NegativeInfinity;

        public CharacterMotor(Rigidbody body, CharacterMovementSettings settings)
        {
            _body = body;
            _settings = settings;
            _body.freezeRotation = true;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void Reset()
        {
            _time = 0f;
            _lastGroundedTime = float.NegativeInfinity;
            _lastJumpPressedTime = float.NegativeInfinity;
            if (!_body.isKinematic)
            {
                _body.linearVelocity = Vector3.zero;
                _body.angularVelocity = Vector3.zero;
            }
        }

        public void Simulate(InputFrame frame, float deltaTime)
        {
            if (frame.Jump)
                _lastJumpPressedTime = _time;

            var grounded = _body.linearVelocity.y <= 0.1f &&
                           Physics.CheckSphere(_body.position + _settings.GroundCheckOffset,
                               _settings.GroundCheckRadius, _settings.GroundLayers,
                               QueryTriggerInteraction.Ignore);
            if (grounded)
                _lastGroundedTime = _time;

            var direction = Vector3.ClampMagnitude(new Vector3(frame.Move.x, 0f, frame.Move.y), 1f);
            var velocity = _body.linearVelocity;
            var horizontal = new Vector3(velocity.x, 0f, velocity.z);
            var acceleration = _settings.Acceleration * (grounded ? 1f : _settings.AirControl);
            horizontal = Vector3.MoveTowards(horizontal, direction * _settings.MoveSpeed, acceleration * deltaTime);
            velocity.x = horizontal.x;
            velocity.z = horizontal.z;

            if (_time - _lastJumpPressedTime <= _settings.JumpBufferTime &&
                _time - _lastGroundedTime <= _settings.CoyoteTime)
            {
                velocity.y = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * _settings.JumpHeight);
                _lastJumpPressedTime = float.NegativeInfinity;
                _lastGroundedTime = float.NegativeInfinity;
            }

            _body.linearVelocity = velocity;
            if (direction.sqrMagnitude > 0.0001f)
            {
                var target = Quaternion.LookRotation(direction, Vector3.up);
                _body.MoveRotation(Quaternion.RotateTowards(_body.rotation, target,
                    _settings.RotationSpeed * deltaTime));
            }

            if (velocity.y < 0f && _settings.GravityMultiplier > 1f)
                _body.AddForce(Physics.gravity * (_settings.GravityMultiplier - 1f), ForceMode.Acceleration);

            _time += deltaTime;
        }
    }
}
