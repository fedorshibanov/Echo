using _Project.Scripts.InteractionObjects;
using _Project.Scripts.RewindSystem;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace _Project.Scripts.EchoSystem
{
    public class Echo : MonoBehaviour, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;

        [SerializeField] private EchoController _controller;
        public EchoController Controller => _controller;

        [SerializeField] private Rigidbody _rb;
        public Rigidbody Rigidbody => _rb;
    }
}