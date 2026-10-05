using System;
using UnityEngine;

namespace _Project.Scripts.InteractionObjects
{
    public class PressurePlate : MonoBehaviour, IRewindable
    {
        [SerializeField] private float _requiredMass;
        
        public event Action OnActivated;
        public event Action OnDeactivated;
        public event Action OnChanged;

        private float _allMass;
        private bool _isActivated;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IPlateInteractable>(out var obj))
                return;
            
            _allMass += obj.Mass;
            OnChanged?.Invoke();

            if (_allMass >= _requiredMass)
            {
                _isActivated = true;
                OnActivated?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IPlateInteractable>(out var obj))
                return;
            
            _allMass -= obj.Mass;
            OnChanged?.Invoke();

            if (!(_allMass >= _requiredMass) &&  _isActivated)
            {
                _isActivated = false;
                OnDeactivated?.Invoke();
            }
        }

        public void Rewind()
        {
            
        }
    }
}
