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

        public EchoFactory(EchoController echoPrefab, IObjectResolver objectResolver)
        {
            _echoPrefab = echoPrefab;
            _objectResolver = objectResolver;
        }
        
        public EchoController Spawn(List<InputFrame> frames)
        {
            var echo = _objectResolver.Instantiate(_echoPrefab);
            echo.SetFrames(frames);
            return echo;
        }
    }
}