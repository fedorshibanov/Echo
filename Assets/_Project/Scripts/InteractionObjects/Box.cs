using _Project.Scripts.RewindSystem;
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
        private Quaternion _startRotation;
        
        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            _startPoint = transform.position;
            _startRotation = transform.rotation;
        }

        public void Rewind()
        {
            transform.position = _startPoint;
            transform.rotation = _startRotation;
            if (!_rb.isKinematic)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
