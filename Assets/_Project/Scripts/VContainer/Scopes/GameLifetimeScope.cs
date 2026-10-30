using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.EchoSystem;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.Level;
using _Project.Scripts.PlayerSystem;
using _Project.Scripts.RecordSystem;
using _Project.Scripts.RewindSystem;
using _Project.Scripts.SaveSystem;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Level;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Scripts.VContainer.Scopes
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Space] [Header("MonoBehavior components")] [Space]
        [SerializeField] private Player _player;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private WinTrigger _winTrigger;
        [SerializeField] private List<MonoBehaviour> _rewindItems;
        [SerializeField] private LevelMenuController _levelMenuController;
        [SerializeField] private LevelTimerView _levelTimerView;
        
        [Space] [Header("Prefabs")] [Space]
        [SerializeField] private Echo _echoPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputSystem>(Lifetime.Scoped);
            builder.Register<GameStateService>(Lifetime.Scoped);
            builder.Register<IRecordService, RecordService>(Lifetime.Scoped);
            builder.Register<IGameEndService, GameEndService>(Lifetime.Scoped);
            builder.Register<EchoFactory>(Lifetime.Scoped);
            builder.Register<EchoService>(Lifetime.Scoped);
            builder.Register<RewindService>(Lifetime.Scoped);

            builder.RegisterComponent(_player);
            builder.RegisterComponent(_player.Controller);
            builder.RegisterComponent(_echoPrefab);
            builder.RegisterComponent(_levelManager);
            builder.RegisterComponent(_winTrigger);
            builder.RegisterComponent(_levelMenuController);
            if (_levelTimerView != null)
                builder.RegisterComponent(_levelTimerView);

            var rewindItems = _rewindItems.OfType<IRewindable>().ToList();
            builder.RegisterInstance<IReadOnlyList<IRewindable>>(rewindItems);
            
            builder.RegisterEntryPoint<GameEntryPoint>();

#if UNITY_EDITOR
            builder.Register<ISceneChanger, SceneChanger>(Lifetime.Singleton);
            builder.Register<ISaveService, JsonSaveService>(Lifetime.Scoped);
#endif
        }
    }
}
