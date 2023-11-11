using System;
using input;
using UnityEngine;

namespace Player
{
    public class PlayerWallRunning : MonoBehaviour
    {
        [Header("Layer Masks")]
        [SerializeField] private LayerMask whatIsWall;
        [SerializeField] private LayerMask whatIsGround;
        
        [Header("Wall Run Settings")] 
        [SerializeField] private float wallRunForce;
        [SerializeField] private float maxWallRunDuration;

        [Header("Detection Settings")] 
        [SerializeField] private float maxWallDistance;
        [SerializeField] private float minJumpHeight;





        private PlayerController _playerController;
        private PlayerMovement _playerMovement;
        private inputSystem _inputSystem;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private GameObject _playerMesh;
        private Transform _playerTransform;
        private bool _leftWall;
        private bool _rightWall;
        private bool _isWallRunning;
        private float _wallRunTimer;

        public bool IsWallRunning => _isWallRunning;
        public bool LeftWall => _leftWall;
        public bool RightWall => _rightWall;
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            // _playerMovement = _playerController.playerMovement;
            _playerMesh = _playerController.playerMesh;
            _playerTransform = _playerTransform.transform;
            _inputSystem = _playerController.inputSystem;
        }


        private void CheckWalls()
        {
            var right = _playerTransform.right;
            var position = _playerTransform.position;
            _rightWall = Physics.Raycast(position, right, out _rightWallHit, maxWallDistance,
                whatIsWall);
            _leftWall = Physics.Raycast(position, -right, out _leftWallHit, maxWallDistance, whatIsWall);
        }

        private void StartWallRun()
        {
            _isWallRunning = true;
        }

  

        private void WallRunMovement()
        {
            Vector3 wallNormal = _rightWall ? _rightWallHit.normal : _leftWallHit.normal;

            Vector3 wallForward = Vector3.Cross(wallNormal, _playerTransform.up);
        }

    }
}
