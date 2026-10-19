using _Project.Scripts.SaveSystem;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Scripts.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _aboutMeButton;
        [SerializeField] private Button _quitButton;

        private int _currentLevelId;
        
        private ISceneChanger _sceneChanger;
        private ISaveService _saveService;
        
        [Inject]
        private void Init(ISceneChanger sceneChanger, ISaveService saveService)
        {
            _sceneChanger = sceneChanger;
            _saveService = saveService;
        }

        private void Start()
        {
            InitButtons();
        }

        private void InitButtons()
        {
            _newGameButton.onClick.AddListener(NewGame);
            _quitButton.onClick.AddListener(Quit);
            _aboutMeButton.onClick.AddListener(AboutMe);

            if (_sceneChanger.CurrentLevelId == 0)
            {
               _continueButton.interactable = false;
                return;
            }
            
            _continueButton.onClick.AddListener(Continue);
        }

        private void NewGame()
        {
            _sceneChanger.LoadLevel(0);
            _saveService.DeleteAllSaves();
        }
        
        private void Continue()
        {
            _sceneChanger.LoadLevel(_sceneChanger.CurrentLevelId);
        }
        
        private void AboutMe()
        {
            Application.OpenURL("https://github.com/fedorshibanov");
        }

        private void Quit()
        {
            Application.Quit();
        }
    }
}