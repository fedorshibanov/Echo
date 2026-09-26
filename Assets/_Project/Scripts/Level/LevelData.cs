using UnityEngine;

namespace _Project.Scripts.Level
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Level",  fileName = "Level")]
    public class LevelData : ScriptableObject
    {
        public int id;
        public int echoMax;
        public float time = 10f;
    }
}
