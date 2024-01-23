using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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
        private Transform EnemyTransform => _enemyMesh.transform;
        private Quaternion EnemyRotation => EnemyTransform.rotation;
        private NavMeshAgent _navMeshAgent;
        private Vector3 _targetPoint;
        private NavMeshHit _enemyNavHit;
        private int wallRunLeft;
        private int wallRunRight;
        private int wallRunAreas;
        private bool _onWall;
        private bool _meshRotated;
        private bool _leftWall;
        private bool _rightWall;
        [SerializeField] private float playerDetectionRange;
        [SerializeField] private float wallRunSpeedMulitplier;
        

        

        private void Start()
        {
            _enemyMesh = gameObject.GetChildWithTag("EnemyMesh");

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag("PlayerMesh");
            wallRunLeft = 1 <<  NavMesh.GetAreaFromName("WallRunLeft");
            wallRunRight = 1 << NavMesh.GetAreaFromName("WallRunRight");

        }

        private void FixedUpdate()
        {
            CheckIfOnWalls();
            if (Vector3.Distance(transform.position, _player.transform.position) > playerDetectionRange) return;
            if (_targetPoint == _player.transform.position) return;
            _targetPoint = _player.transform.position;
            _navMeshAgent.SetDestination(_targetPoint);
  

        }

        private void CheckIfOnWalls()
        {
            _leftWall = NavMesh.SamplePosition(transform.position, out _enemyNavHit, 1.0f, wallRunLeft) && _enemyNavHit.mask == wallRunLeft;
            _rightWall = NavMesh.SamplePosition(transform.position, out _enemyNavHit, 1.0f, wallRunRight) &&
                         _enemyNavHit.mask == wallRunRight;
            _onWall = _leftWall || _rightWall;
            
            // if (_onWall)
            //     _navMeshAgent. *= wallRunSpeedMulitplier;
            switch (_onWall)
            {

                case true when !_meshRotated && _leftWall:
                    EnemyTransform.Rotate(0, 0, 90, Space.Self);
                    _meshRotated = true;
                    break;
                case true when !_meshRotated && _rightWall:
                    EnemyTransform.Rotate(0, 0, -90, Space.Self);
                    _meshRotated = true;
                    break;
                case false when _meshRotated && !_leftWall:
                    EnemyTransform.Rotate(0, 0, -270, Space.Self);
                    _meshRotated = false;
                    break;
                case false when _meshRotated && !_rightWall:
                    EnemyTransform.Rotate(0, 0, 270, Space.Self);
                    break;
            }
    
        }
        
    }
}
