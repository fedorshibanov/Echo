using System.Collections.Generic;
using _Project.Scripts.Echo;
using _Project.Scripts.InteractionObjects;
using UnityEngine;

namespace _Project.Scripts
{
    public class RewindService
    {
        private readonly EchoService _echoService;
        private readonly PlayerController _playerController;
        private IReadOnlyList<IRewindable> _items;

        public RewindService(EchoService echoService, PlayerController playerController, IReadOnlyList<IRewindable> items)
        {
            _echoService = echoService;
            _playerController = playerController;
            _items = items;
        }
        
        public void RewindEntities(Vector3 spawnPoint)
        {
            _playerController.transform.position = spawnPoint;
            
            foreach (var echo in _echoService.GetEchos())
            {
                echo.transform.position = spawnPoint;
                echo.MoveEcho();
            }
        }

        public void RewindItems()
        {
            foreach (var item in _items)
            {
                item.Rewind();
            }
        }
    }
}