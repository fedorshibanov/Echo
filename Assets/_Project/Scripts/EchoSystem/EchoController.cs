using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.PlayerSystem;
using _Project.Scripts.RecordSystem;
using UnityEngine;

namespace _Project.Scripts.EchoSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class EchoController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;

        private CharacterMotor _motor;
        private List<InputFrame> _inputFrames;
        private int _frameIndex;
        private bool _isPlaying;
        private TaskCompletionSource<bool> _playbackCompletion;

        public Task PlaybackCompletion => _playbackCompletion?.Task ?? Task.CompletedTask;

        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();
        }

        public void ConfigureMovement(CharacterMovementSettings settings)
        {
            _motor = new CharacterMotor(_rb, settings);
        }

        public void SetFrames(List<InputFrame> frames) => _inputFrames = frames;

        public void MoveEcho()
        {
            if (_isPlaying || _inputFrames == null || _inputFrames.Count == 0) return;

            _motor.Reset();
            _frameIndex = 0;
            _playbackCompletion = new TaskCompletionSource<bool>();
            _isPlaying = true;
        }

        private void OnEnable() => _motor?.Reset();

        private void OnDisable() => StopPlayback();

        private void FixedUpdate()
        {
            var frame = new InputFrame(Vector2.zero, false);
            if (_isPlaying)
            {
                if (_frameIndex < _inputFrames.Count)
                    frame = _inputFrames[_frameIndex++];
                else
                    StopPlayback();
            }

            if (isActiveAndEnabled)
                _motor.Simulate(frame, Time.fixedDeltaTime);
        }

        private void StopPlayback()
        {
            _isPlaying = false;
            _playbackCompletion?.TrySetResult(true);
        }
    }
}
