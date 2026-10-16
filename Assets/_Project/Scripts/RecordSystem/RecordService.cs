using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

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
        private bool _isRecording;
        private bool _jumpPressed;
        
        private readonly PlayerInputSystem _playerInput;
        
        [Inject]
        public RecordService(PlayerInputSystem playerInput)
        {
            _playerInput = playerInput;
            
            _playerInput.Player.Jump.performed += OnJump;
        }
        
        public void Dispose()
        {
            _playerInput.Player.Jump.performed -= OnJump;
        }
        
        public bool TryStartRecord(List<InputFrame> inputFrames, float time, Action onComplete)
        {
            if (_isRecording)
            {
                Debug.LogWarning("Recording is already running");
                return false;
            }
            
            _ = TakeFramesAsync(inputFrames, time, onComplete);
            return true;
        }

        private async Awaitable TakeFramesAsync(List<InputFrame> inputFrames, float time, Action onComplete = null)
        {
            _isRecording = true;

            try
            {
                for (float timer = 0; timer < time; timer += Time.fixedDeltaTime)
                {
                    var move = _playerInput.Player.Move.ReadValue<Vector2>();
                    inputFrames.Add(new InputFrame(move, _jumpPressed));
                    _jumpPressed = false;

                    await Awaitable.FixedUpdateAsync();
                }

                _isRecording = false;
                onComplete?.Invoke();
            }
            finally
            {
                _isRecording = false;
            }
        }

        private void OnJump(InputAction.CallbackContext context) => _jumpPressed = true;
    }
}
