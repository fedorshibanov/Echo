using System;
using UnityEngine;

namespace _Project.Scripts.InteractionObjects
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private PressurePlate _plate;

        private void OnEnable()
        {
            _plate.OnActivated += Open;
            _plate.OnDeactivated += Close;
        }

        private void OnDisable()
        {
            _plate.OnActivated += Open;
            _plate.OnDeactivated += Close;
        }

        private void Open()
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }

        private void Close()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}