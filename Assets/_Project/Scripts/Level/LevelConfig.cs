using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.Scripts.Level
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Level",  fileName = "Level")]
    public class LevelConfig : ScriptableObject
    {
        public AssetReference scene;
        [Space]
        public int id;
        public int echoMax;
        public float time = 10f;
    }
}
