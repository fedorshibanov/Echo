using UnityEngine;

namespace _Project.Scripts.InteractionObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Box : MonoBehaviour, IRewindable, IPlateInteractable
    {
        [SerializeField] private float _mass;
        public float Mass => _mass;
        
        [Space]
        [SerializeField] private Rigidbody _rb;
        
        private Vector3 _startPoint;
        
        private void Start()
        {
            _startPoint = transform.position; // лучше переделать под entry point // на потом
        }
        
        public void Rewind()
        {
            transform.position = _startPoint;
            transform.rotation = Quaternion.identity;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}