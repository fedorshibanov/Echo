using _Project.Scripts.InteractionObjects;
using UnityEngine;

namespace _Project.Scripts.PlayerSystem
{
    public class Player : MonoBehaviour, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;
    }
}