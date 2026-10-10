using UnityEngine;
using VContainer;
using _Project.Scripts.PlayerSystem;

namespace _Project.Scripts.Level
{
    public class WinTrigger : MonoBehaviour
    {
        private GameEndService _gameEndService;
        
        [Inject]
        private void Init(GameEndService gameEndService) => _gameEndService = gameEndService;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Player>(out var obj)) 
                return;
            
            _gameEndService.Win();
        }
    }
}