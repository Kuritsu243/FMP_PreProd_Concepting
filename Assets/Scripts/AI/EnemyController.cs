using System.Collections;
using System.Linq;
using Tutorial;
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
        [SerializeField] private float playerDetectionRange;
        [SerializeField] private float pauseBeforeAttack;
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private TutorialEnemyController tutorialEnemyController;
        private Animator _enemyAnimator;
        private bool _canMove = true;
        private EnemyShooting _enemyShooting;
        private float _velocity;
        private GameObject _player;
        private GameObject _enemyMesh;
        private NavMeshAgent _navMeshAgent;
        private Quaternion _enemyRotation;
        private Transform _enemyTransform;
        private Vector3 _targetPoint;
        private Vector3 _previousPos;

        // ReSharper disable once NotAccessedField.Local
        private TutorialEnemy _tutorialEnemy;
        private static readonly int Velocity = Animator.StringToHash("velocity");
        public bool IsTutorial { get; private set; }
        public TutorialEnemyController TutorialEnemyManager => tutorialEnemyController;


        private void Start()
        {
            _enemyMesh = gameObject.GetChildWithTag("EnemyMesh");
            _player = GameObject.FindGameObjectWithTag("PlayerMesh");
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _enemyShooting = GetComponent<EnemyShooting>();
            _enemyAnimator = GetComponentInChildren<Animator>();
            IsTutorial = TryGetComponent(out _tutorialEnemy);
            if (!IsTutorial) return;
            _navMeshAgent.enabled = false;
            _enemyShooting.enabled = false;
            _canMove = false;
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
    }
}