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
        private readonly CoroutineRunner _coroutineRunner;
        
        [Inject]
        public RecordService(PlayerInputSystem playerInput, CoroutineRunner coroutineRunner)
        {
            _playerInput = playerInput;
            _coroutineRunner = coroutineRunner;
            
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
            
            _coroutineRunner.Run(TakeFrames(inputFrames, time, onComplete));
            return true;
        }

        private IEnumerator TakeFrames(List<InputFrame> inputFrames, float time, Action onComplete)
        {
            _isRecording = true;
            
            for (float timer = 0; time > timer; timer += Time.fixedDeltaTime)
            {
                var move = _playerInput.Player.Move.ReadValue<Vector2>();
                inputFrames.Add(new InputFrame(move, _jumpPressed));
                _jumpPressed = false;
                yield return new WaitForFixedUpdate();
            }
            
            Debug.Log("Record Done");
            _isRecording = false;
            onComplete?.Invoke();
        }

        private void OnJump(InputAction.CallbackContext context) => _jumpPressed = true;
    }
}
