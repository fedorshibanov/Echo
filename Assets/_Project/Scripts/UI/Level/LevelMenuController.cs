using _Project.Scripts.Level;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace _Project.Scripts.UI
{
    public class LevelMenuController : MonoBehaviour
    {
        [Header("Pause Menu")]
        [SerializeField] private GameObject _pauseMenu;
        [SerializeField] private Button _pContinueButton;
        [SerializeField] private Button _pMenuButton;
        
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
        private GameStateService _gameStateService;
        private PlayerInputSystem _playerInput;
        private bool _started;
        private bool _levelEnded;
        
        [Inject]
        private void Init(ISceneChanger sceneChanger, IGameEndService gameEndService,
            GameStateService gameStateService, PlayerInputSystem playerInput)
        {
            _sceneChanger = sceneChanger;
            _gameEndService = gameEndService;
            _gameStateService = gameStateService;
            _playerInput = playerInput;
        }

        private void Start()
        {
            _started = true;
            Subscribe();
        }

        private void OnEnable()
        {
            if (_started) Subscribe();
        }

        private void Subscribe()
        {
            _wMenuButton.onClick.AddListener(LoadMainMenu);
            _wNextLevelButton.onClick.AddListener(NextLevel);
            _lMenuButton.onClick.AddListener(LoadMainMenu);
            _lRestartButton.onClick.AddListener(RestartLevel);

            if (_pauseMenu != null)
            {
                _pContinueButton.onClick.AddListener(ClosePauseMenu);
                _pMenuButton.onClick.AddListener(LoadMainMenu);
            }

            _gameStateService.StateChanged += OnStateChanged;
            _playerInput.UI.Pause.performed += OnPausePerformed;
            _playerInput.UI.Enable();
            _gameEndService.OnWin += OpenWinMenu;
            _gameEndService.OnLose += OpenLoseMenu;
            OnStateChanged(_gameStateService.CurrentState);
        }

        private void OnDisable()
        {
            if (!_started) return;

            _wMenuButton.onClick.RemoveListener(LoadMainMenu);
            _wNextLevelButton.onClick.RemoveListener(NextLevel);
            _lMenuButton.onClick.RemoveListener(LoadMainMenu);
            _lRestartButton.onClick.RemoveListener(RestartLevel);

            if (_pauseMenu != null)
            {
                _pContinueButton.onClick.RemoveListener(ClosePauseMenu);
                _pMenuButton.onClick.RemoveListener(LoadMainMenu);
            }

            _playerInput.UI.Pause.performed -= OnPausePerformed;
            _playerInput.UI.Disable();
            ClosePauseMenu();
            _gameStateService.StateChanged -= OnStateChanged;
            _gameEndService.OnWin -= OpenWinMenu;
            _gameEndService.OnLose -= OpenLoseMenu;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (_levelEnded || _pauseMenu == null) return;

            _gameStateService.SetState(_gameStateService.CurrentState == GameState.Pause
                ? GameState.Ingame
                : GameState.Pause);
        }

        private void OnStateChanged(GameState state)
        {
            if (_pauseMenu == null) return;

            _pauseMenu.SetActive(state == GameState.Pause);
            if (state == GameState.Pause) _pContinueButton.Select();
        }

        private void ClosePauseMenu() => _gameStateService.SetState(GameState.Ingame);

        private void LoadMainMenu()
        {
            ClosePauseMenu();
            _sceneChanger.LoadMainMenu();
        }

        private void NextLevel() => _ = _sceneChanger.NextLevel();
        private void RestartLevel() => _ = _sceneChanger.RestartLevel();

        private void OpenWinMenu()
        {
            _levelEnded = true;
            ClosePauseMenu();
            _winMenu.SetActive(true); // temp solution
        }

        private void OpenLoseMenu()
        {
            _levelEnded = true;
            ClosePauseMenu();
            _loseMenu.SetActive(true); // temp solution
        }
    }
}
