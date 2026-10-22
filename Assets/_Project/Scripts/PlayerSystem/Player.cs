using _Project.Scripts.EchoSystem;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.RewindSystem;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace _Project.Scripts.PlayerSystem
{
    public class Player : MonoBehaviour, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;
        
        [SerializeField] private PlayerController _controller;
        public PlayerController Controller => _controller;
        
        [SerializeField] private Rigidbody _rb;
        public Rigidbody Rigidbody => _rb;
    }
}