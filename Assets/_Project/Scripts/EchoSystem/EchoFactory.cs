using System.Collections.Generic;
using _Project.Scripts.RecordSystem;
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

        public EchoFactory(Echo echoPrefab, IObjectResolver objectResolver, EchoService echoService)
        {
            _echoPrefab = echoPrefab;
            _objectResolver = objectResolver;
            _echoService = echoService;
        }
        
        public Echo Spawn(List<InputFrame> frames, Transform position)
        {
            var echo = _objectResolver.Instantiate(_echoPrefab);
            
            echo.transform.position = position.position;
            echo.Controller.SetFrames(frames);
            
            _echoService.AddEcho(echo);
            
            return echo;
        }
    }
}