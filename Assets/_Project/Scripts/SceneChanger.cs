using System.Threading.Tasks;
using _Project.Scripts.Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public interface ISceneChanger
    {
        int CurrentLevelIndex { get; }
        
        Task NextLevel();
        Task LoadLevel(int levelIndex);
        void LoadMainMenu();
    }
    
    public class SceneChanger : ISceneChanger
    {
        public int CurrentLevelIndex { get; private set; }
        
        private LevelConfig[] _levelConfigs = Resources.LoadAll<LevelConfig>("Config/Levels");
        
        public async Task NextLevel()
        {
            CurrentLevelIndex++;
            await LoadLevel(_levelConfigs[CurrentLevelIndex]);
        }

        public async Task LoadLevel(int levelIndex)
        {
            CurrentLevelIndex = levelIndex;
            await LoadLevel(_levelConfigs[levelIndex]);
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        private async Task LoadLevel(LevelConfig config)
        {
            await config.scene.LoadSceneAsync(LoadSceneMode.Single).Task;
        }
    }
}
