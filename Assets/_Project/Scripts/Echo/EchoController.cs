using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.InteractionObjects;
using _Project.Scripts.RecordSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _Project.Scripts.Echo
{
    public class EchoController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpForce;
        [Space]
        [SerializeField] private Rigidbody _rb;
        
        private List<InputFrame> _inputFrames;
        private bool _isPlaying;

        public void MoveEcho()
        {
            if (_isPlaying) return;
            
            StartCoroutine(PlayRecord(_inputFrames));
        }
        
        public void SetFrames(List<InputFrame> frames)
        {
            _inputFrames = frames;
        }
        
        public void Rewind(Transform startPoint)
        {
            transform.position = startPoint.position;
            MoveEcho();
        }

        private IEnumerator PlayRecord(List<InputFrame> inputFrames)
        {
            _isPlaying = true;
            foreach (var frame in inputFrames)
            {
                HorizontalMovement(frame.Move);
                Jump(frame.Jump);
                yield return new WaitForFixedUpdate();
            }
            _isPlaying = false;
        }
        
        private void HorizontalMovement(Vector2 direction)
        {
            transform.Translate(direction * (_moveSpeed * Time.fixedDeltaTime));
        }
        
        private void Jump(bool isJump)
        {
            if (isJump)
                _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }
}