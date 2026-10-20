using _Project.Scripts.InteractionObjects;
using _Project.Scripts.RewindSystem;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.PlayerSystem
{
    public class Player : MonoBehaviour, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;
        
        public PlayerController PlayerController { get; private set; }

        [Inject]
        private void Init(PlayerController playerController)
        {
            PlayerController = playerController;
        }
    }
}