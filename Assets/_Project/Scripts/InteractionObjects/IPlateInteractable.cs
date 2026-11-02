using System;

namespace _Project.Scripts.InteractionObjects
{
    public interface IPlateInteractable
    {
        public float Mass { get; }
        public event Action<IPlateInteractable> OnDisabled;
    }
}
