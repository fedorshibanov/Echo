using UnityEngine;
using VContainer;
using _Project.Scripts.PlayerSystem;

namespace _Project.Scripts.Level
{
    public class WinTrigger : MonoBehaviour
    {
        private IGameEndService _gameEndService;
        
        [Inject]
        private void Init(IGameEndService gameEndService) => _gameEndService = gameEndService;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") && !other.CompareTag("Echo")) 
                return;
            
            _gameEndService.Win();
        }
    }
}