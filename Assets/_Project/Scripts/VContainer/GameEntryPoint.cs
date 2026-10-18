using System;
using _Project.Scripts.Level;
using _Project.Scripts.SaveSystem;
using VContainer.Unity;

namespace _Project.Scripts.VContainer
{
    public class GameEntryPoint : IStartable, IDisposable
    {
        private readonly ISaveService _saveService;
        private readonly IGameEndService _gameEndService;
        
        public GameEntryPoint(ISaveService saveService, IGameEndService gameEndService)
        {
            _saveService = saveService;
            _gameEndService = gameEndService;
        }
        
        public void Start()
        {
            _gameEndService.OnWin += _saveService.Save;
        }

        public void Dispose()
        {
            _gameEndService.OnWin -= _saveService.Save;
        }
    }
}