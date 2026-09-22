using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpForce;
        
        [Space]
        [SerializeField] private Rigidbody _rb;
        
        private PlayerInputSystem _playerInput;

        [Inject]
        private void Init(PlayerInputSystem playerInput)
        {
            _playerInput = playerInput;
        }

        private void OnEnable()
        {
            _playerInput.Enable();
            _playerInput.Player.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            _playerInput.Disable();
            _playerInput.Player.Jump.performed -= OnJump;
        }

        private void OnDestroy()
        {
            _playerInput.Dispose();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            var direction = _playerInput.Player.Move.ReadValue<Vector2>();
            transform.Translate(direction * (_moveSpeed * Time.fixedDeltaTime));
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}
