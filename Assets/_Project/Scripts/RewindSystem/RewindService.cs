using System.Collections.Generic;
using _Project.Scripts.EchoSystem;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.PlayerSystem;
using _Project.Scripts.RewindSystem;
using UnityEngine;

namespace _Project.Scripts
{
    public class RewindService
    {
        private readonly EchoService _echoService;
        private readonly Player _player;
        private readonly IReadOnlyList<IRewindable> _items;

        public RewindService(EchoService echoService, Player player, IReadOnlyList<IRewindable> items)
        {
            _echoService = echoService;
            _player = player;
            _items = items;
        }

        public void RewindAll(Vector3 spawnPoint)
        {
            RewindEntities(spawnPoint);
            RewindItems();
        }
        
        public void RewindEntities(Vector3 spawnPoint)
        {
            _player.transform.position = spawnPoint;
            
            foreach (var echo in _echoService.GetEchos())
            {
                Debug.Log(echo.ToString());
                echo.transform.position = spawnPoint;
                echo.EchoController.MoveEcho();
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