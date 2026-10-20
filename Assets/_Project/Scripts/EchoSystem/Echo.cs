using _Project.Scripts.InteractionObjects;
using _Project.Scripts.RewindSystem;
using UnityEngine;
using VContainer;

namespace _Project.Scripts.EchoSystem
{
    public class Echo : MonoBehaviour, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;

        public EchoController EchoController { get; private set; }

        [Inject]
        private void Initialize(EchoController echoController)
        {
            EchoController = echoController;
        }
    }
}