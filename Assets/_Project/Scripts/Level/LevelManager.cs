using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.EchoSystem;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelConfig _levelConfig;
        [Space]
        [SerializeField] private Transform _spawnPoint;

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

        private void Start()
        {
            StartLevel();
        }

        private void StartLevel()
        {
            StartCoroutine(StartGameCycle());
        }

        private IEnumerator StartGameCycle()
        {
            for (var i = 0; i < _levelConfig.echoMax; i++)
            {
                var frames = new List<InputFrame>();
                var isDone = false;
                
                if(!_recordService.TryStartRecord(frames, _levelConfig.time, () => isDone = true))
                    yield break;
                
                yield return new WaitUntil(() => isDone);

                _echoFactory.Spawn(frames, _spawnPoint);
                _rewindService.RewindEntities(_spawnPoint.position);
                _rewindService.RewindItems();

                yield return new WaitForFixedUpdate();
            }          
            
            _gameEndService.Lose();
        }
    }
}