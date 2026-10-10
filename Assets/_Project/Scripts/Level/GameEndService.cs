using System;

namespace _Project.Scripts.Level
{
    public interface IGameEndService
    {
        void Win();
        void Lose();
        
        event Action OnWin;
        event Action OnLose;
    }
    
    public class GameEndService : IGameEndService
    {
        public event Action OnWin;
        public event Action OnLose;
        
        public void Win()
        {
            OnWin?.Invoke();
        }

        public void Lose()
        {
            OnLose?.Invoke();
        }
    }
}