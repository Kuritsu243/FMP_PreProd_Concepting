using System;
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


        private PlayerMovement _playerMovement;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private Transform _playerTransform;
        private bool _rightWall;
        private float _wallRunTimer;
        
    }
}
