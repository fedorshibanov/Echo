using _Project.Scripts.InteractionObjects;
using UnityEngine;

namespace _Project.Scripts.EchoSystem
{
    public class Echo : MonoBehaviour, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;
    }
}