using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerProjectilePool : MonoBehaviour
    {
        public List<GameObject> pooledProjectiles;

        [SerializeField] private GameObject objectToPool;
        [SerializeField] private int amountToPool;

        private GameObject _projParent;
        

        private void Start()
        {
            _projParent = GameObject.FindGameObjectWithTag("ProjectilePool");
            pooledProjectiles = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                var tmp = Instantiate(objectToPool, _projParent.transform);
                tmp.SetActive(false);
                pooledProjectiles.Add(tmp);
            }
        }

        public GameObject GetPooledProjectile()
        {
            for (int i = 0; i < amountToPool; i++)
            {
                if (!pooledProjectiles[i].activeInHierarchy)
                    return pooledProjectiles[i];
            }

            return null;
        }
    }
}
