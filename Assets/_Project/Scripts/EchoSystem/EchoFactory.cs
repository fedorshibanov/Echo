using System.Collections.Generic;
using _Project.Scripts.RecordSystem;
using _Project.Scripts.PlayerSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.EchoSystem
{
    public class EchoFactory
    {
        private readonly Echo _echoPrefab;
        private readonly IObjectResolver _objectResolver;
        private readonly EchoService _echoService;
        private readonly PlayerController _playerController;

        public EchoFactory(Echo echoPrefab, IObjectResolver objectResolver, EchoService echoService, PlayerController playerController)
        {
            _echoPrefab = echoPrefab;
            _objectResolver = objectResolver;
            _echoService = echoService;
            _playerController = playerController;
        }
        
        public Echo Spawn(List<InputFrame> frames, Transform position)
        {
            var echo = _objectResolver.Instantiate(_echoPrefab);
            
            echo.Rigidbody.position = position.position;
            echo.Rigidbody.rotation = position.rotation;
            echo.Controller.ConfigureMovement(_playerController.MovementSettings);
            echo.Controller.SetFrames(frames);
            
            _echoService.AddEcho(echo);
            
            return echo;
        }
    }
}
