using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.EchoSystem;
using _Project.Scripts.RecordSystem;
using _Project.Scripts.RewindSystem;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelConfig _levelConfig;
        [Space] [SerializeField] private Transform _spawnPoint;

        private IRecordService _recordService;
        private EchoFactory _echoFactory;
        private RewindService _rewindService;
        private IGameEndService _gameEndService;

        [Inject]
        private void Init(IRecordService recordService, EchoFactory echoFactory,
            RewindService rewindService, IGameEndService gameEndService)
        {
            _recordService = recordService;
            _echoFactory = echoFactory;
            _rewindService = rewindService;
            _gameEndService = gameEndService;
        }

        private async void Start() => await StartGameCycle();

        private async Task StartGameCycle()
        {
            for (var i = 0; i < _levelConfig.echoMax; i++)
            {
                var frames = new List<InputFrame>();
                var done = new TaskCompletionSource<bool>();

                if (!_recordService.TryStartRecord(frames, _levelConfig.time, () => done.TrySetResult(true)))
                    return;

                await done.Task;
                if (this == null) return;

                await _rewindService.RewindAll(_spawnPoint.position);
                if (this == null) return;

                var echo = _echoFactory.Spawn(frames, _spawnPoint);
                echo.Controller.MoveEcho();
            }

            _gameEndService.Lose();
        }
    }
}