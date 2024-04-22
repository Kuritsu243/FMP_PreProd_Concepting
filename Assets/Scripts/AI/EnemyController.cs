using System;
using System.Collections;
using System.Linq;
using Tutorial;
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
        
        [SerializeField] private float playerDetectionRange;
        [SerializeField] private float pauseBeforeAttack;
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private TutorialEnemyController tutorialEnemyController;
        
        
        
        private GameObject _player;
        private GameObject _enemyMesh;
        private Transform _enemyTransform;
        private Quaternion _enemyRotation;
        private NavMeshAgent _navMeshAgent;
        private Vector3 _targetPoint;
        private EnemyShooting _enemyShooting;
        private bool _canMove = true;
        private Animator _enemyAnimator;
        private float _velocity;
        private Vector3 _previousPos;
        private TutorialEnemy _tutorialEnemy;

        private static readonly int Velocity = Animator.StringToHash("velocity");

        public bool IsTutorial { get; set; }        
        
        public TutorialEnemy TutorialEnemyScript 
        { 
            get => _tutorialEnemy;
            set => _tutorialEnemy = value;
        }

        public TutorialEnemyController TutorialEnemyManager
        {
            get => tutorialEnemyController;
            set => tutorialEnemyController = value;
        }
        
        

        

        private void Start()
        {
            _enemyMesh = gameObject.GetChildWithTag("EnemyMesh");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _enemyShooting = GetComponent<EnemyShooting>();
            _player = GameObject.FindGameObjectWithTag("PlayerMesh");
            _enemyAnimator = GetComponentInChildren<Animator>();
            IsTutorial = TryGetComponent(out _tutorialEnemy);
            if (!IsTutorial) return;
            _navMeshAgent.enabled = false;
            _enemyShooting.enabled = false;
            _canMove = false;
            // _wallRunLeft = 1 << NavMesh.GetAreaFromName("WallRunLeft");
            // _wallRunRight = 1 << NavMesh.GetAreaFromName("WallRunRight");
            // _defaultSpeed = _navMeshAgent.speed;
        }

        public void EnableEnemy()
        {
            _navMeshAgent.enabled = true;
            _enemyShooting.enabled = true;
            _canMove = true;
        }
        
        private void FixedUpdate()
        {
            if (!_enemyShooting.enabled) return;
            if (_enemyShooting.CurrentWeapon.CurrentPrimaryAmmo == 0)
            {
                _enemyShooting.Reload();
                return;
            }
            if (Vector3.Distance(transform.position, _player.transform.position) > playerDetectionRange) return;
            if (IsFacingPlayer() && _enemyShooting.CanAttack) PrepareToShoot();
            if (_targetPoint == _player.transform.position) return;
            _targetPoint = _player.transform.position;
            if (_canMove) _navMeshAgent.SetDestination(_targetPoint);
        }

        private void LateUpdate()
        {
            _velocity = (_enemyMesh.transform.position - _previousPos).magnitude / Time.deltaTime;
            _previousPos = _enemyMesh.transform.position;
            _enemyAnimator.SetFloat(Velocity, _velocity);
        }

        private bool IsFacingPlayer()
        {
            return Vector3.Dot(transform.forward, (_player.transform.position - transform.position).normalized) > 0.95f;
        }

        private void PrepareToShoot()
        {
            _navMeshAgent.isStopped = true;
            _canMove = false;
            transform.LookAt(_player.transform);
            StartCoroutine(WaitBeforeAttack());
        }

        private IEnumerator WaitBeforeAttack()
        {
            if (!_enemyShooting.CanAttack) yield break; 
            yield return new WaitForSeconds(pauseBeforeAttack);
            _enemyShooting.Fire();
            _navMeshAgent.isStopped = false;
            _canMove = true;
            StartCoroutine(TimeBeforeAttacks());
        }

        private IEnumerator TimeBeforeAttacks()
        {
            _enemyShooting.CanAttack = false;
            yield return new WaitForSeconds(timeBetweenAttacks);
            _enemyShooting.CanAttack = true;
        }
        
        

        // private void CheckIfPathExists()
        // {
        //     // var pathexists = _navMeshAgent.CalculatePath(_player.transform.position, _navMeshAgent.path) && _navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial;
        //     // Debug.LogWarning("Does Path exist?: " + pathexists);
        // }


        // private void GroundCheck()
        // {
        //     if (!Physics.Raycast(transform.position, Vector3.down, out var hit)) return;
        //     if (Physics.Raycast(_player.transform.position, Vector3.down, out var playerHit))
        //         _playerFloorID = playerHit.transform.GetInstanceID();
        //     
        //     _floorID = hit.transform.GetInstanceID();
        //
        //     
        // }

        // private void CheckIfOnWalls()
        // {
        //     var position = transform.position;
        //     _leftWall = NavMesh.SamplePosition(position, out _enemyNavHit, 0.1f, _wallRunLeft) && 
        //                 _enemyNavHit.mask == _wallRunLeft;
        //     _rightWall = NavMesh.SamplePosition(position, out _enemyNavHit, 0.1f, _wallRunRight) && 
        //                 _enemyNavHit.mask == _wallRunRight;
        //     _onWall = _leftWall || _rightWall;
        //     
        //     var rotation = _enemyMesh.transform.rotation;
        //
        //     // switch (_onWall)
        //     // {
        //     //     case true:
        //     //         _navMeshAgent.enabled = false;
        //     //         if (_floorID != _playerFloorID)
        //     //         {
        //     //             var wallNormal = _enemyNavHit.normal;
        //     //             var wallForward = Vector3.Cross(wallNormal, transform.up);
        //     //             if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
        //     //                 wallForward = -wallForward;
        //     //             if (_leftWall)
        //     //                 transform.Translate(wallForward * (15f * Time.deltaTime));
        //     //             if (_rightWall)
        //     //                 transform.Translate(-wallNormal * (100 * Time.deltaTime));
        //     //             
        //     //         }
        //     //         break;
        //     //     case false:
        //     //         _navMeshAgent.enabled = true;
        //     //         break;
        //     // }
        //     
        //     
        //     switch (_onWall)
        //     {
        //         case true when _rightWall && !_meshRotated:
        //             _enemyMesh.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, -90f);
        //             _meshRotated = true;
        //             break;
        //         case true when _leftWall && !_meshRotated:
        //             _enemyMesh.transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 90f);
        //             _meshRotated = true;
        //             break;
        //         case false when _meshRotated:
        //             _enemyMesh.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
        //             _meshRotated = false;
        //             break;
        //     }
        //     
        //     switch (_onWall)
        //     {
        //         case true when !_hasAppliedAcceleration:
        //             // _navMeshAgent.autoRepath = false;
        //             _navMeshAgent.speed *= wallRunSpeedMultiplier;
        //             _hasAppliedAcceleration = true;
        //             break;
        //         case false when _hasAppliedAcceleration:
        //             // _navMeshAgent.autoRepath = true;
        //             _navMeshAgent.speed = _defaultSpeed;
        //             _hasAppliedAcceleration = false;
        //             break;
        //     }
        //     
        // }
        
    }
}
