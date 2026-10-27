using System;
using System.Collections.Generic;
using _Project.Scripts.PlayerSystem;
using UnityEngine;

namespace _Project.Scripts.RecordSystem
{
    public struct InputFrame
    {
        public Vector2 Move { get; set; }
        public bool Jump { get; set; }

        public InputFrame(Vector2 move, bool jump) => (Move, Jump) = (move, jump);
    }

    public interface IRecordService
    {
        int RemainingSeconds { get; }
        event Action<int> RemainingSecondsChanged;

        bool TryStartRecord(List<InputFrame> inputFrames, float time, Action onComplete);
    }

    public class RecordService : IRecordService, IDisposable
    {
        public int RemainingSeconds { get; private set; }
        public event Action<int> RemainingSecondsChanged;

        private readonly PlayerController _playerController;
        private List<InputFrame> _inputFrames;
        private Action _onComplete;
        private int _remainingFrames;
        private float _recordStep;

        public RecordService(PlayerController playerController)
        {
            _playerController = playerController;
            _playerController.InputCaptured += OnInputCaptured;
        }

        public void Dispose()
        {
            _playerController.InputCaptured -= OnInputCaptured;
            _inputFrames = null;
            _onComplete = null;
        }

        public bool TryStartRecord(List<InputFrame> inputFrames, float time, Action onComplete)
        {
            if (_inputFrames != null)
            {
                Debug.LogWarning("Recording is already running");
                return false;
            }

            _inputFrames = inputFrames ?? throw new ArgumentNullException(nameof(inputFrames));
            _recordStep = Time.fixedDeltaTime;
            _remainingFrames = Mathf.Max(1, Mathf.RoundToInt(time / _recordStep));
            _onComplete = onComplete;
            UpdateRemainingSeconds();
            return true;
        }

        private void OnInputCaptured(InputFrame frame)
        {
            if (_inputFrames == null) return;

            if (_remainingFrames > 0)
            {
                _inputFrames.Add(frame);
                _remainingFrames--;
                UpdateRemainingSeconds();
                return;
            }

            var onComplete = _onComplete;
            _inputFrames = null;
            _onComplete = null;
            onComplete?.Invoke();
        }

        private void UpdateRemainingSeconds()
        {
            var seconds = Mathf.CeilToInt(_remainingFrames * _recordStep);
            if (RemainingSeconds == seconds) return;

            RemainingSeconds = seconds;
            RemainingSecondsChanged?.Invoke(seconds);
        }
    }
}
