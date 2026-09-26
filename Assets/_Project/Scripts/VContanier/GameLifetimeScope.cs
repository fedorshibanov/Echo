using _Project.Scripts.Echo;
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
        
        [Space] [Header("Prefabs")] [Space]
        [SerializeField] private EchoController _echoPrefab;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputSystem>(Lifetime.Singleton);
            builder.Register<IRecordService, RecordService>(Lifetime.Singleton);
            builder.Register<EchoFactory>(Lifetime.Scoped); //check RegisterFactory
            builder.RegisterComponent(_coroutineRunner);
            builder.RegisterComponent(_playerController);
            builder.RegisterComponent(_levelManager);

            builder.RegisterComponentInNewPrefab(_echoPrefab, Lifetime.Scoped);
        }
    }
}
