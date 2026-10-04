using System.Collections.Generic;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.Echo
{
    public class EchoFactory
    {
        private readonly EchoController _echoPrefab;
        private readonly IObjectResolver _objectResolver;
        private readonly EchoService _echoService;

        public EchoFactory(EchoController echoPrefab, IObjectResolver objectResolver, EchoService echoService)
        {
            _echoPrefab = echoPrefab;
            _objectResolver = objectResolver;
            _echoService = echoService;
        }
        
        public EchoController Spawn(List<InputFrame> frames, Transform position)
        {
            var echo = _objectResolver.Instantiate(_echoPrefab);
            
            echo.transform.position = position.position;
            echo.SetFrames(frames);
            
            _echoService.AddEcho(echo);
            
            return echo;
        }
    }
}