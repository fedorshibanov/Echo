using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Echo;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData _levelData;
        [Space]
        [SerializeField] private Transform _spawnPoint;
        
        public event Action OnGameOver;

        private IRecordService _recordService;
        private EchoFactory _echoFactory;
        private RewindService _rewindService;
        
        [Inject]
        private void Init(IRecordService recordService, EchoFactory echoFactory, RewindService rewindService)
        {
            _recordService = recordService;
            _echoFactory = echoFactory;
            _rewindService = rewindService;
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
            for (var i = 0; i < _levelData.echoMax; i++)
            {
                var frames = new List<InputFrame>();
                _recordService.TryStartRecord(frames, _levelData.time);
                
                yield return new WaitForSeconds(_levelData.time);

                _echoFactory.Spawn(frames, _spawnPoint);
                
                _rewindService.RewindEntities(_spawnPoint.position);
                _rewindService.RewindItems();
                
                yield return null;
            }          
            
            OnGameOver?.Invoke();
        }
    }
}