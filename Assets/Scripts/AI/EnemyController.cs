using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace AI
{
    public static class GameObjectExtensions
    {
        public static GameObject GetChildWithTag(this GameObject parent, string tag)
        {
            var t = parent.transform;
            return (from Transform tr in t where tr.CompareTag(tag) select tr.gameObject).FirstOrDefault();
        }
    }
    
    public class EnemyController : MonoBehaviour
    {
        private GameObject _player;
        private GameObject _enemyMesh;
        private Transform _enemyTransform;
        private Quaternion _enemyRotation;
        private NavMeshAgent _navMeshAgent;
        private Vector3 _targetPoint;
        private NavMeshHit _enemyNavHit;
        private int _wallRunLeft;
        private int _wallRunRight;
        private int _wallRunAreas;
        private float _defaultSpeed;
        private bool _onWall;
        private bool _meshRotated;
        private bool _leftWall;
        private bool _rightWall;
        private bool _hasAppliedAcceleration;
        [SerializeField] private float playerDetectionRange;
        [SerializeField] private float wallRunSpeedMultiplier;
        

        

        private void Start()
        {
            _enemyMesh = gameObject.GetChildWithTag("EnemyMesh");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag("PlayerMesh");
            _wallRunLeft = 1 <<  NavMesh.GetAreaFromName("WallRunLeft");
            _wallRunRight = 1 << NavMesh.GetAreaFromName("WallRunRight");
            _defaultSpeed = _navMeshAgent.speed;

        }

        private void FixedUpdate()
        {
            CheckIfOnWalls();
            var rotation = _enemyMesh.transform.rotation;
            
            if (!_onWall)
                _enemyMesh.transform.localRotation = Quaternion.Euler(rotation.x, transform.rotation.y, rotation.z);
            
            if (Vector3.Distance(transform.position, _player.transform.position) > playerDetectionRange) return;
            if (_targetPoint == _player.transform.position) return;
            _targetPoint = _player.transform.position;
            _navMeshAgent.SetDestination(_targetPoint);


        }

        private void CheckIfOnWalls()
        {
            var position = transform.position;
            _leftWall = NavMesh.SamplePosition(position, out _enemyNavHit, 0.1f, _wallRunLeft) && _enemyNavHit.mask == _wallRunLeft;
            _rightWall = NavMesh.SamplePosition(position, out _enemyNavHit, 0.1f, _wallRunRight) &&
                         _enemyNavHit.mask == _wallRunRight;
            _onWall = _leftWall || _rightWall;

            var rotation = _enemyMesh.transform.rotation;
            switch (_onWall)
            {
                case true when _rightWall && !_meshRotated:
                    _enemyMesh.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, -90f);
                    _meshRotated = true;
                    break;
                case true when _leftWall && !_meshRotated:
                    _enemyMesh.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 90f);
                    _meshRotated = true;
                    break;
                case false when _meshRotated:
                    _enemyMesh.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
                    _meshRotated = false;
                    break;
            }

            switch (_onWall)
            {
                case true when !_hasAppliedAcceleration:
                    _navMeshAgent.speed *= wallRunSpeedMultiplier;
                    _hasAppliedAcceleration = true;
                    break;
                case false when _hasAppliedAcceleration:
                    _navMeshAgent.speed = _defaultSpeed;
                    _hasAppliedAcceleration = false;
                    break;
            }
            

    
        }
        
    }
}
