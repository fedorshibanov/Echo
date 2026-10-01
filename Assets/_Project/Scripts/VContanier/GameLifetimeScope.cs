using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Echo;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.Level;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContanier
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Space] [Header("MonoBehavior components")] [Space]
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private List<MonoBehaviour> _rewindItems;
        
        [Space] [Header("Prefabs")] [Space]
        [SerializeField] private EchoController _echoPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputSystem>(Lifetime.Singleton);
            builder.Register<IRecordService, RecordService>(Lifetime.Singleton);
            builder.Register<EchoFactory>(Lifetime.Scoped); //check RegisterFactory
            builder.Register<EchoService>(Lifetime.Scoped); //check RegisterFactory
            builder.Register<RewindService>(Lifetime.Scoped);
            
            builder.RegisterComponent(_coroutineRunner);
            builder.RegisterComponent(_playerController);
            builder.RegisterComponent(_levelManager);
            builder.RegisterComponent(_echoPrefab);
            
            foreach (var item in _rewindItems.OfType<IRewindable>())
            {
                builder.RegisterComponent(item).As<IRewindable>();
            }
        }
    }
}
