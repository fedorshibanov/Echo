using System;
using System.Collections.Generic;
using _Project.Scripts.Animations;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.InteractionObjects
{
    public class PressurePlate : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float _requiredMass;
        [SerializeField] private Transform _model;
        [SerializeField] private TMP_Text _requiredMassText;
        [SerializeField, Min(0f)] private float _pressDepth = 0.08f;
        [SerializeField, Min(0f)] private float _pressDuration = 0.2f;

        public event Action OnActivated;
        public event Action OnDeactivated;
        public event Action OnChanged;

        private readonly Dictionary<IPlateInteractable, int> _occupants = new();
        private float _allMass;
        private bool _isActivated;
        private Vector3 _restPosition;
        private Tween _pressTween;

        private void Awake()
        {
            if (_model != null)
                _restPosition = _model.localPosition;

            UpdateMassText();
        }

        private void OnValidate() => UpdateMassText();

        private void OnDisable()
        {
            _pressTween?.Kill();
            if (_model != null)
                _model.localPosition = _restPosition;

            foreach (var occupant in _occupants.Keys)
                occupant.OnDisabled -= RemoveOccupant;
            _occupants.Clear();

            _allMass = 0f;
            UpdateMassText();
            if (_isActivated)
            {
                _isActivated = false;
                OnDeactivated?.Invoke();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IPlateInteractable>(out var obj))
                return;

            if (_occupants.TryGetValue(obj, out int contacts))
            {
                _occupants[obj] = contacts + 1;
                return;
            }

            _occupants.Add(obj, 1);
            obj.OnDisabled += RemoveOccupant;
            _allMass += obj.Mass;
            UpdateLoad();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IPlateInteractable>(out var obj))
                return;

            if (!_occupants.TryGetValue(obj, out int contacts))
                return;

            if (contacts > 1)
            {
                _occupants[obj] = contacts - 1;
                return;
            }

            RemoveOccupant(obj);
        }

        private void RemoveOccupant(IPlateInteractable obj)
        {
            if (!_occupants.Remove(obj))
                return;

            obj.OnDisabled -= RemoveOccupant;
            _allMass = Mathf.Max(0f, _allMass - obj.Mass);
            UpdateLoad();
        }

        private void UpdateLoad()
        {
            if (_model != null)
            {
                _pressTween?.Kill();
                float load = _requiredMass > 0f ? Mathf.Clamp01(_allMass / _requiredMass) : 1f;
                _pressTween = Anim.Level.PressPlate(_model, _restPosition.y, _pressDepth, load, _pressDuration);
            }

            UpdateMassText();
            OnChanged?.Invoke();

            bool isActivated = _allMass >= _requiredMass;
            if (_isActivated == isActivated)
                return;

            _isActivated = isActivated;
            if (_isActivated)
                OnActivated?.Invoke();
            else
                OnDeactivated?.Invoke();
        }

        private void UpdateMassText()
        {
            if (_requiredMassText != null)
                _requiredMassText.text = $"{_allMass:0.##} / {_requiredMass:0.##} kg";
        }
    }
}
