using System;
using _Project.Scripts.Level;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Scripts.UI
{
    public class LevelMenuController : MonoBehaviour
    {
        [Header("Pause Menu")]
        
        [Space, Header("Win Menu")]
        [SerializeField] private GameObject _winMenu;
        [SerializeField] private Button _wNextLevelButton;
        [SerializeField] private Button _wMenuButton;
        
        [Space, Header("Lose Menu")]
        [SerializeField] private GameObject _loseMenu;
        [SerializeField] private Button _lRestartButton;
        [SerializeField] private Button _lMenuButton;
        
        private ISceneChanger _sceneChanger;
        private IGameEndService _gameEndService;
        
        [Inject]
        private void Init(ISceneChanger sceneChanger, IGameEndService gameEndService)
        {
            _sceneChanger = sceneChanger;
            _gameEndService = gameEndService;
        }

        private void Start()
        {
            InitButtons();
            
            _gameEndService.OnWin += OpenWinMenu;
            _gameEndService.OnLose += OpenLoseMenu;
        }

        private void OnDisable()
        {
            _gameEndService.OnWin -= OpenWinMenu;
            _gameEndService.OnLose -= OpenLoseMenu;
        }

        private void InitButtons()
        {
            _wMenuButton.onClick.AddListener(() => _sceneChanger.LoadMainMenu());
            _wNextLevelButton.onClick.AddListener(() => _sceneChanger.NextLevel());
            
            _lMenuButton.onClick.AddListener(() => _sceneChanger.LoadMainMenu());
            _lRestartButton.onClick.AddListener(() => _sceneChanger.RestartLevel());
        }

        private void OpenWinMenu()
        {
            _winMenu.SetActive(true); // temp solution
        }

        private void OpenLoseMenu()
        {
            _loseMenu.SetActive(true); // temp solution
        }
    }
}