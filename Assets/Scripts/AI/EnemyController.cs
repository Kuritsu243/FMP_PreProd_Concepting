using System;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class EnemyController : MonoBehaviour
    {
        private GameObject _player;
        private NavMeshAgent _navMeshAgent;
        private Vector3 _targetPoint;

        [SerializeField] private float playerDetectionRange;
        

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _player = GameObject.FindGameObjectWithTag("PlayerMesh");
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, _player.transform.position) > playerDetectionRange) return;
            if (_targetPoint == _player.transform.position) return;
            _targetPoint = _player.transform.position;
            _navMeshAgent.SetDestination(_targetPoint);

        }
        
    }
}
