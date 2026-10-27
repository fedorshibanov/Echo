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
        bool TryStartRecord(List<InputFrame> inputFrames, float time, Action onComplete);
    }

    public class RecordService : IRecordService, IDisposable
    {
        private readonly PlayerController _playerController;
        private List<InputFrame> _inputFrames;
        private Action _onComplete;
        private int _remainingFrames;

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
            _remainingFrames = Mathf.Max(1, Mathf.RoundToInt(time / Time.fixedDeltaTime));
            _onComplete = onComplete;
            return true;
        }

        private void OnInputCaptured(InputFrame frame)
        {
            if (_inputFrames == null) return;

            if (_remainingFrames > 0)
            {
                _inputFrames.Add(frame);
                _remainingFrames--;
                return;
            }

            // Complete on the next tick: the last recorded input has now passed through physics.
            var onComplete = _onComplete;
            _inputFrames = null;
            _onComplete = null;
            onComplete?.Invoke();
        }
    }
}
