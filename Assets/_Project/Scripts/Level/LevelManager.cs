using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Echo;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData _levelData;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private PlayerController _player;
        
        public event Action OnGameOver;

        private List<EchoController> _echos = new List<EchoController>();

        private IRecordService _recordService;
        private EchoFactory _echoFactory;
        
        [Inject]
        private void Init(IRecordService recordService, EchoFactory echoFactory)
        {
            _recordService = recordService;
            _echoFactory = echoFactory;
        }

        private void Start()
        {
            StartLevel();
        }

        public void StartLevel()
        {
            ReloadEntities(_player,  _echos);
            StartCoroutine(StartGameCycle());
        }

        private IEnumerator StartGameCycle()
        {
            for (var i = 0; i < _levelData.echoMax; i++)
            {
                var frames = new List<InputFrame>();
                _recordService.TryStartRecord(frames, _levelData.time);
                yield return new WaitForSeconds(_levelData.time);

                var echo = _echoFactory.Spawn(frames);
                _echos.Add(echo);
                ReloadEntities(_player, _echos);
                yield return null;
            }          
        }

        private void ReloadEntities(PlayerController player, List<EchoController> echos)
        {
            player.transform.position = _spawnPoint.position;
            
            foreach (var echo in echos)
            {
                echo.transform.position = _spawnPoint.position;
                echo.MoveEcho();
            }
        }
    }
}