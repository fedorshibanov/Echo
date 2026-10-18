using System;
using System.IO;
using UnityEngine;

namespace _Project.Scripts.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int levelId;
    }
    
    public class JsonSaveService : ISaveService
    {
        private readonly string _path = Path.Combine(Application.persistentDataPath, "save.json");

        private readonly ISceneChanger _sceneChanger;
        
        public JsonSaveService(ISceneChanger sceneChanger)
        {
            _sceneChanger = sceneChanger;
        }

        public void Save()
        {
            var data = new SaveData()
            {
                levelId = _sceneChanger.CurrentLevelId + 1
            };
            
            Save(data);
        }
        
        public void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_path, json);
            
            Debug.Log($"Save complete. path: {_path}");
        }

        public SaveData Load()
        {
            if (!File.Exists(_path))
                return new SaveData {levelId = 0};

            try
            {
                return JsonUtility.FromJson<SaveData>(File.ReadAllText(_path));
            }
            catch (Exception e)
            {
                Debug.LogError($"Save corrupted: {e}");
                return null;
            }
        }
    }
}