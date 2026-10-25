using System;
using UnityEngine;

namespace _Project.Scripts.Level
{
    public enum GameState
    {
        Ingame,
        Pause
    }

    public sealed class GameStateService : IDisposable
    {
        public GameState CurrentState { get; private set; } = GameState.Ingame;
        public event Action<GameState> StateChanged;

        private float _previousTimeScale;

        public void SetState(GameState state)
        {
            if (CurrentState == state) return;

            if (state == GameState.Pause)
            {
                _previousTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = _previousTimeScale;
            }

            CurrentState = state;
            StateChanged?.Invoke(state);
        }

        public void Dispose()
        {
            SetState(GameState.Ingame);
        }
    }
}
