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
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputSystem>(Lifetime.Singleton);
            builder.Register<IRecordService, RecordService>(Lifetime.Singleton);
            builder.RegisterComponent(_coroutineRunner);
            builder.RegisterComponent(_playerController);
        }
    }
}
