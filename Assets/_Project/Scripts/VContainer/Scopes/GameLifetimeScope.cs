using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.EchoSystem;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.Level;
using _Project.Scripts.PlayerSystem;
using _Project.Scripts.RecordSystem;
using _Project.Scripts.RewindSystem;
using _Project.Scripts.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContainer.Scopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Space] [Header("MonoBehavior components")] [Space]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Player _player;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private WinTrigger _winTrigger;
        [SerializeField] private List<MonoBehaviour> _rewindItems;
        [SerializeField] private LevelMenuController _levelMenuController;
        
        [Space] [Header("Prefabs")] [Space]
        [SerializeField] private Echo _echoPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputSystem>(Lifetime.Scoped);
            builder.Register<IRecordService, RecordService>(Lifetime.Scoped);
            builder.Register<IGameEndService, GameEndService>(Lifetime.Scoped);
            builder.Register<EchoFactory>(Lifetime.Scoped);
            builder.Register<EchoService>(Lifetime.Scoped);
            builder.Register<RewindService>(Lifetime.Scoped);

            builder.RegisterComponent(_playerController);
            builder.RegisterComponent(_player);
            builder.RegisterComponent(_levelManager);
            builder.RegisterComponent(_echoPrefab);
            builder.RegisterComponent(_winTrigger);
            builder.RegisterComponent(_levelMenuController);

            var rewindItems = _rewindItems.OfType<IRewindable>().ToList();
            builder.RegisterInstance<IReadOnlyList<IRewindable>>(rewindItems);
            
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
