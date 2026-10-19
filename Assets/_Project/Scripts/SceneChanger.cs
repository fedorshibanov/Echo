using System.Threading.Tasks;
using _Project.Scripts.Level;
using _Project.Scripts.SaveSystem;
using _Project.Scripts.VContainer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public interface ISceneChanger
    {
        int CurrentLevelId { get; }

        void LoadLevelId(int id);
        Task NextLevel();
        Task LoadLevel(int levelIndex);
        Task RestartLevel();
        void LoadMainMenu();
    }
    
    public class SceneChanger : ISceneChanger
    {
        public int CurrentLevelId { get; private set; }
        
        private LevelConfig[] _levelConfigs = Resources.LoadAll<LevelConfig>("Config/Levels");

        private bool isLoaded = false;
        
        public void LoadLevelId(int id)
        {
            if (isLoaded) return;

            CurrentLevelId = id;
            isLoaded = true;
        }

        public async Task NextLevel()
        {
            if (_levelConfigs[CurrentLevelId + 1] == null) return;

            CurrentLevelId++;
            await LoadLevel(_levelConfigs[CurrentLevelId]);
        }

        public async Task LoadLevel(int levelIndex)
        {
            if (_levelConfigs[levelIndex] == null) return;
            
            CurrentLevelId = levelIndex;
            await LoadLevel(_levelConfigs[levelIndex]);
        }

        public async Task RestartLevel()
        {
            await LoadLevel(_levelConfigs[CurrentLevelId]);
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        private async Task LoadLevel(LevelConfig config)
        {
            await Addressables.LoadSceneAsync(config.scene, LoadSceneMode.Single).Task;
        }
    }
}
