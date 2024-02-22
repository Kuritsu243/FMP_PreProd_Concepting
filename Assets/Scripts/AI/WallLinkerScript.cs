using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{

    
    
    public class WallLinkerScript : MonoBehaviour
    {
        private NavMeshLink[] _navMeshLinks;
        
        
        private void Start()
        {
            _navMeshLinks = GetComponentsInChildren<NavMeshLink>();
        }
    }
}
