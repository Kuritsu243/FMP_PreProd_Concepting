using System;
using UnityEngine;

namespace Player
{
    // src:
    public class PlayerAnimation : MonoBehaviour
    {
        private PlayerController _playerController;
        private Animator _playerAnimator;
        private float _velocity;
        private Vector3 _previousPos;
        private GameObject _playerMesh;
        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int HasWeapon = Animator.StringToHash("hasWeapon");

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            _playerMesh = _playerController.playerMesh;
            _playerAnimator = _playerMesh.GetComponent<Animator>();

        }
        

        private void LateUpdate()
        {
            _velocity = (_playerMesh.transform.position - _previousPos).magnitude / Time.deltaTime;
            _previousPos = _playerMesh.transform.position;
            _playerAnimator.SetBool(HasWeapon, _playerController.playerShooting.CurrentWeapon);

            _playerAnimator.SetFloat(Velocity, _velocity);
        }
    }
}
