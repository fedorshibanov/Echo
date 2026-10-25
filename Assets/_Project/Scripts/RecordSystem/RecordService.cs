using System;
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
        private readonly PlayerInputSystem _playerInput;

        private bool _isRecording;
        private bool _jumpPressed;

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

            // Флаг ставим сразу, чтобы два вызова подряд не запустили две записи
            _isRecording = true;
            _jumpPressed = false;

            _ = TakeFramesAsync(inputFrames, time, onComplete);
            return true;
        }

        private async Awaitable TakeFramesAsync(List<InputFrame> inputFrames, float time, Action onComplete)
        {
            // Количество кадров считаем целым числом, без накопления float-ошибки
            var totalFrames = Mathf.Max(1, Mathf.RoundToInt(time / Time.fixedDeltaTime));
            var completed = false;

            try
            {
                var token = Application.exitCancellationToken;

                // Сначала дожидаемся физического тика, чтобы первый кадр
                // записывался в тике, а не в произвольный момент Update
                await Awaitable.FixedUpdateAsync(token);

                for (var i = 0; i < totalFrames; i++)
                {
                    var move = _playerInput.Player.Move.ReadValue<Vector2>();
                    inputFrames.Add(new InputFrame(move, _jumpPressed));
                    _jumpPressed = false;

                    if (i < totalFrames - 1)
                        await Awaitable.FixedUpdateAsync(token);
                }

                completed = true;
            }
            finally
            {
                _isRecording = false;
                _jumpPressed = false;
            }

            if (completed)
                onComplete?.Invoke();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            // Нажатия вне записи игнорируем, иначе старое нажатие попадёт в первый кадр
            if (_isRecording)
                _jumpPressed = true;
        }
    }
}